using System;
using Eshava.Core.Logging;
using Eshava.Core.Logging.Interfaces;
using Eshava.Core.Logging.Models;
using Eshava.Test.Core.Logging.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eshava.Test.Core.Logging
{
	[TestClass, TestCategory("Core.Logging")]
	public class LogEngineTest
	{
		private LogEngine _classUnderTest;
		private ILogWriter _logWriterFake;

		[TestInitialize]
		public void Setup()
		{
			_logWriterFake = A.Fake<ILogWriter>();
			_classUnderTest = new LogEngine("DarkwingDuck", LogLevel.Error, _logWriterFake);
		}

		[DataTestMethod]
		[DataRow(LogLevel.Trace, false)]
		[DataRow(LogLevel.Debug, false)]
		[DataRow(LogLevel.Information, false)]
		[DataRow(LogLevel.Warning, false)]
		[DataRow(LogLevel.Error, true)]
		[DataRow(LogLevel.Critical, true)]
		public void IsEnabledTest(LogLevel logLevel, bool isEnabled)
		{
			// Act
			var result = _classUnderTest.IsEnabled(logLevel);

			// Assert
			result.Should().Be(isEnabled);
		}


		[TestMethod]
		public void LogTest()
		{
			// Arrange
			var eventId = new EventId(0, "MegaVolt");
			var alpha = new Alpha
			{
				Beta = 123,
				Gamma = "Super Hero",
			};
			var additionalInformation = new AdditionalInformation
			{
				Class = "Villain",
				Method = "Attack",
				Message = "DarkwingDuck is late",
				Information = alpha
			};
			var exception = new Exception("MegaVolt is here!", new NotSupportedException("Overload"));


			var logEntry = default(LogEntry);
			A.CallTo(() => _logWriterFake.Write(A<LogEntry>.Ignored)).Invokes(fakeCallObject =>
			{
				logEntry = fakeCallObject.Arguments[0] as LogEntry;
			});

			// Act
			_classUnderTest.Log(LogLevel.Error, eventId, additionalInformation, exception, null);

			// Assert
			logEntry.Host.HostName.Should().Be(Environment.MachineName);
			logEntry.Host.OperationSystem.Should().Be(Environment.OSVersion.VersionString);
			logEntry.Host.OperationSystem64Bit.Should().Be(Environment.Is64BitOperatingSystem);
			logEntry.Host.ProcessorCount.Should().Be(Environment.ProcessorCount);
			logEntry.Host.Culture.Should().Be(System.Globalization.CultureInfo.CurrentCulture.Name);

			logEntry.Process.ProcessName.Should().Be(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
			logEntry.Process.ProcessStart.Should().Be(System.Diagnostics.Process.GetCurrentProcess().StartTime);
			logEntry.Process.Process64Bit.Should().Be(Environment.Is64BitProcess);
			logEntry.Process.MemoryUsage.Should().EndWith("MB");

			logEntry.LogLevel.Should().Be(LogLevel.Error.ToString().ToLower());
			logEntry.ApplicationId.Should().Be("MegaVolt");
			logEntry.Category.Should().Be("DarkwingDuck");
			logEntry.Version.Should().BeNull();

			logEntry.Message.Message.Should().Be(additionalInformation.Message);
			logEntry.Message.Class.Should().Be(additionalInformation.Class);
			logEntry.Message.Method.Should().Be(additionalInformation.Method);
			logEntry.Additional.Should().BeEquivalentTo(JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(alpha)));

			logEntry.Exception.Message.Should().Be(exception.Message);
			logEntry.Exception.StackTrace.Should().Be(exception.StackTrace);
			logEntry.Exception.InnerException.Message.Should().Be(exception.InnerException.Message);
			logEntry.Exception.InnerException.StackTrace.Should().Be(exception.InnerException.StackTrace);
			logEntry.Timestamp.Should().NotBe(DateTime.MinValue);
			logEntry.Timestamp.Should().NotBe(DateTime.MaxValue);
			logEntry.Timestamp.Kind.Should().Be(DateTimeKind.Utc);
		}

		[TestMethod]
		public void LogNoAdditionalInformationTest()
		{
			// Arrange
			var eventId = new EventId(0, "MegaVolt");

			var logEntry = default(LogEntry);
			A.CallTo(() => _logWriterFake.Write(A<LogEntry>.Ignored)).Invokes(fakeCallObject =>
			{
				logEntry = fakeCallObject.Arguments[0] as LogEntry;
			});

			// Act
			_classUnderTest.Log(LogLevel.Error, eventId, null, null, null);

			// Assert
			logEntry.Message.Message.Should().Be("No message");
			logEntry.Message.Class.Should().BeNull();
			logEntry.Message.Method.Should().BeNull();
			logEntry.Additional.Should().BeNull();
		}

		[TestMethod]
		public void LogNoAdditionalInformationInformationTest()
		{
			// Arrange
			var eventId = new EventId(0, "MegaVolt");
			var additionalInformation = new AdditionalInformation
			{
				Class = "Villain",
				Method = "Attack",
				Message = "DarkwingDuck is late",
				Information = null
			};
			
			var logEntry = default(LogEntry);
			A.CallTo(() => _logWriterFake.Write(A<LogEntry>.Ignored)).Invokes(fakeCallObject =>
			{
				logEntry = fakeCallObject.Arguments[0] as LogEntry;
			});

			// Act
			_classUnderTest.Log(LogLevel.Error, eventId, additionalInformation, null, null);

			// Assert
			logEntry.Message.Message.Should().Be(additionalInformation.Message);
			logEntry.Message.Class.Should().Be(additionalInformation.Class);
			logEntry.Message.Method.Should().Be(additionalInformation.Method);
			logEntry.Additional.Should().BeNull();
		}

		[TestMethod]
		public void LogNoExceptionTest()
		{
			// Arrange
			var eventId = new EventId(0, "MegaVolt");
			var alpha = new Alpha
			{
				Beta = 123,
				Gamma = "Super Hero",
			};
			var additionalInformation = new AdditionalInformation
			{
				Class = "Villain",
				Method = "Attack",
				Message = "DarkwingDuck is late",
				Information = alpha
			};
			
			var logEntry = default(LogEntry);
			A.CallTo(() => _logWriterFake.Write(A<LogEntry>.Ignored)).Invokes(fakeCallObject =>
			{
				logEntry = fakeCallObject.Arguments[0] as LogEntry;
			});

			// Act
			_classUnderTest.Log(LogLevel.Error, eventId, additionalInformation, null, null);

			// Assert
			logEntry.Exception.Should().BeNull();
		}
	}
}