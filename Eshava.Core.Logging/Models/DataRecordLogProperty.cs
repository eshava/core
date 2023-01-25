using System;
using Eshava.Core.Logging.Enums;

namespace Eshava.Core.Logging.Models
{
	public class DataRecordLogProperty<T>
	{
		public T LogEntryGroupId { get; set; }
		public T UserId { get; set; }
		public T DataRecordId { get; set; }
		public T DataRecordParentId { get; set; }
		public DataRecordAction Action { get; set; }
		public DateTime TimestampUtc { get; set; }
		public string DataRecordName { get; set; }
		public string PropertyName { get; set; }
		public Type DataType { get; set; }
		public object Value { get; set; }
	}
}