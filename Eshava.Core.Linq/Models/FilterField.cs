using Eshava.Core.Linq.Enums;

namespace Eshava.Core.Linq.Models
{
	public class FilterField
	{
		public string SearchTerm { get; set; }
		public CompareOperator Operator { get; set; }
	}
}