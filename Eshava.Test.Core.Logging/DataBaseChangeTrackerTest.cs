using System;
using System.Linq;
using Eshava.Core.Logging;
using Eshava.Test.Core.Logging.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Logging
{
	[TestClass, TestCategory("Core.Logging")]
	public class DatabaseChangeTrackerTest
	{
		private DatabaseChangeTracker<int> _classUnderTest;
		
		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new DatabaseChangeTracker<int>();
		}

		[TestMethod]
		public void CreateInsertLogsTest()
		{
			// Arrange
			var dataRecord = new DataRecord
			{
				Id = 1,
				RecordName = "Darkwing Duck",
				RecordValue = "MegaVolt",
				RecordVersion = "1.0.0.0",
				Timestamp = DateTime.UtcNow.Date
			};

			// Act
			var logs = _classUnderTest.CreateInsertLogs(dataRecord, dataRecord.Id, 2).ToList();

			// Assert
			logs.Should().HaveCount(3);
			var logOne = logs[0];
			var logTwo = logs[1];
			var logThree = logs[2];

			logOne.DataRecordId.Should().Be(dataRecord.Id);
			logOne.DataRecordParentId.Should().Be(2);
			logOne.DataRecordName.Should().Be("DataRecordTable");
			logOne.DataType.Should().Be(typeof(int));
			logOne.PropertyName.Should().Be("RecordIdColumn");
			logOne.Value.Should().Be(dataRecord.Id);

			logTwo.DataRecordId.Should().Be(dataRecord.Id);
			logTwo.DataRecordParentId.Should().Be(2);
			logTwo.DataRecordName.Should().Be("DataRecordTable");
			logTwo.DataType.Should().Be<string>();
			logTwo.PropertyName.Should().Be("RecordNameColumn");
			logTwo.Value.Should().Be(dataRecord.RecordName);

			logThree.DataRecordId.Should().Be(dataRecord.Id);
			logThree.DataRecordParentId.Should().Be(2);
			logThree.DataRecordName.Should().Be("DataRecordTable");
			logThree.DataType.Should().Be<DateTime>();
			logThree.PropertyName.Should().Be("TimestampColumn");
			logThree.Value.Should().Be(dataRecord.Timestamp);
		}

		[TestMethod]
		public void CreateUpdateLogsTest()
		{
			// Arrange
			var dataRecord = new DataRecord
			{
				Id = 1,
				RecordName = "Darkwing Duck",
				RecordValue = "MegaVolt",
				RecordVersion = "1.0.0.0"
			};

			var dataRecordToCompare = new DataRecord
			{
				Id = 1,
				RecordName = "Darkwing Duck !!!",
				RecordValue = "MegaVolt",
				RecordVersion = "1.0.0.1"
			};

			// Act
			var logs = _classUnderTest.CreateUpdateLogs(dataRecord, dataRecordToCompare,  dataRecord.Id, 2);

			// Assert
			logs.Should().HaveCount(1);
			var log = logs.Single();

			log.DataRecordId.Should().Be(dataRecord.Id);
			log.DataRecordParentId.Should().Be(2);
			log.DataRecordName.Should().Be("DataRecordTable");
			log.DataType.Should().Be<string>();
			log.PropertyName.Should().Be("RecordNameColumn");
			log.Value.Should().Be(dataRecord.RecordName);
		}

		[TestMethod]
		public void CreateDeleteLogTest()
		{
			// Act
			var log = _classUnderTest.CreateDeleteLog<DataRecord>(1, 2);

			// Assert
			log.DataRecordId.Should().Be(1);
			log.DataRecordParentId.Should().Be(2);
			log.DataRecordName.Should().Be("DataRecordTable");
			log.DataType.Should().Be<string>();
			log.PropertyName.Should().Be("RecordIdColumn");
			log.Value.Should().Be(nameof(DataRecord) + " deleted");
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void CreateInsertLogsNoTableAttributeTest()
		{
			// Act
			_classUnderTest.CreateInsertLogs(new Alpha(), 1, 2);
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void CreateUpdateLogsNoTableAttributeTest()
		{
			// Act
			_classUnderTest.CreateUpdateLogs(new Alpha(), new Alpha(), 1, 2);
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void CreateDeleteLogNoTableAttributeTest()
		{
			// Act
			_classUnderTest.CreateDeleteLog<Alpha>(1, 2);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateInsertLogsDataRecordIsNullTest()
		{
			// Act
			_classUnderTest.CreateInsertLogs<DataRecord>(null, 1, 2);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateUpdateLogsDataRecordIsNullTest()
		{
			// Act
			_classUnderTest.CreateUpdateLogs(null, new DataRecord(), 1, 2);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateUpdateLogsDataRecordToCompareIsNullTest()
		{
			// Act
			_classUnderTest.CreateUpdateLogs(new DataRecord(), null, 1, 2);
		}
	}
}