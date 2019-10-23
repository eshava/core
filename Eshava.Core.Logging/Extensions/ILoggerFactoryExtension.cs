using Eshava.Core.Logging.Interfaces;
using Eshava.Core.Logging.Models;
using Microsoft.Extensions.Logging;

namespace Eshava.Core.Logging.Extensions
{
	public static class ILoggerFactoryExtension
	{
		public static ILoggerFactory AddProvider(this ILoggerFactory loggerFactory, LogSettings logSettings, string version, ILogWriter logWriter)
		{
			loggerFactory.AddProvider(new LoggerProvider(logSettings, version, logWriter));

			return loggerFactory;
		}
	}
}