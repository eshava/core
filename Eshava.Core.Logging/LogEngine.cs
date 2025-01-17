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
		private JsonSerializerSettings _jsonSerializerSettings;
		private readonly string _categoryName;
		private readonly string _version;
		private readonly ILogWriter _logWriter;
		private readonly LogLevel _logLevel;

		public LogEngine(string categoryName, string version, LogLevel logLevel, ILogWriter logWriter, ReferenceLoopHandling referenceLoopHandling)
		{
			_categoryName = categoryName;
			_version = version;
			_logWriter = logWriter;
			_logLevel = logLevel;
			_jsonSerializerSettings = new() { ReferenceLoopHandling = referenceLoopHandling };
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return (int)_logLevel <= (int)logLevel;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TState"><see cref="LogInformationDto"/></typeparam>
		/// <param name="logLevel"></param>
		/// <param name="eventId">Name: <see cref="LogEntryDto.ApplicationId"/></param>
		/// <param name="state"><see cref="LogEntryDto.Additional"/></param>
		/// <param name="exception"></param>
		/// <param name="formatter">Not used</param>
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}

			var logEntry = new LogEntryDto
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
					ProcessStartUtc = TimeZoneInfo.ConvertTimeToUtc(System.Diagnostics.Process.GetCurrentProcess().StartTime),
					Process64Bit = Environment.Is64BitProcess,
					MemoryUsage = $"{Environment.WorkingSet / 1024 / 1024}MB"
				},
				LogLevel = logLevel.ToString().ToLower(),
				Category = _categoryName,
				Details = state == null
					? null
					: JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(state, _jsonSerializerSettings)),
				Exception = ConvertException(exception),
				TimestampUtc = DateTime.UtcNow,
				Version = _version
			};

			_logWriter.Write(logEntry);
		}

		private LogEntryExceptionDto ConvertException(Exception exception)
		{
			if (exception == null)
			{
				return null;
			}

			return new LogEntryExceptionDto
			{
				Message = exception.Message,
				StackTrace = exception.StackTrace,
				InnerException = ConvertException(exception.InnerException)
			};
		}
	}
}