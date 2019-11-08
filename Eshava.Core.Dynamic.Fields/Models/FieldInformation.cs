using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Interfaces;

namespace Eshava.Core.Dynamic.Fields.Models
{
	public class FieldInformation<T, D>
	{
		public IList<IFieldDefinition<T>> Definitions { get; set; }
		public IList<IFieldAssignment<T, D>> Assignments { get; set; }
		public IList<IFieldValue<T>> Values { get; set; }

		public bool IsValid
		{
			get
			{
				return Definitions != null && Definitions.Count > 0 &&
					   Assignments != null && Assignments.Count > 0 &&
					   Values != null && Values.Count > 0;
			}
		}

		public IFieldValue<T> GetField(string fieldId)
		{
			return Values.FirstOrDefault(d => d.Id.ToString() == fieldId);
		}

		public IFieldAssignment<T, D> GetAssignment(T assignmentId)
		{
			return Assignments.FirstOrDefault(d => Equals(d.Id, assignmentId));
		}

		public IFieldDefinition<T> GetDefinition(T definitionId)
		{
			return Definitions.FirstOrDefault(d => Equals(d.Id, definitionId));
		}
	}
}