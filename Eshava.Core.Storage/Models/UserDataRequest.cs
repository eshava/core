namespace Eshava.Core.Storage.Models
{
	public class UserDataRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public DatabaseConnectionOptions Server { get; set; }
	}
}