namespace Eshava.Core.Logging.Models
{
	public class LogMessage
	{
		public string Class { get; set; }
		public string Method { get; set; }
		public string Message { get; set; }

		public override string ToString()
		{
			return $"[{Class}]#[{Method}]: {Message}";
		}
	}
}