using Eshava.Core.Linq.Attributes;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Models;

namespace Eshava.Test.Core.Linq.Models
{
	public class FilterModel
	{
		public string SearchTerm { get; set; }
		
		public FilterField Beta { get; set; }
		
		public FilterField Chi { get; set; }

		[AllowedCompareOperator(CompareOperator.Equal)]
		[AllowedCompareOperator(CompareOperator.Contains)]
		public SpecialFilterField Gamma { get; set; }

		[AllowedCompareOperator(CompareOperator.Equal)]
		[AllowedCompareOperator(CompareOperator.Contains)]
		[AllowedCompareOperator(CompareOperator.ContainsNot)]
		
		public FilterField Delta { get; set; }
		
		public FilterField Epsilon { get; set; }
		
		public FilterField Kappa { get; set; }
		
		public FilterField Rho { get; set; }
		
		public FilterField Sigma { get; set; }
	}
}