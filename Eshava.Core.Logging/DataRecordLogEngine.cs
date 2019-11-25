using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Eshava.Core.Extensions;
using Eshava.Core.Logging.Enums;
using Eshava.Core.Logging.Interfaces;
using Eshava.Core.Logging.Models;

namespace Eshava.Core.Logging
{
	public class DataRecordLogEngine : IDataRecordLogEngine
	{
		private readonly static Type _typeBoolean = typeof(bool);
		private readonly static Type _typeInteger = typeof(int);
		private readonly static Type _typeLong = typeof(long);
		private readonly static Type _typeDecimal = typeof(decimal);
		private readonly static Type _typeDouble = typeof(double);
		private readonly static Type _typeFloat = typeof(float);
		private readonly static Type _typeString = typeof(string);
		private readonly static Type _typeByteArray = typeof(byte[]);
		private readonly static Type _typeGuid = typeof(Guid);
		private readonly static Type _typeDateTime = typeof(DateTime);
		private readonly static Type _typeXmlDocument = typeof(XmlDocument);

		private readonly DataRecordLogSettings _settings;
		private readonly static List<(Func<Type, bool> Check, Func<object, string> Convert)> _dataTypeActions = new List<(Func<Type, bool> Check, Func<object, string> Convert)>
		{
			(type => type == _typeInteger, value => ((int)value).ToString(CultureInfo.InvariantCulture)),
			(type => type == _typeLong, value => ((long)value).ToString(CultureInfo.InvariantCulture)),
			(type => type == _typeDecimal, value => ((decimal)value).ToString(CultureInfo.InvariantCulture)),
			(type => type == _typeDouble, value => ((double)value).ToString(CultureInfo.InvariantCulture)),
			(type => type == _typeFloat, value => ((float)value).ToString(CultureInfo.InvariantCulture)),
			(type => type == _typeString, value => value.ToString()),
			(type => type == _typeGuid, value => value.ToString()),
			(type => type == _typeBoolean, value => ((bool)value) ? "1" : "0"),
			(type => type == _typeByteArray, value => ((byte[])value).ToBase64().Compress().ToBase64()),
			(type => type == _typeXmlDocument, value => ((XmlDocument)value).OuterXml.Compress().ToBase64()),
			(type => type == _typeDateTime, value => ((DateTime)value).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"))
		};

		private readonly static List<(Func<FieldValueType, bool> Check, Func<string, object> Convert)> _fieldValueTypeActions = new List<(Func<FieldValueType, bool> Check, Func<string, object> Convert)>
		{
			(type => type == FieldValueType.Integer, value => System.Convert.ToInt32(value)),
			(type => type == FieldValueType.Long, value => System.Convert.ToInt64(value)),
			(type => type == FieldValueType.Decimal, value => System.Convert.ToDecimal(value, CultureInfo.InvariantCulture)),
			(type => type == FieldValueType.Double, value => System.Convert.ToDouble(value, CultureInfo.InvariantCulture)),
			(type => type == FieldValueType.Float, value => System.Convert.ToSingle(value, CultureInfo.InvariantCulture)),
			(type => type == FieldValueType.String, value => value),
			(type => type == FieldValueType.Guid, value => Guid.Parse(value)),
			(type => type == FieldValueType.Boolean, value => value == "1"),
			(type => type == FieldValueType.ByteArray, value => value.FromBase64().DecompressString().FromBase64()),
			(type => type == FieldValueType.XmlDocument, value => { var document = new XmlDocument(); document.LoadXml(value.FromBase64().DecompressString()); return document; }),
			(type => type == FieldValueType.DateTime, value => DateTime.ParseExact(value,"yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal))
		};

		private static readonly Dictionary<Type, FieldValueType> _typeToEnum = new Dictionary<Type, FieldValueType>
		{
			{ _typeBoolean, FieldValueType.Boolean },
			{ _typeInteger, FieldValueType.Integer },
			{ _typeLong, FieldValueType.Long },
			{ _typeDecimal, FieldValueType.Decimal },
			{ _typeFloat, FieldValueType.Float },
			{ _typeDouble, FieldValueType.Double },
			{ _typeString, FieldValueType.String },
			{ _typeDateTime, FieldValueType.DateTime },
			{ _typeGuid, FieldValueType.Guid },
			{ _typeXmlDocument, FieldValueType.XmlDocument },
			{ _typeByteArray, FieldValueType.ByteArray }
		};

		public DataRecordLogEngine(DataRecordLogSettings settings)
		{
			_settings = settings;
		}

		public object Convert(DataRecordField dataRecordField)
		{
			return _fieldValueTypeActions.Single(action => action.Check(dataRecordField.Type)).Convert(dataRecordField.Value);
		}

		public IEnumerable<DataRecordLogEntry<T>> Convert<T>(IEnumerable<DataRecordLogProperty<T>> dataRecordLogProperties)
		{
			var logEntries = new List<DataRecordLogEntry<T>>();
			var dataRecords = dataRecordLogProperties.ToList().GroupBy(p => p.DataRecordId);

			foreach (var dataRecord in dataRecords)
			{
				logEntries.Add(CreateLogEntry(dataRecord));
			}

			return logEntries;
		}

		private DataRecordLogEntry<T> CreateLogEntry<T>(IEnumerable<DataRecordLogProperty<T>> properties)
		{
			var logEntry = default(DataRecordLogEntry<T>);

			foreach (var property in properties)
			{
				if (logEntry == null)
				{
					logEntry = CreateLogEntry(property);
				}

				var propertyType = property.DataType;
				if (propertyType.IsEnum)
				{
					propertyType = _settings.EnumAsString ? _typeString : _typeInteger;
				}

				var dataTypeAction = _dataTypeActions.SingleOrDefault(action => action.Check(propertyType));
				if (dataTypeAction.Convert == null)
				{
					if (_settings.UnsupportedDataTypeAction == UnsupportedDataTypeAction.ThrowExeception)
					{
						throw new NotSupportedException("Property data type: " + propertyType.Name);
					}
					else
					{
						continue;
					}
				}

				AddProperty(logEntry, property, propertyType, dataTypeAction.Convert);
			}

			return logEntry;
		}

		private DataRecordLogEntry<T> CreateLogEntry<T>(DataRecordLogProperty<T> property)
		{
			return new DataRecordLogEntry<T>
			{
				Action = property.Action,
				DataRecordId = property.DataRecordId,
				DataRecordName = property.DataRecordName,
				DataRecordParentId = property.DataRecordParentId,
				LogEntryGroupId = property.LogEntryGroupId,
				Timestamp = property.Timestamp,
				UserId = property.UserId
			};
		}

		private void AddProperty<T>(DataRecordLogEntry<T> logEntry, DataRecordLogProperty<T> property, Type propertyType, Func<object, string> convert)
		{
			if (logEntry.Properties.ContainsKey(property.PropertyName))
			{
				logEntry.Properties[property.PropertyName].Value = property.Value == null ? null : convert(property.Value);
			}
			else
			{
				logEntry.Properties.Add(property.PropertyName, new DataRecordField
				{
					Type = _typeToEnum[propertyType],
					Value = property.Value == null ? null : convert(property.Value)
				});
			}
		}
	}
}