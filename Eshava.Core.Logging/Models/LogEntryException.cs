namespace Eshava.Core.Logging.Models
{
	public class LogEntryException
	{
		public string Message { get; set; }
		public string StackTrace { get; set; }
		public LogEntryException InnerException { get; set; }
	}
}