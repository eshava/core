using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Eshava.Core.Logging.Models
{
	public class LogSettings
	{
		public LogSettings()
		{
			LogLevel = new Dictionary<string, LogLevel>();
			IgnoredCategories = new List<string>();
		}

		public bool IncludeScopes { get; set; }

		public Dictionary<string, LogLevel> LogLevel { get; set; }
		public List<string> IgnoredCategories { get; set; }
	}
}