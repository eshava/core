using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Eshava.Core.Communication.Mail.Interfaces;
using Eshava.Core.Communication.Models;

namespace Eshava.Core.Communication.Mail
{
	public class SystemNetMailEngine : IMailEngine
	{
		private readonly MailEngineSettings _settings;
		
		public SystemNetMailEngine(MailEngineSettings settings)
		{
			_settings = settings;
		}

		public string Type => "System.Net.Mail";

		public async Task<IMailEngine> SendMailAsync(MailData mail, string sender, string password)
		{
			var client = CreateSmtpClient(sender, password);
			var mailMessage = new MailMessage
			{
				From = new MailAddress(sender),
				Subject = mail.Subject,
				Body = mail.Body,
				IsBodyHtml = mail.Html
			};

			await SendMailAsync(client, mailMessage, mail);

			return this;
		}

		public async Task<IMailEngine> SendMailAsync(MailData mail)
		{
			await SendMailAsync(mail, _settings.Sender, _settings.Password);

			return this;
		}

		/// <summary>
		/// Creates a smtp client
		/// </summary>
		/// <param name="sender">Email address of the sender</param>
		/// <param name="password">Password of the sender</param>
		/// <returns>SmtpClient</returns>
		private SmtpClient CreateSmtpClient(string sender, string password)
		{
			var client = new SmtpClient(_settings.SmtpServer, _settings.Port);

			if (_settings.Authentication)
			{
				//Authentication of the sender is required
				var basicCredential = new NetworkCredential(sender, password);
				client.UseDefaultCredentials = false;
				client.Credentials = basicCredential;
			}
			else
			{
				//Anonymous sending the mail
				client.UseDefaultCredentials = false;
				client.Credentials = null;
			}

			client.EnableSsl = _settings.SSL;

			return client;
		}

		private async Task SendMailAsync(SmtpClient client, MailMessage mailMessage, MailData mail)
		{
			var recipients = mail.Recipients.ToList();
			var recipientsCC = mail.RecipientsCC.ToList();
			var recipientsBCC = mail.RecipientsBCC.ToList();

			do
			{
				mailMessage.CC.Clear();
				mailMessage.Bcc.Clear();
				mailMessage.To.Clear();

				var availableRecipents = _settings.ChunkSize;
				recipients = AddRecipients(recipients, ref availableRecipents, recipient => mailMessage.To.Add(recipient));
				recipientsCC = AddRecipients(recipientsCC, ref availableRecipents, recipient => mailMessage.CC.Add(recipient));
				recipientsBCC = AddRecipients(recipientsBCC, ref availableRecipents, recipient => mailMessage.Bcc.Add(recipient));

				await client.SendMailAsync(mailMessage);
			}
			while (recipients.Count > 0 || recipientsCC.Count > 0 || recipientsBCC.Count > 0);
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
	}
}
