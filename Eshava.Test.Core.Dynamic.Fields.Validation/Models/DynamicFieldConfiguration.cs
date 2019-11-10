using System;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;

namespace Eshava.Test.Core.Dynamic.Fields.Validation.Models
{
	public class DynamicFieldConfiguration : IFieldConfiguration<string>
	{
		public string Id { get; set; }
		public string DefinitionId { get; set; }
		public FieldConfigurationType ConfigurationType { get; set; }
		public bool? ValueBool { get; set; }
		public int? ValueInteger { get; set; }
		public long? ValueLong { get; set; }
		public double? ValueDouble { get; set; }
		public float? ValueFloat { get; set; }
		public decimal? ValueDecimal { get; set; }
		public string ValueString { get; set; }
		public DateTime? ValueDateTime { get; set; }
		public Guid? ValueGuid { get; set; }
		public byte[] ValueBinary { get; set; }
	}
}