using Eshava.Core.Dynamic.Fields.Interfaces;

namespace Eshava.Test.Core.Dynamic.Fields.Validation.Models
{
	public class DynamicFieldAssignment : IFieldAssignment<string, int>
	{
		public string Id { get; set; }
		public string DefinitionId { get; set; }
		public int ParentDataTypeId { get; set; }
	}
}