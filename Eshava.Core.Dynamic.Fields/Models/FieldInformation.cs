using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Interfaces;

namespace Eshava.Core.Dynamic.Fields.Models
{
	public class FieldInformation<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T,D> where FV : IFieldValue<T>
	{
		public IEnumerable<FD> Definitions { get; set; }
		public IEnumerable<FA> Assignments { get; set; }
		public IEnumerable<FV> Values { get; set; }

		public bool IsValid
		{
			get
			{
				return Definitions != null && Definitions.Any() &&
					   Assignments != null && Assignments.Any() &&
					   Values != null && Values.Any();
			}
		}

		public FV GetField(string fieldId)
		{
			return Values.FirstOrDefault(d => d.Id.ToString() == fieldId);
		}

		public FA GetAssignment(T assignmentId)
		{
			return Assignments.FirstOrDefault(d => Equals(d.Id, assignmentId));
		}

		public FD GetDefinition(T definitionId)
		{
			return Definitions.FirstOrDefault(d => Equals(d.Id, definitionId));
		}
	}
}