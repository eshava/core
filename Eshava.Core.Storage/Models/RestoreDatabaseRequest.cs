namespace Eshava.Core.Storage.Models
{
	public class RestoreDatabaseRequest
	{
		public DatabaseConnectionOptions Server { get; set; }
		public string FilePathData { get; set; }
		public string FilePathLog { get; set; }
		public string BackupFullFileName { get; set; }
	}
}