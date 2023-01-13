namespace Eshava.Core.Communication.Models
{
	public class EmailAccount
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public bool SSL { get; set; }
		public bool SendWithoutAuthentication { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int ChunkSize { get; set; }
	}
}