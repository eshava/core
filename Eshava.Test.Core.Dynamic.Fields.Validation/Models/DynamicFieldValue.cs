using System;
using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;

namespace Eshava.Test.Core.Dynamic.Fields.Validation.Models
{
	public class DynamicFieldValue : IFieldValue<string>
	{
		public string Id { get; set; }
		public string AssignmentId { get; set; }
		public bool? ValueBool { get; set; }
		public int? ValueInt { get; set; }
		public decimal? ValueDecimals { get; set; }
		public string ValueString { get; set; }
		public DateTime? ValueDateTime { get; set; }
		public Guid? ValueGuid { get; set; }
		public IEnumerable<FieldListItem> ListItems { get; set; }
	}
}