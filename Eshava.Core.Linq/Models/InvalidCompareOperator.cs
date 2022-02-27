using Eshava.Core.Linq.Enums;

namespace Eshava.Core.Linq.Models
{
	public class InvalidCompareOperator
	{
		public string Field { get; set; }
		public CompareOperator Operator { get; set; }
	}
}