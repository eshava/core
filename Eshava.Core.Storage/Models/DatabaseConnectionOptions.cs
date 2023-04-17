namespace Eshava.Core.Storage.Models
{
	public class DatabaseConnectionOptions
	{
		public int CommandTimeOut { get; set; }
		public string ServerInstance { get; set; }
		public string DatabaseName { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool IntegratedSecurity { get; set; }
		public bool TrustServerCertificate { get; set; }
	}
}