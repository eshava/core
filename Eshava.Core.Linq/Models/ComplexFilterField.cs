using System.Collections.Generic;
using Eshava.Core.Linq.Enums;

namespace Eshava.Core.Linq.Models
{
	public class ComplexFilterField : FilterField
	{
		public string Field { get; set; }
		public LinkOperator LinkOperator { get; set; }
		public IList<ComplexFilterField> LinkOperations { get; set; }
	}
}