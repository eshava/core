namespace Eshava.Core.Logging.Models
{
	public class LogEntryExceptionDto
	{
		public string Message { get; set; }
		public string StackTrace { get; set; }
		public LogEntryExceptionDto InnerException { get; set; }
	}
}