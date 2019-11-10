using System;
using Eshava.Core.Dynamic.Fields.Enums;

namespace Eshava.Core.Dynamic.Fields.Interfaces
{
	public interface IFieldConfiguration<T>
	{
		T Id { get; set; }
		T DefinitionId { get; set; }
		FieldConfigurationType ConfigurationType { get; set; }
		bool? ValueBool { get; set; }
		int? ValueInteger { get; set; }
		long? ValueLong { get; set; }
		double? ValueDouble { get; set; }
		float? ValueFloat { get; set; }
		decimal? ValueDecimal { get; set; }
		DateTime? ValueDateTime { get; set; }
		string ValueString { get; set; }
		Guid? ValueGuid { get; set; }
		byte[] ValueBinary { get; set; }
	}
}