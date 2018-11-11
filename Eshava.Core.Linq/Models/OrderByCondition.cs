using System.Linq.Expressions;
using Eshava.Core.Linq.Enums;

namespace Eshava.Core.Linq.Models
{
	public class OrderByCondition
	{
		public SortOrder SortOrder { get; set; }
		public MemberExpression Member { get; set; }
		public ParameterExpression Parameter { get; set; }
	}
}