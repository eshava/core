using System;
using System.Runtime.CompilerServices;
using Eshava.Core.Logging.Models;
using Microsoft.Extensions.Logging;

namespace Eshava.Core.Logging.Extensions
{
	public static class ILoggerExtension
	{
		public static void LogTrace<T>(this ILogger logger, T source, string applicationId, string message, object additional = null, [CallerMemberName] string memberName = null) where T : class
		{
			Log(logger, LogLevel.Trace, source, memberName, applicationId, message, null, additional);
		}

		public static void LogTrace(this ILogger logger, Type sourceType, string applicationId, string message, object additional = null, [CallerMemberName] string memberName = null)
		{
			Log(logger, LogLevel.Trace, sourceType, memberName, applicationId, message, null, additional);
		}

		public static void LogDebug<T>(this ILogger logger, T source, string applicationId, string message, object additional = null, [CallerMemberName] string memberName = null) where T : class
		{
			Log(logger, LogLevel.Debug, source, memberName, applicationId, message, null, additional);
		}

		public static void LogDebug(this ILogger logger, Type sourceType, string applicationId, string message, object additional = null, [CallerMemberName] string memberName = null)
		{
			Log(logger, LogLevel.Debug, sourceType, memberName, applicationId, message, null, additional);
		}

		public static void LogInformation<T>(this ILogger logger, T source, string applicationId, string message, object additional = null, [CallerMemberName] string memberName = null) where T : class
		{
			Log(logger, LogLevel.Information, source, memberName, applicationId, message, null, additional);
		}

		public static void LogInformation(this ILogger logger, Type sourceType, string applicationId, string message, object additional = null, [CallerMemberName] string memberName = null)
		{
			Log(logger, LogLevel.Information, sourceType, memberName, applicationId, message, null, additional);
		}

		public static void LogWarning<T>(this ILogger logger, T source, string applicationId, string message, Exception exception = null, object additional = null, [CallerMemberName] string memberName = null) where T : class
		{
			Log(logger, LogLevel.Warning, source, memberName, applicationId, message, exception, additional);
		}

		public static void LogWarning(this ILogger logger, Type sourceType, string applicationId, string message, Exception exception = null, object additional = null, [CallerMemberName] string memberName = null)
		{
			Log(logger, LogLevel.Warning, sourceType, memberName, applicationId, message, exception, additional);
		}

		public static void LogError<T>(this ILogger logger, T source, string applicationId, string message, Exception exception = null, object additional = null, [CallerMemberName] string memberName = null) where T : class
		{
			Log(logger, LogLevel.Error, source, memberName, applicationId, message, exception, additional);
		}

		public static void LogError(this ILogger logger, Type sourceType, string applicationId, string message, Exception exception = null, object additional = null, [CallerMemberName] string memberName = null)
		{
			Log(logger, LogLevel.Error, sourceType, memberName, applicationId, message, exception, additional);
		}

		public static void LogCritical<T>(this ILogger logger, T source, string applicationId, string message, Exception exception = null, object additional = null, [CallerMemberName] string memberName = null) where T : class
		{
			Log(logger, LogLevel.Critical, source, memberName, applicationId, message, exception, additional);
		}

		public static void LogCritical(this ILogger logger, Type sourceType, string applicationId, string message, Exception exception = null, object additional = null, [CallerMemberName] string memberName = null)
		{
			Log(logger, LogLevel.Critical, sourceType, memberName, applicationId, message, exception, additional);
		}

		private static void Log<T>(ILogger logger, LogLevel logLevel, T source, string memberName, string applicationId, string message, Exception exception, object additional) where T : class
		{
			Log(logger, logLevel, source.GetType(), memberName, applicationId, message, exception, additional);
		}

		private static void Log(ILogger logger, LogLevel logLevel, Type sourceType, string memberName, string applicationId, string message, Exception exception, object additional)
		{
			var additionalInformation = new AdditionalInformation
			{
				Message = message,
				Class = sourceType.Name,
				Method = memberName,
				Information = additional
			};

			logger.Log(logLevel, new EventId(0, applicationId), additionalInformation, exception, (state, ex) => ex?.Message);
		}
	}
}