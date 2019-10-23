using System;
using Eshava.Core.Logging.Interfaces;
using Eshava.Core.Logging.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eshava.Core.Logging
{
	public class LogEngine : ILogger
	{
		public const string DEFAULT = "Default";

		private readonly string _categoryName;
		private readonly string _version;
		private readonly ILogWriter _logWriter;
		private readonly LogLevel _logLevel;

		public LogEngine(string categoryName, string version, LogLevel logLevel, ILogWriter logWriter)
		{
			_categoryName = categoryName;
			_version = version;
			_logWriter = logWriter;
			_logLevel = logLevel;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return GetLogLevelValue(_logLevel) <= GetLogLevelValue(logLevel);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TState"><see cref="AdditionalInformation"/></typeparam>
		/// <param name="logLevel"></param>
		/// <param name="eventId">Name: <see cref="LogEntry.ApplicationId"/></param>
		/// <param name="state"><see cref="LogEntry.Additional"/></param>
		/// <param name="exception"></param>
		/// <param name="formatter">Not used</param>
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}

			var additionalInformation = state as AdditionalInformation;
			var logMessage = new LogMessage
			{
				Message = additionalInformation?.Message ?? "No message",
				Class = additionalInformation?.Class,
				Method = additionalInformation?.Method
			};

			var log = new LogEntry
			{
				Host = new Maschine
				{
					HostName = Environment.MachineName,
					OperationSystem = Environment.OSVersion.VersionString,
					OperationSystem64Bit = Environment.Is64BitOperatingSystem,
					ProcessorCount = Environment.ProcessorCount,
					Culture = System.Globalization.CultureInfo.CurrentCulture.Name
				},
				Process = new Process
				{
					ProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName,
					ProcessStart = System.Diagnostics.Process.GetCurrentProcess().StartTime,
					Process64Bit = Environment.Is64BitProcess,
					MemoryUsage = $"{Environment.WorkingSet / 1024 / 1024}MB"
				},
				LogLevel = logLevel.ToString().ToLower(),
				ApplicationId = eventId.Name,
				Version = _version,
				Category = _categoryName,
				Message = logMessage,
				Additional = additionalInformation?.Information == null ? null : JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(additionalInformation?.Information)),
				Exception = ConvertException(exception),
				Timestamp = DateTime.UtcNow
			};

			_logWriter.Write(log);
		}

		private LogEntryException ConvertException(Exception exception)
		{
			if (exception == null)
			{
				return null;
			}

			return new LogEntryException
			{
				Message = exception.Message,
				StackTrace = exception.StackTrace,
				InnerException = ConvertException(exception.InnerException)
			};
		}

		private int GetLogLevelValue(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Trace:
					return 1;
				case LogLevel.Debug:
					return 2;
				case LogLevel.Information:
					return 3;
				case LogLevel.Warning:
					return 4;
				case LogLevel.Error:
					return 5;
				case LogLevel.Critical:
					return 6;
				default:
					return 7;
			}
		}
	}
}