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
	/// Create the log information based on data type name and property names
	/// </summary>
	/// <typeparam name="P">Type of the Identifer properties of the log</typeparam>
	public class DataModelChangeTracker<P> : IDataRecordChangeTracker<P>
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
			var keyProperty = GetPrimaryKeyProperty(dataRecordType);
			var message = $"{(dataRecordType.Name.EndsWith("Model") ? dataRecordType.Name.Substring(0, dataRecordType.Name.IndexOf("Model", StringComparison.Ordinal)) : dataRecordType.Name)} deleted";

			return new DataRecordLogProperty<P>
			{
				Action = DataRecordAction.Delete,
				DataRecordName = dataRecordType.Name,
				PropertyName = keyProperty,
				Value = message,
				DataType = typeof(string),
				DataRecordId = dataRecordId,
				DataRecordParentId = dataRecordParentId
			};
		}

		private IEnumerable<DataRecordLogProperty<P>> CreateInsertOrUpdateLogs<T>(T dataRecord, T dataRecordToCompare, P dataRecordId, P dataRecordParentId) where T : class
		{
			var dataRecordType = typeof(T);
			var insert = dataRecordToCompare == default(T);
			var logs = new List<DataRecordLogProperty<P>>();

			foreach (var propertyInfo in dataRecordType.GetProperties())
			{
				var notMappedAttribute = propertyInfo.GetCustomAttribute(typeof(NotMappedAttribute)) as NotMappedAttribute;
				if (notMappedAttribute != null)
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
						DataRecordName = dataRecordType.Name,
						PropertyName = propertyInfo.Name,
						Value = valueCurrent,
						DataType = propertyInfo.PropertyType.GetDataType(),
						DataRecordId = dataRecordId,
						DataRecordParentId = dataRecordParentId
					});
				}
			}

			return logs;
		}

		private string GetPrimaryKeyProperty(Type type)
		{
			var propertyInfo = type.GetProperties().FirstOrDefault(p => (p.GetCustomAttribute(typeof(KeyAttribute)) as KeyAttribute) != null);

			return propertyInfo?.Name;
		}
	}
}