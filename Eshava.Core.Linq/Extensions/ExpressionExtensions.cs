using System.Linq.Expressions;

namespace Eshava.Core.Linq.Extensions
{
	internal static class ExpressionExtensions
	{
		internal static Expression ChangeParameterExpression(this Expression condition, ParameterExpression newParameter)
		{
			var previousVisitor = new ReplaceExpressionVisitor(condition, newParameter);

			return previousVisitor.Visit(condition);
		}
	}
}