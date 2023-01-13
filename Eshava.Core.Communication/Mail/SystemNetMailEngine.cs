﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Eshava.Core.Communication.Mail.Interfaces;
using Eshava.Core.Communication.Models;
using Eshava.Core.Extensions;
using Eshava.Core.Models;

namespace Eshava.Core.Communication.Mail
{
	public class SystemNetMailEngine : IEmailEngine
	{
		private readonly EmailEngineSettings _settings;

		public SystemNetMailEngine(EmailEngineSettings settings)
		{
			_settings = settings;
		}

		public string Type => "System.Net.Mail";

		public Task<ResponseData<bool>> SendMailAsync(EmailData emailData, EmailAccount emailAccount)
		{
			var sender = emailData.Sender;
			if (sender.IsNullOrEmpty())
			{
				sender = emailAccount.Username;
			}

			var client = CreateSmtpClient(emailAccount);
			var mailMessage = new MailMessage
			{
				From = emailData.SenderDisplayName.IsNullOrEmpty()
					? new MailAddress(sender?.Trim())
					: new MailAddress(sender?.Trim(), emailData.SenderDisplayName),
				Subject = emailData.Subject,
				Body = emailData.Body,
				IsBodyHtml = emailData.Html
			};

			return SendMailAsync(client, mailMessage, emailData,emailAccount.ChunkSize);
		}

		public Task<ResponseData<bool>> SendMailAsync(EmailData emailData)
		{
			var emailAccount = new EmailAccount
			{
				SmtpServer = _settings.SmtpServer,
				Username = _settings.Username,
				Password = _settings.Password,
				Port = _settings.Port,
				SSL = _settings.SSL,
				SendWithoutAuthentication = _settings.SendWithoutAuthentication,
				ChunkSize = _settings.ChunkSize,
			};

			return SendMailAsync(emailData, emailAccount);
		}

		/// <summary>
		/// Creates a smtp client
		/// </summary>
		/// <param name="username">Username of the email account</param>
		/// <param name="password">Password of the sender</param>
		/// <returns>SmtpClient</returns>
		private SmtpClient CreateSmtpClient(EmailAccount emailAccount)
		{
			var client = new SmtpClient(emailAccount.SmtpServer, emailAccount.Port);

			if (emailAccount.SendWithoutAuthentication)
			{
				//Authentication of the sender is required
				client.UseDefaultCredentials = false;
				client.Credentials =  new NetworkCredential(emailAccount.Username?.Trim(), emailAccount.Password);
			}
			else
			{
				//Anonymous sending the mail
				client.UseDefaultCredentials = false;
				client.Credentials = null;
			}

			client.EnableSsl = emailAccount.SSL;

			return client;
		}

		private async Task<ResponseData<bool>> SendMailAsync(SmtpClient client, MailMessage mailMessage, EmailData emailData, int chunkSize)
		{
			var recipients = emailData.Recipients
				?.Where(recipient => IsAllowEmailAddress(recipient))
				.Select(recipient => recipient.Trim())
				.ToList()
				?? new List<string>();

			var recipientsCC = emailData.RecipientsCC
				?.Where(recipient => IsAllowEmailAddress(recipient))
				.Select(recipient => recipient.Trim())
				.ToList()
				?? new List<string>();

			var recipientsBCC = emailData.RecipientsBCC
				?.Where(recipient => IsAllowEmailAddress(recipient))
				.Select(recipient => recipient.Trim())
				.ToList()
				?? new List<string>();

			if (recipients.Count == 0 && recipientsCC.Count == 0 && recipientsBCC.Count == 0)
			{
				if (_settings.FallbackReceiverEmailAddress.IsNullOrEmpty())
				{
					return ResponseData<bool>.CreateFaultyResponse("INVALIDDATA", validationResult: new List<ValidationError>
					{
						new ValidationError
						{
							PropertyName = nameof(EmailData.Recipients),
							ErrorType = "Required"
						}
					});
				}

				recipients.Add(_settings.FallbackReceiverEmailAddress);
			}

			do
			{
				mailMessage.CC.Clear();
				mailMessage.Bcc.Clear();
				mailMessage.To.Clear();

				var availableRecipents = chunkSize;
				recipients = AddRecipients(recipients, ref availableRecipents, recipient => mailMessage.To.Add(recipient));
				recipientsCC = AddRecipients(recipientsCC, ref availableRecipents, recipient => mailMessage.CC.Add(recipient));
				recipientsBCC = AddRecipients(recipientsBCC, ref availableRecipents, recipient => mailMessage.Bcc.Add(recipient));

				await client.SendMailAsync(mailMessage);
			}
			while (recipients.Count > 0 || recipientsCC.Count > 0 || recipientsBCC.Count > 0);

			return new ResponseData<bool>(true);
		}

		private List<string> AddRecipients(List<string> recipients, ref int availableRecipents, Action<string> addRecipent)
		{
			if (recipients.Count == 0)
			{
				return recipients;
			}

			if (recipients.Count > availableRecipents)
			{
				recipients.Take(availableRecipents).ToList().ForEach(addRecipent);
				recipients = recipients.Skip(availableRecipents).ToList();
				availableRecipents = 0;
			}
			else
			{
				recipients.ForEach(addRecipent);
				availableRecipents -= recipients.Count;
				recipients.Clear();
			}

			return recipients;
		}

		private bool IsAllowEmailAddress(string emailAddress)
		{
			if (emailAddress.IsNullOrEmpty())
			{
				return false;
			}

			if (!(_settings.DomainWhiteList?.Any() ?? false))
			{
				return true;
			}

			emailAddress = emailAddress.Trim().ToLower();

			return _settings.DomainWhiteList.Any(domain => emailAddress.EndsWith(domain.ToLower()));
		}
	}
}