using System;
using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Models;

namespace Eshava.Core.Dynamic.Fields.Interfaces
{
	public interface IFieldValue<T>
	{
		T Id { get; set; }
		T AssignmentId { get; set; }
		bool? ValueBoolean { get; set; }
		int? ValueInteger { get; set; }
		long? ValueLong { get; set; }
		double? ValueDouble { get; set; }
		float? ValueFloat { get; set; }
		decimal? ValueDecimal { get; set; }
		string ValueString { get; set; }
		DateTime? ValueDateTime { get; set; }
		Guid? ValueGuid { get; set; }
		IEnumerable<FieldListItem> ListItems { get; set; }
	}
}