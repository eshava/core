using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Eshava.Core.Extensions;
using Eshava.Core.Logging;
using Eshava.Core.Logging.Enums;
using Eshava.Core.Logging.Models;
using Eshava.Test.Core.Logging.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Logging
{
	[TestClass, TestCategory("Core.Logging")]
	public class DataRecordLogEngineTest
	{
		private DataRecordLogEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			var settings = new DataRecordLogSettings
			{
				EnumAsString = false,
				UnsupportedDataTypeAction = UnsupportedDataTypeAction.Ignore
			};

			_classUnderTest = new DataRecordLogEngine(settings);
		}

		[TestMethod]
		public void ConvertTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Gamma),
					DataRecordName = nameof(Alpha),
					DataType = typeof(string),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = "DarkwingDuck",
					TimestampUtc = DateTime.Today
				},
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Eta),
					DataRecordName = nameof(Alpha),
					DataType = typeof(decimal),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = 10.5m,
					TimestampUtc = DateTime.Today
				},
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Update,
					DataRecordId = 2,
					DataRecordParentId = 22,
					PropertyName = nameof(AdditionalInformation.Message),
					DataRecordName = nameof(AdditionalInformation),
					DataType = typeof(string),
					LogEntryGroupId = 777,
					UserId = 919,
					Value = "222",
					TimestampUtc = DateTime.Today.AddDays(-1)
				},
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Update,
					DataRecordId = 2,
					DataRecordParentId = 22,
					PropertyName = nameof(AdditionalInformation.Class),
					DataRecordName = nameof(AdditionalInformation),
					DataType = typeof(string),
					LogEntryGroupId = 777,
					UserId = 919,
					Value = "MegaVolt",
					TimestampUtc = DateTime.Today.AddDays(-1)
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(2);
			var logForInsert = logs.First();
			var logForUpdate = logs.Last();

			logForInsert.Action.Should().Be(DataRecordAction.Insert);
			logForInsert.DataRecordId.Should().Be(1);
			logForInsert.DataRecordParentId.Should().Be(11);
			logForInsert.DataRecordName.Should().Be(nameof(Alpha));
			logForInsert.LogEntryGroupId.Should().Be(888);
			logForInsert.UserId.Should().Be(999);
			logForInsert.TimestampUtc.Should().Be(DateTime.Today);
			logForInsert.Properties[nameof(Alpha.Eta)].Value.Should().Be("10.5");
			logForInsert.Properties[nameof(Alpha.Eta)].Type.Should().Be(FieldValueType.Decimal);
			logForInsert.Properties[nameof(Alpha.Gamma)].Value.Should().Be("DarkwingDuck");
			logForInsert.Properties[nameof(Alpha.Gamma)].Type.Should().Be(FieldValueType.String);

			logForUpdate.Action.Should().Be(DataRecordAction.Update);
			logForUpdate.DataRecordId.Should().Be(2);
			logForUpdate.DataRecordParentId.Should().Be(22);
			logForUpdate.DataRecordName.Should().Be(nameof(AdditionalInformation));
			logForUpdate.LogEntryGroupId.Should().Be(777);
			logForUpdate.UserId.Should().Be(919);
			logForUpdate.TimestampUtc.Should().Be(DateTime.Today.AddDays(-1));
			logForUpdate.Properties[nameof(AdditionalInformation.Message)].Value.Should().Be("222");
			logForUpdate.Properties[nameof(AdditionalInformation.Message)].Type.Should().Be(FieldValueType.String);
			logForUpdate.Properties[nameof(AdditionalInformation.Class)].Value.Should().Be("MegaVolt");
			logForUpdate.Properties[nameof(AdditionalInformation.Class)].Type.Should().Be(FieldValueType.String);
		}

		[TestMethod]
		public void ConvertWithNullValueTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Gamma),
					DataRecordName = nameof(Alpha),
					DataType = typeof(string),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = null,
					TimestampUtc = DateTime.Today
				},
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Eta),
					DataRecordName = nameof(Alpha),
					DataType = typeof(decimal),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = 10.5m,
					TimestampUtc = DateTime.Today
				},
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Update,
					DataRecordId = 2,
					DataRecordParentId = 22,
					PropertyName = nameof(AdditionalInformation.Message),
					DataRecordName = nameof(AdditionalInformation),
					DataType = typeof(string),
					LogEntryGroupId = 777,
					UserId = 919,
					Value = null,
					TimestampUtc = DateTime.Today.AddDays(-1)
				},
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Update,
					DataRecordId = 2,
					DataRecordParentId = 22,
					PropertyName = nameof(AdditionalInformation.Class),
					DataRecordName = nameof(AdditionalInformation),
					DataType = typeof(string),
					LogEntryGroupId = 777,
					UserId = 919,
					Value = "MegaVolt",
					TimestampUtc = DateTime.Today.AddDays(-1)
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(2);
			var logForInsert = logs.First();
			var logForUpdate = logs.Last();

			logForInsert.Action.Should().Be(DataRecordAction.Insert);
			logForInsert.DataRecordId.Should().Be(1);
			logForInsert.DataRecordParentId.Should().Be(11);
			logForInsert.DataRecordName.Should().Be(nameof(Alpha));
			logForInsert.LogEntryGroupId.Should().Be(888);
			logForInsert.UserId.Should().Be(999);
			logForInsert.TimestampUtc.Should().Be(DateTime.Today);
			logForInsert.Properties[nameof(Alpha.Eta)].Value.Should().Be("10.5");
			logForInsert.Properties[nameof(Alpha.Eta)].Type.Should().Be(FieldValueType.Decimal);
			logForInsert.Properties[nameof(Alpha.Gamma)].Value.Should().BeNull();
			logForInsert.Properties[nameof(Alpha.Gamma)].Type.Should().Be(FieldValueType.String);

			logForUpdate.Action.Should().Be(DataRecordAction.Update);
			logForUpdate.DataRecordId.Should().Be(2);
			logForUpdate.DataRecordParentId.Should().Be(22);
			logForUpdate.DataRecordName.Should().Be(nameof(AdditionalInformation));
			logForUpdate.LogEntryGroupId.Should().Be(777);
			logForUpdate.UserId.Should().Be(919);
			logForUpdate.TimestampUtc.Should().Be(DateTime.Today.AddDays(-1));
			logForUpdate.Properties[nameof(AdditionalInformation.Message)].Value.Should().BeNull();
			logForUpdate.Properties[nameof(AdditionalInformation.Message)].Type.Should().Be(FieldValueType.String);
			logForUpdate.Properties[nameof(AdditionalInformation.Class)].Value.Should().Be("MegaVolt");
			logForUpdate.Properties[nameof(AdditionalInformation.Class)].Type.Should().Be(FieldValueType.String);
		}

		[TestMethod]
		public void ConvertTwicePropertyTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Gamma),
					DataRecordName = nameof(Alpha),
					DataType = typeof(string),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = "MegaVolt",
					TimestampUtc = DateTime.Today
				},
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Gamma),
					DataRecordName = nameof(Alpha),
					DataType = typeof(string),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = "DarkwingDuck",
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);
			var logForInsert = logs.First();

			logForInsert.Action.Should().Be(DataRecordAction.Insert);
			logForInsert.DataRecordId.Should().Be(1);
			logForInsert.DataRecordParentId.Should().Be(11);
			logForInsert.DataRecordName.Should().Be(nameof(Alpha));
			logForInsert.LogEntryGroupId.Should().Be(888);
			logForInsert.UserId.Should().Be(999);
			logForInsert.TimestampUtc.Should().Be(DateTime.Today);
			logForInsert.Properties[nameof(Alpha.Gamma)].Value.Should().Be("DarkwingDuck");
			logForInsert.Properties[nameof(Alpha.Gamma)].Type.Should().Be(FieldValueType.String);
		}

		[TestMethod]
		public void ConvertIgnoreNotSupportedDataTypeTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Epsilon),
					DataRecordName = nameof(Alpha),
					DataType = typeof(AdditionalInformation),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = new AdditionalInformation { Message = "MegaVolt" },
					TimestampUtc = DateTime.Today
				},
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Gamma),
					DataRecordName = nameof(Alpha),
					DataType = typeof(string),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = "DarkwingDuck",
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);
			var logForInsert = logs.First();

			logForInsert.Action.Should().Be(DataRecordAction.Insert);
			logForInsert.DataRecordId.Should().Be(1);
			logForInsert.DataRecordParentId.Should().Be(11);
			logForInsert.DataRecordName.Should().Be(nameof(Alpha));
			logForInsert.LogEntryGroupId.Should().Be(888);
			logForInsert.UserId.Should().Be(999);
			logForInsert.TimestampUtc.Should().Be(DateTime.Today);
			logForInsert.Properties[nameof(Alpha.Gamma)].Value.Should().Be("DarkwingDuck");
			logForInsert.Properties[nameof(Alpha.Gamma)].Type.Should().Be(FieldValueType.String);
			logForInsert.Properties.ContainsKey(nameof(Alpha.Epsilon)).Should().BeFalse();
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void ConvertThrowExceptionForNotSupportedDataTypeTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Epsilon),
					DataRecordName = nameof(Alpha),
					DataType = typeof(AdditionalInformation),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = new AdditionalInformation { Message = "MegaVolt" },
					TimestampUtc = DateTime.Today
				}
			};

			var settings = new DataRecordLogSettings
			{
				EnumAsString = true,
				UnsupportedDataTypeAction = UnsupportedDataTypeAction.ThrowExeception
			};

			var classUnderTest = new DataRecordLogEngine(settings);

			// Act
			classUnderTest.Convert(properties);
		}

		[TestMethod]
		public void ConvertTreadEnumAsIntegerTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Delta),
					DataRecordName = nameof(Alpha),
					DataType = typeof(LogLevel),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = LogLevel.Error,
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);
			var logForInsert = logs.First();

			logForInsert.Action.Should().Be(DataRecordAction.Insert);
			logForInsert.DataRecordId.Should().Be(1);
			logForInsert.DataRecordParentId.Should().Be(11);
			logForInsert.DataRecordName.Should().Be(nameof(Alpha));
			logForInsert.LogEntryGroupId.Should().Be(888);
			logForInsert.UserId.Should().Be(999);
			logForInsert.TimestampUtc.Should().Be(DateTime.Today);
			logForInsert.Properties[nameof(Alpha.Delta)].Value.Should().Be(((int)LogLevel.Error).ToString());
			logForInsert.Properties[nameof(Alpha.Delta)].Type.Should().Be(FieldValueType.Integer);
		}

		[TestMethod]
		public void ConvertTreadEnumAsStringTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Delta),
					DataRecordName = nameof(Alpha),
					DataType = typeof(LogLevel),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = LogLevel.Error,
					TimestampUtc = DateTime.Today
				}
			};

			var settings = new DataRecordLogSettings
			{
				EnumAsString = true,
				UnsupportedDataTypeAction = UnsupportedDataTypeAction.Ignore
			};

			var classUnderTest = new DataRecordLogEngine(settings);

			// Act
			var logs = classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);
			var logForInsert = logs.First();

			logForInsert.Action.Should().Be(DataRecordAction.Insert);
			logForInsert.DataRecordId.Should().Be(1);
			logForInsert.DataRecordParentId.Should().Be(11);
			logForInsert.DataRecordName.Should().Be(nameof(Alpha));
			logForInsert.LogEntryGroupId.Should().Be(888);
			logForInsert.UserId.Should().Be(999);
			logForInsert.TimestampUtc.Should().Be(DateTime.Today);
			logForInsert.Properties[nameof(Alpha.Delta)].Value.Should().Be(LogLevel.Error.ToString());
			logForInsert.Properties[nameof(Alpha.Delta)].Type.Should().Be(FieldValueType.String);
		}

		[DataTestMethod]
		[DataRow(nameof(Alpha.Beta), 123, "123", typeof(int), FieldValueType.Integer, DisplayName = "DataType Integer")]
		[DataRow(nameof(Alpha.Gamma), "DarkwingDuck", "DarkwingDuck", typeof(string), FieldValueType.String, DisplayName = "DataType String")]
		[DataRow(nameof(Alpha.Delta), LogLevel.Error, "4", typeof(LogLevel), FieldValueType.Integer, DisplayName = "DataType Enum as Integer")]
		[DataRow(nameof(Alpha.Zeta), 10L, "10", typeof(long), FieldValueType.Long, DisplayName = "DataType Long")]
		[DataRow(nameof(Alpha.Theta), 10.5f, "10.5", typeof(float), FieldValueType.Float, DisplayName = "DataType Float")]
		[DataRow(nameof(Alpha.Iota), 10.5, "10.5", typeof(double), FieldValueType.Double, DisplayName = "DataType Double")]
		[DataRow(nameof(Alpha.Xi), true, "1", typeof(bool), FieldValueType.Boolean, DisplayName = "DataType Boolean True")]
		[DataRow(nameof(Alpha.Xi), false, "0", typeof(bool), FieldValueType.Boolean, DisplayName = "DataType Boolean False")]
		public void ConvertDataTypeTest(string propertyName, object value, string expectedvalue, Type propertyType, FieldValueType expectedType)
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = propertyName,
					DataRecordName = nameof(Alpha),
					DataType = propertyType,
					LogEntryGroupId = 888,
					UserId = 999,
					Value = value,
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);

			var log = logs.First();
			log.Properties[propertyName].Value.Should().Be(expectedvalue);
			log.Properties[propertyName].Type.Should().Be(expectedType);
		}

		[TestMethod]
		public void ConvertDataTypeDecimalTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Eta),
					DataRecordName = nameof(Alpha),
					DataType = typeof(decimal),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = 10.5m,
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);

			var log = logs.First();
			log.Properties[nameof(Alpha.Eta)].Value.Should().Be("10.5");
			log.Properties[nameof(Alpha.Eta)].Type.Should().Be(FieldValueType.Decimal);
		}

		[TestMethod]
		public void ConvertDataTypeGuidTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Kappa),
					DataRecordName = nameof(Alpha),
					DataType = typeof(Guid),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = "e6541e3e-d555-41b8-9641-d436c38d038b",
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);

			var log = logs.First();
			log.Properties[nameof(Alpha.Kappa)].Value.Should().Be("e6541e3e-d555-41b8-9641-d436c38d038b");
			log.Properties[nameof(Alpha.Kappa)].Type.Should().Be(FieldValueType.Guid);
		}

		[TestMethod]
		public void ConvertDataTypeDateTimeTest()
		{
			// Arrange
			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Lambda),
					DataRecordName = nameof(Alpha),
					DataType = typeof(DateTime),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = new DateTime(2019, 10, 23, 23, 10, 15, 123, DateTimeKind.Utc),
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);

			var log = logs.First();
			log.Properties[nameof(Alpha.Lambda)].Value.Should().Be("2019-10-23T23:10:15");
			log.Properties[nameof(Alpha.Lambda)].Type.Should().Be(FieldValueType.DateTime);
		}

		[TestMethod]
		public void ConvertDataTypeXmlDocumentTest()
		{
			// Arrange
			var document = new XmlDocument();
			document.LoadXml("<root><alpha>Beta</alpha></root>");

			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.My),
					DataRecordName = nameof(Alpha),
					DataType = typeof(XmlDocument),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = document,
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);

			var log = logs.First();
			log.Properties[nameof(Alpha.My)].Value.Should().Be(document.OuterXml.Compress().ToBase64());
			log.Properties[nameof(Alpha.My)].Type.Should().Be(FieldValueType.XmlDocument);
		}

		[TestMethod]
		public void ConvertDataTypeByteArrayTest()
		{
			// Arrange
			var binary = new byte[] { 1, 2, 3, 4 };

			var properties = new List<DataRecordLogProperty<int>>
			{
				new DataRecordLogProperty<int>
				{
					Action = DataRecordAction.Insert,
					DataRecordId = 1,
					DataRecordParentId = 11,
					PropertyName = nameof(Alpha.Ny),
					DataRecordName = nameof(Alpha),
					DataType = typeof(byte[]),
					LogEntryGroupId = 888,
					UserId = 999,
					Value = binary,
					TimestampUtc = DateTime.Today
				}
			};

			// Act
			var logs = _classUnderTest.Convert(properties);

			// Assert
			logs.Should().HaveCount(1);

			var log = logs.First();
			log.Properties[nameof(Alpha.Ny)].Value.Should().Be(binary.ToBase64().Compress().ToBase64());
			log.Properties[nameof(Alpha.Ny)].Type.Should().Be(FieldValueType.ByteArray);
		}

		[DataTestMethod]
		[DataRow("123", 123, FieldValueType.Integer, typeof(int), DisplayName = "DataType Integer")]
		[DataRow("DarkwingDuck", "DarkwingDuck", FieldValueType.String, typeof(string), DisplayName = "DataType String")]
		[DataRow("10", 10L, FieldValueType.Long, typeof(long), DisplayName = "DataType Long")]
		[DataRow("10.5", 10.5f, FieldValueType.Float, typeof(float), DisplayName = "DataType Float")]
		[DataRow("10.5", 10.5, FieldValueType.Double, typeof(double), DisplayName = "DataType Double")]
		[DataRow("1", true, FieldValueType.Boolean, typeof(bool), DisplayName = "DataType Boolean")]
		[DataRow("0", false, FieldValueType.Boolean, typeof(bool), DisplayName = "DataType Boolean")]
		public void ConvertFromDataRecordFieldTest(string value, object expectedvalue, FieldValueType fieldType, Type expectedType)
		{
			// Arrange
			var field = new DataRecordField
			{
				Type = fieldType,
				Value = value
			};

			// Act
			var result = _classUnderTest.Convert(field);

			// Assert
			result.Should().BeOfType(expectedType);
			result.Should().BeEquivalentTo(expectedvalue);
		}

		[TestMethod]
		public void ConvertFromDataRecordFieldDecimalTest()
		{
			// Arrange
			var field = new DataRecordField
			{
				Type = FieldValueType.Decimal,
				Value = "10.5"
			};

			// Act
			var result = _classUnderTest.Convert(field);

			// Assert
			result.Should().BeOfType(typeof(decimal));
			result.Should().BeEquivalentTo(10.5m);
		}

		[TestMethod]
		public void ConvertFromDataRecordFieldGuidTest()
		{
			// Arrange
			var field = new DataRecordField
			{
				Type = FieldValueType.Guid,
				Value = "e6541e3e-d555-41b8-9641-d436c38d038b"
			};

			// Act
			var result = _classUnderTest.Convert(field);

			// Assert
			result.Should().BeOfType(typeof(Guid));
			result.Should().BeEquivalentTo(Guid.Parse("e6541e3e-d555-41b8-9641-d436c38d038b"));
		}

		[TestMethod]
		public void ConvertFromDataRecordFieldDateTimeTest()
		{
			// Arrange
			var field = new DataRecordField
			{
				Type = FieldValueType.DateTime,
				Value = "2019-10-23T23:10:15"
			};

			// Act
			var result = _classUnderTest.Convert(field);

			// Assert
			result.Should().BeOfType(typeof(DateTime));
			result.Should().BeEquivalentTo(new DateTime(2019, 10, 23, 23, 10, 15, 0, DateTimeKind.Utc));
		}

		[TestMethod]
		public void ConvertFromDataRecordFieldXmlDocumentTest()
		{
			// Arrange
			var document = new XmlDocument();
			document.LoadXml("<root><alpha>Beta</alpha></root>");

			// Arrange
			var field = new DataRecordField
			{
				Type = FieldValueType.XmlDocument,
				Value = document.OuterXml.Compress().ToBase64()
			};

			// Act
			var result = _classUnderTest.Convert(field);

			// Assert
			result.Should().BeOfType(typeof(XmlDocument));
			result.Should().BeEquivalentTo(document);
		}

		[TestMethod]
		public void ConvertFromDataRecordFieldByteArrayTest()
		{
			// Arrange
			var binary = new byte[] { 1, 2, 3, 4 };

			// Arrange
			var field = new DataRecordField
			{
				Type = FieldValueType.ByteArray,
				Value = binary.ToBase64().Compress().ToBase64()
			};

			// Act
			var result = _classUnderTest.Convert(field);

			// Assert
			result.Should().BeOfType(typeof(byte[]));
			result.Should().BeEquivalentTo(binary);
		}
	}
}