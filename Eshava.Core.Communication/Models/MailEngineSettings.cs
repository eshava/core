namespace Eshava.Core.Communication.Models
{
	public class MailEngineSettings
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public bool SSL { get; set; }
		public bool Authentication { get; set; }
		public string Sender { get; set; }
		public string Password { get; set; }
		public int ChunkSize { get; set; }
	}
}