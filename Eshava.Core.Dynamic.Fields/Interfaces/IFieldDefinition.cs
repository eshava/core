using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Enums;

namespace Eshava.Core.Dynamic.Fields.Interfaces
{
	public interface IFieldDefinition<T>
	{
		T Id { get; set; }
		FieldType FieldType { get; set; }
		IEnumerable<IFieldConfiguration<T>> Configurations { get; set; }
	}
}