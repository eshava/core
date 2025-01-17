using System;
using System.Runtime.CompilerServices;
using Eshava.Core.Logging.Models;
using Microsoft.Extensions.Logging;

namespace Eshava.Core.Logging.Extensions
{
	public static class ILoggerExtension
	{
		public static Guid LogTrace<T>(this ILogger logger, T source, string message, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0) where T : class
		{
			return Log(logger, LogLevel.Trace, source, memberName, lineNumber, message, null, additional, messageGuid);
		}

		public static Guid LogTrace(this ILogger logger, Type sourceType, string message, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(logger, LogLevel.Trace, sourceType, memberName, lineNumber, message, null, additional, messageGuid);
		}

		public static Guid LogDebug<T>(this ILogger logger, T source, string message, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0) where T : class
		{
			return Log(logger, LogLevel.Debug, source, memberName, lineNumber, message, null, additional, messageGuid);
		}

		public static Guid LogDebug(this ILogger logger, Type sourceType, string message, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(logger, LogLevel.Debug, sourceType, memberName, lineNumber, message, null, additional, messageGuid);
		}

		public static Guid LogInformation<T>(this ILogger logger, T source, string message, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0) where T : class
		{
			return Log(logger, LogLevel.Information, source, memberName, lineNumber, message, null, additional, messageGuid);
		}

		public static Guid LogInformation(this ILogger logger, Type sourceType, string message, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(logger, LogLevel.Information, sourceType, memberName, lineNumber, message, null, additional, messageGuid);
		}

		public static Guid LogWarning<T>(this ILogger logger, T source, string message, Exception exception = null, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0) where T : class
		{
			return Log(logger, LogLevel.Warning, source, memberName, lineNumber, message, exception, additional, messageGuid);
		}

		public static Guid LogWarning(this ILogger logger, Type sourceType, string message, Exception exception = null, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(logger, LogLevel.Warning, sourceType, memberName, lineNumber, message, exception, additional, messageGuid);
		}

		public static Guid LogError<T>(this ILogger logger, T source, string message, Exception exception = null, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0) where T : class
		{
			return Log(logger, LogLevel.Error, source, memberName, lineNumber, message, exception, additional, messageGuid);
		}

		public static Guid LogError(this ILogger logger, Type sourceType, string message, Exception exception = null, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(logger, LogLevel.Error, sourceType, memberName, lineNumber, message, exception, additional, messageGuid);
		}

		public static Guid LogCritical<T>(this ILogger logger, T source, string message, Exception exception = null, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0) where T : class
		{
			return Log(logger, LogLevel.Critical, source, memberName, lineNumber, message, exception, additional, messageGuid);
		}

		public static Guid LogCritical(this ILogger logger, Type sourceType, string message, Exception exception = null, object additional = null, Guid? messageGuid = null, [CallerMemberName] string memberName = null, [CallerLineNumber] int lineNumber = 0)
		{
			return Log(logger, LogLevel.Critical, sourceType, memberName, lineNumber, message, exception, additional, messageGuid);
		}

		private static Guid Log<T>(ILogger logger, LogLevel logLevel, T source, string memberName, int lineNumber, string message, Exception exception, object additional, Guid? messageGuid) where T : class
		{
			return Log(logger, logLevel, source.GetType(), memberName, lineNumber, message, exception, additional, messageGuid);
		}

		private static Guid Log(ILogger logger, LogLevel logLevel, Type sourceType, string memberName, int lineNumber, string message, Exception exception, object additional, Guid? messageGuid)
		{
			if (!messageGuid.HasValue || messageGuid.Value == Guid.Empty)
			{
				messageGuid = Guid.NewGuid();
			}

			var additionalInformation = new LogInformationDto
			{
				Message = message,
				Class = sourceType.Name,
				Method = memberName,
				LineNumber = lineNumber,
				Information = additional,
				MessageGuid = messageGuid
			};

			logger.Log(logLevel, new EventId(0), additionalInformation, exception, (state, ex) => ex?.Message);

			return messageGuid.Value;
		}
	}
}