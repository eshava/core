using System;
using System.Collections.Generic;
using Eshava.Core.Logging.Enums;

namespace Eshava.Core.Logging.Models
{
	public class DataRecordLogEntry<T>
	{
		public DataRecordLogEntry()
		{
			Properties = new Dictionary<string, DataRecordField>();
		}

		public T LogEntryGroupId { get; set; }
		public T UserId { get; set; }
		public T DataRecordId { get; set; }
		public T DataRecordParentId { get; set; }
		public DateTime Timestamp { get; set; }
		public DataRecordAction Action { get; set; }
		public string DataRecordName { get; set; }
		public Dictionary<string, DataRecordField> Properties { get; set; }
	}
}