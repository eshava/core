namespace Eshava.Core.Logging.Models
{
	public class Maschine
	{
		public string HostName { get; set; }
		public string OperationSystem { get; set; }
		public bool OperationSystem64Bit { get; set; }
		public int ProcessorCount { get; set; }
		public string Culture { get; set; }
	}
}