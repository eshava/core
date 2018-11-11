using System;
using System.Linq.Expressions;

namespace Eshava.Core.Linq
{
	public abstract class AbstractQueryEngine
	{
		protected MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> funcExpression) where T : class
		{
			MemberExpression memberExpression = null;

			if (funcExpression.Body is UnaryExpression expBodyMemberExpression && expBodyMemberExpression.Operand is MemberExpression)
			{
				memberExpression = (MemberExpression)expBodyMemberExpression.Operand;
			}
			else if (funcExpression.Body is UnaryExpression expBodyBinaryExpression && expBodyBinaryExpression.Operand is BinaryExpression)
			{
				throw new NotSupportedException("Logical binary expressions are not supported");
			}
			else if (funcExpression.Body is MemberExpression expression)
			{
				memberExpression = expression;
			}
			
			return memberExpression;
		}
	}
}