using Eshava.Core.Linq.Models;

namespace Eshava.Test.Core.Linq.Models
{
	public class FilterModel
	{
		public string SearchTerm { get; set; }
		public FilterField Beta { get; set; }
		public FilterField Chi { get; set; }
		public SpecialFilterField Gamma { get; set; }
		public FilterField Delta { get; set; }
		public FilterField Epsilon { get; set; }
		public FilterField Kappa { get; set; }
		public FilterField Rho { get; set; }
		public FilterField Sigma { get; set; }
	}
}