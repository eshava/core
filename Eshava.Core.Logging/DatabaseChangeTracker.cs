using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Logging.Enums;
using Eshava.Core.Logging.Interfaces;
using Eshava.Core.Logging.Models;

namespace Eshava.Core.Logging
{
	/// <summary>
	/// Creates the log based on <see cref="TableAttribute"/> and <see cref="ColumnAttribute"/> 
	/// </summary>
	/// <typeparam name="P">Type of the Identifer properties of the log</typeparam>
	public class DatabaseChangeTracker<P> : IDataRecordChangeTracker<P>
	{
		public IEnumerable<DataRecordLogProperty<P>> CreateInsertLogs<T>(T dataRecord, P dataRecordId, P dataRecordParentId) where T : class
		{
			var dataRecordType = typeof(T);
			if (Equals(default(T), dataRecord))
			{
				throw new ArgumentNullException("The data record must not be NULL: " + dataRecordType.Name);
			}

			return CreateInsertOrUpdateLogs(dataRecord, null, dataRecordId, dataRecordParentId);
		}

		public IEnumerable<DataRecordLogProperty<P>> CreateUpdateLogs<T>(T dataRecord, T dataRecordToCompare, P dataRecordId, P dataRecordParentId) where T : class
		{
			var dataRecordType = typeof(T);
			if (Equals(default(T), dataRecord))
			{
				throw new ArgumentNullException("The data record must not be NULL: " + dataRecordType.Name);
			}

			if (Equals(default(T), dataRecordToCompare))
			{
				throw new ArgumentNullException("The data record for comparison  must not be NULL: " + dataRecordType.Name);
			}

			return CreateInsertOrUpdateLogs(dataRecord, dataRecordToCompare, dataRecordId, dataRecordParentId);
		}

		/// <summary>
		/// To create an deletion log the <see cref="KeyAttribute"/> should be set on one property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataRecordId"></param>
		/// <param name="dataRecordParentId"></param>
		/// <returns></returns>
		public DataRecordLogProperty<P> CreateDeleteLog<T>(P dataRecordId, P dataRecordParentId) where T : class
		{
			var dataRecordType = typeof(T);
			(var tableName, var primaryKeyColumnName) = GetTableInformation(dataRecordType);

			if (tableName.IsNullOrEmpty())
			{
				throw new Exception($"The deletion log could not be created for: {dataRecordType.Name}. The TableAttribute is not set.");
			}

			var message = $"{(dataRecordType.Name.EndsWith("Model") ? dataRecordType.Name.Substring(0, dataRecordType.Name.IndexOf("Model", StringComparison.Ordinal)) : dataRecordType.Name)} deleted";

			return new DataRecordLogProperty<P>
			{
				Action = DataRecordAction.Delete,
				DataRecordName = tableName,
				PropertyName = primaryKeyColumnName,
				Value = message,
				DataType = typeof(string),
				DataRecordId = dataRecordId,
				DataRecordParentId = dataRecordParentId
			};
		}

		private IEnumerable<DataRecordLogProperty<P>> CreateInsertOrUpdateLogs<T>(T dataRecord, T dataRecordToCompare, P dataRecordId, P dataRecordParentId) where T : class
		{
			var dataRecordType = typeof(T);
			var tableName = GetTableName(dataRecordType);
			if (tableName.IsNullOrEmpty())
			{
				throw new Exception($"The change log could not be created for: {dataRecordType.Name}. The TableAttribute is not set.");
			}

			var insert = dataRecordToCompare == default(T);
			var logs = new List<DataRecordLogProperty<P>>();

			foreach (var propertyInfo in dataRecordType.GetProperties())
			{
				var column = GetColumnName(propertyInfo);
				if (column.IsNullOrEmpty())
				{
					continue;
				}

				var valueToCompare = insert ? null : propertyInfo.GetValue(dataRecordToCompare);
				var valueCurrent = propertyInfo.GetValue(dataRecord);

				if (insert || !Equals(valueToCompare, valueCurrent))
				{
					logs.Add(new DataRecordLogProperty<P>
					{
						Action = insert ? DataRecordAction.Insert : DataRecordAction.Update,
						DataRecordName = tableName,
						PropertyName = column,
						Value = valueCurrent,
						DataType = propertyInfo.PropertyType.GetDataType(),
						DataRecordId = dataRecordId,
						DataRecordParentId = dataRecordParentId
					});

				}
			}

			return logs;
		}

		private (string TableName, string PrimaryKeyColumnName) GetTableInformation(Type type)
		{
			var tableName = GetTableName(type);
			if (tableName.IsNullOrEmpty())
			{
				return (null, null);
			}

			return (tableName, GetPrimaryKeyProperty(type));
		}

		private string GetTableName(Type type)
		{
			var tableAttribute = type.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

			return tableAttribute?.Name;
		}

		private string GetPrimaryKeyProperty(Type type)
		{
			var propertyInfo = type.GetProperties().FirstOrDefault(p => (p.GetCustomAttribute(typeof(KeyAttribute)) as KeyAttribute) != null);

			return propertyInfo != null ? GetColumnName(propertyInfo) : null;
		}

		private string GetColumnName(PropertyInfo propertyInfo)
		{
			var notMappedAttribute = propertyInfo.GetCustomAttribute(typeof(NotMappedAttribute)) as NotMappedAttribute;

			if (notMappedAttribute == null && propertyInfo.GetCustomAttribute(typeof(ColumnAttribute)) is ColumnAttribute columnAttribute)
			{
				return columnAttribute.Name;
			}

			return null;
		}
	}
}