using System;
using Newtonsoft.Json.Linq;

namespace Eshava.Core.Logging.Models
{
	public class LogEntry
	{
		public DateTime TimestampUtc { get; set; }
		public Maschine Host { get; set; }
		public Process Process { get; set; }
		public string ApplicationId { get; set; }
		public string LogLevel { get; set; }
		public string Version { get; set; }
		public string Source { get; set; }
		public string Category { get; set; }
		public LogMessage Message { get; set; }
		public LogEntryException Exception { get; set; }
		public JToken Additional { get; set; }
	}
}