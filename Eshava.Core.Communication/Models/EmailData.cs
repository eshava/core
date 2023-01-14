using System.Collections.Generic;

namespace Eshava.Core.Communication.Models
{
	public class EmailData
	{
		public EmailData()
		{
			Recipients = new List<string>();
			RecipientsCC = new List<string>();
			RecipientsBCC = new List<string>();
			LinkedResources = new List<EmailAttachmentData>();
			Attachments = new List<EmailAttachmentData>();
		}

		public string Sender { get; set; }
		public string SenderDisplayName { get; set; }

		public IEnumerable<string> Recipients { get; set; }

		public IEnumerable<string> RecipientsCC { get; set; }

		public IEnumerable<string> RecipientsBCC { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }

		public bool IsHtml { get; set; }

		public IEnumerable<EmailAttachmentData> LinkedResources { get; set; }
		public IEnumerable<EmailAttachmentData> Attachments { get; set; }
	}
}