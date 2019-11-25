using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshava.Test.Core.Logging.Models
{
	[Table("DataRecordTable")]
	public class DataRecord
	{
		[Key]
		[Column("RecordIdColumn")]
		public int Id { get; set; }

		[Column("RecordNameColumn")]
		public string RecordName { get; set; }

		[NotMapped]
		public string RecordValue { get; set; }

		public string RecordVersion { get; set; }

		[Column("TimestampColumn")]
		public DateTime? Timestamp { get; set; }
	}
}