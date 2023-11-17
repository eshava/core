namespace Eshava.Core.Storage.Models
{
	public class CopyDatabaseRequest
	{
		public DatabaseConnectionOptions Server { get; set; }
		public string DatabaseNameSource { get; set; }
		public string DatabaseNameTarget { get; set; }
		public string BackupPath { get; set; }
		public string TargetFilePathData { get; set; }
		public string TargetFilePathLog { get; set; }
	}
}