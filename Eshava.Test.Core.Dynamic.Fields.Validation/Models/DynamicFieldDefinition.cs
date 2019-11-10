using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;

namespace Eshava.Test.Core.Dynamic.Fields.Validation.Models
{
	public class DynamicFieldDefinition : IFieldDefinition<string>
	{
		public string Id { get; set; }
		public FieldType FieldType { get; set; }
		public IEnumerable<IFieldConfiguration<string>> Configurations { get; set; }
	}
}