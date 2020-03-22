namespace Eshava.Core.Storage.Models
{
	public class CreateDatabaseRequest
	{
		public DatabaseFileOptions DataFileOptions { get; set; }
		public DatabaseFileOptions LogFileOptions { get; set; }
		public DatabaseConnectionOptions Server { get; set; }
	}
}