using Eshava.Core.Linq.Enums;

namespace Eshava.Core.Linq.Models
{
	public class WhereQueryProperty
	{
		public string PropertyName { get; set; }
		public string SearchTerm { get; set; }
		public CompareOperator Operator { get; set; }
	}
}