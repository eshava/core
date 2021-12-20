using System;
using System.Linq.Expressions;
using Eshava.Core.Extensions;

namespace Eshava.Core.Linq.Extensions
{
	internal static class ParameterExpressionExtensions
	{
		public static ParameterExpression ToNullableType(this ParameterExpression parameterExpression)
		{
			var type = typeof(Nullable<>).MakeGenericType(parameterExpression.Type);

			return Expression.Parameter(type, parameterExpression.Name);
		}

		public static ParameterExpression FromNullableType(this ParameterExpression parameterExpression)
		{
			var type = parameterExpression.Type.GetDataType();

			return Expression.Parameter(type, parameterExpression.Name);
		}
	}
}