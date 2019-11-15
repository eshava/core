using System.Collections.Generic;
using Eshava.Core.Logging;
using Eshava.Core.Logging.Interfaces;
using Eshava.Core.Logging.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Logging
{
	[TestClass, TestCategory("Core.Logging")]
	public class LoggerProviderTest
	{
		private LoggerProvider _classUnderTest;
		private ILogWriter _logWriterFake;

		[TestInitialize]
		public void Setup()
		{
			var logSettings = new LogSettings
			{
				LogLevel = new Dictionary<string, LogLevel>
				{
					{ "DarkwingDuck", LogLevel.Warning },
					{ "LaunchpadMcQuack", LogLevel.Trace },
				},
				IgnoredCategories = new List<string>
				{
					"MegaVolt"
				}
			};

			_logWriterFake = A.Fake<ILogWriter>();
			_classUnderTest = new LoggerProvider(logSettings, "1.0.0.0", _logWriterFake);
		}

		[TestMethod]
		public void CreateLoggerLogLevelTest()
		{
			// Arrange
			var categoryName = "DarkwingDuck";

			// Act
			var logger = _classUnderTest.CreateLogger(categoryName);

			// Assert
			logger.IsEnabled(LogLevel.Trace).Should().BeFalse();
			logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
			logger.IsEnabled(LogLevel.Information).Should().BeFalse();
			logger.IsEnabled(LogLevel.Warning).Should().BeTrue();
			logger.IsEnabled(LogLevel.Error).Should().BeTrue();
			logger.IsEnabled(LogLevel.Critical).Should().BeTrue();
		}

		[TestMethod]
		public void CreateLoggerSameLogEngineTest()
		{
			// Arrange
			var categoryName = "DarkwingDuck";

			// Act
			var loggerOne = _classUnderTest.CreateLogger(categoryName);
			var loggerTwo = _classUnderTest.CreateLogger(categoryName);

			// Assert
			loggerOne.Should().Be(loggerTwo);
		}

		[TestMethod]
		public void CreateLoggerDifferentLogEngineTest()
		{
			// Arrange
			var categoryNameOne = "DarkwingDuck";
			var categoryNameTwo = "LaunchpadMcQuack";

			// Act
			var loggerOne = _classUnderTest.CreateLogger(categoryNameOne);
			var loggerTwo = _classUnderTest.CreateLogger(categoryNameTwo);

			// Assert
			loggerOne.Should().NotBe(loggerTwo);
		}

		[TestMethod]
		public void DisposeTest()
		{
			// Arrange
			var categoryName = "DarkwingDuck";

			// Act
			var loggerOne = _classUnderTest.CreateLogger(categoryName);

			_classUnderTest.Dispose();

			var loggerTwo = _classUnderTest.CreateLogger(categoryName);

			// Assert
			loggerOne.Should().NotBe(loggerTwo);
		}

		[TestMethod]
		public void CreateGarbageLoggerLogLevelTest()
		{
			// Arrange
			var categoryName = "megaVolt";

			// Act
			var logger = _classUnderTest.CreateLogger(categoryName);

			// Assert
			logger.IsEnabled(LogLevel.Trace).Should().BeFalse();
			logger.IsEnabled(LogLevel.Debug).Should().BeFalse();
			logger.IsEnabled(LogLevel.Information).Should().BeFalse();
			logger.IsEnabled(LogLevel.Warning).Should().BeFalse();
			logger.IsEnabled(LogLevel.Error).Should().BeFalse();
			logger.IsEnabled(LogLevel.Critical).Should().BeFalse();
			logger.IsEnabled(LogLevel.None).Should().BeTrue();
		}
	}
}