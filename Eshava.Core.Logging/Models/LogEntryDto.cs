using System;
using Newtonsoft.Json.Linq;

namespace Eshava.Core.Logging.Models
{
	public class LogEntryDto
	{
		public DateTime TimestampUtc { get; set; }
		public Maschine Host { get; set; }
		public Process Process { get; set; }
		public string LogLevel { get; set; }
		public string Version { get; set; }
		public string Source { get; set; }
		public string Category { get; set; }
		public LogEntryExceptionDto Exception { get; set; }		
		public JToken Details { get; set; }
	}
}