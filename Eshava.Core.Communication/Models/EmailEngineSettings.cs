using System.Collections.Generic;

namespace Eshava.Core.Communication.Models
{
	public class EmailEngineSettings
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public bool SSL { get; set; }
		public bool SendWithoutAuthentication { get; set; }
		public string Username { get; set; }
		public string Sender { get; set; }
		public string SenderDisplayName { get; set; }
		public string Password { get; set; }
		public int ChunkSize { get; set; }
		public List<string> DomainWhiteList { get; set; }
		public string FallbackReceiverEmailAddress { get; set; }
	}
}