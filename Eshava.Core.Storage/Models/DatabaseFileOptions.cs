namespace Eshava.Core.Storage.Models
{
	public class DatabaseFileOptions
	{
		public string FilePath { get; set; }
		public int Size { get; set; }
		public int MaxSize { get; set; }
		public int FileGrowth { get; set; }
	}
}