using System;

namespace Eshava.Core.Logging.Models
{
	public class LogInformationDto
	{
		public string Class { get; set; }
		public string Method { get; set; }
		public int LineNumber { get; set; }
		public string Message { get; set; }
		public object Information { get; set; }
		public Guid? MessageGuid { get; set; }
	}
}