namespace Eshava.Core.Storage.Models
{
	public class BackupDatabaseRequest
	{
		public DatabaseConnectionOptions Server { get; set; }
		public string BackupPath { get; set; }
	}
}