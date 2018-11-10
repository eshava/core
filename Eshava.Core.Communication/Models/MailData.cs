using System.Collections.Generic;

namespace Eshava.Core.Communication.Models
{
	public class MailData
	{
		public MailData()
		{
			Recipients = new List<string>();
			RecipientsCC = new List<string>();
			RecipientsBCC = new List<string>();
		}

		public IEnumerable<string> Recipients { get; set; }

		public IEnumerable<string> RecipientsCC { get; set; }

		public IEnumerable<string> RecipientsBCC { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }

		public bool Html { get; set; }
	}
}