using System;
using System.Linq;
using Eshava.Core.Logging;
using Eshava.Test.Core.Logging.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Logging
{
	[TestClass, TestCategory("Core.Logging")]
	public class DataModelChangeTrackerTest
	{
		private DataModelChangeTracker<int> _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new DataModelChangeTracker<int>();
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
			logs.Should().HaveCount(4);
			var logOne = logs[0];
			var logTwo = logs[1];
			var logThree = logs[2];
			var logFour = logs[3];

			logOne.DataRecordId.Should().Be(dataRecord.Id);
			logOne.DataRecordParentId.Should().Be(2);
			logOne.DataRecordName.Should().Be(nameof(DataRecord));
			logOne.DataType.Should().Be(typeof(int));
			logOne.PropertyName.Should().Be("Id");
			logOne.Value.Should().Be(dataRecord.Id);

			logTwo.DataRecordId.Should().Be(dataRecord.Id);
			logTwo.DataRecordParentId.Should().Be(2);
			logTwo.DataRecordName.Should().Be(nameof(DataRecord));
			logTwo.DataType.Should().Be<string>();
			logTwo.PropertyName.Should().Be(nameof(DataRecord.RecordName));
			logTwo.Value.Should().Be(dataRecord.RecordName);

			logThree.DataRecordId.Should().Be(dataRecord.Id);
			logThree.DataRecordParentId.Should().Be(2);
			logThree.DataRecordName.Should().Be(nameof(DataRecord));
			logThree.DataType.Should().Be<string>();
			logThree.PropertyName.Should().Be(nameof(DataRecord.RecordVersion));
			logThree.Value.Should().Be(dataRecord.RecordVersion);

			logFour.DataRecordId.Should().Be(dataRecord.Id);
			logFour.DataRecordParentId.Should().Be(2);
			logFour.DataRecordName.Should().Be(nameof(DataRecord));
			logFour.DataType.Should().Be<DateTime>();
			logFour.PropertyName.Should().Be(nameof(DataRecord.Timestamp));
			logFour.Value.Should().Be(dataRecord.Timestamp);
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
			var logs = _classUnderTest.CreateUpdateLogs(dataRecord, dataRecordToCompare, dataRecord.Id, 2);

			// Assert
			logs.Should().HaveCount(2);
			var logOne = logs.First();
			var logTwo = logs.Last();

			logOne.DataRecordId.Should().Be(dataRecord.Id);
			logOne.DataRecordParentId.Should().Be(2);
			logOne.DataRecordName.Should().Be(nameof(DataRecord));
			logOne.DataType.Should().Be<string>();
			logOne.PropertyName.Should().Be(nameof(DataRecord.RecordName));
			logOne.Value.Should().Be(dataRecord.RecordName);

			logTwo.DataRecordId.Should().Be(dataRecord.Id);
			logTwo.DataRecordParentId.Should().Be(2);
			logTwo.DataRecordName.Should().Be(nameof(DataRecord));
			logTwo.DataType.Should().Be<string>();
			logTwo.PropertyName.Should().Be(nameof(DataRecord.RecordVersion));
			logTwo.Value.Should().Be(dataRecord.RecordVersion);
		}

		[TestMethod]
		public void CreateDeleteLogTest()
		{
			// Act
			var log = _classUnderTest.CreateDeleteLog<DataRecord>(1, 2);

			// Assert
			log.DataRecordId.Should().Be(1);
			log.DataRecordParentId.Should().Be(2);
			log.DataRecordName.Should().Be(nameof(DataRecord));
			log.DataType.Should().Be<string>();
			log.PropertyName.Should().Be("Id");
			log.Value.Should().Be(nameof(DataRecord) + " deleted");
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