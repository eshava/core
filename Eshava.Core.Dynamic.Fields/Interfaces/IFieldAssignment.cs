namespace Eshava.Core.Dynamic.Fields.Interfaces
{
	public interface IFieldAssignment<T, D>
	{
		T Id { get; set; }
		T DefinitionId { get; set; }
		D ParentDataTypeId { get; set; }
	}
}