using System;

namespace Eshava.Core.Logging.Models
{
	public class Process
	{
		public bool Process64Bit { get; set; }
		public string ProcessName { get; set; }
		public DateTime ProcessStart { get; set; }
		public string MemoryUsage { get; set; }
	}
}