using System;
using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Models;

namespace Eshava.Core.Dynamic.Fields.Interfaces
{
	public interface IFieldValue<T>
	{
		T Id { get; set; }
		T AssignmentId { get; set; }
		bool? ValueBool { get; set; }
		int? ValueInt { get; set; }
		decimal? ValueDecimals { get; set; }
		string ValueString { get; set; }
		DateTime? ValueDateTime { get; set; }
		Guid? ValueGuid { get; set; }
		IEnumerable<FieldListItem> ListItems { get; set; }
	}
}