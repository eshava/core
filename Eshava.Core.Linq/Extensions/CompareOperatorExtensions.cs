using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Eshava.Core.Extensions;
using Eshava.Core.Linq.Constants;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Models;

namespace Eshava.Core.Linq.Extensions
{
	internal static class CompareOperatorExtensions
	{
		private static readonly Dictionary<CompareOperator, Func<MemberExpression, ConstantExpression, WhereQueryEngineOptions, Expression>> _compareOperatorExpressions = new()
		{
			{ CompareOperator.Equal, GetEqualExpression },
			{ CompareOperator.NotEqual, GetNotEqualExpression },
			{ CompareOperator.GreaterThan, GetGreaterThanExpression },
			{ CompareOperator.GreaterThanOrEqual,GetGreaterThanOrEqualExpression },
			{ CompareOperator.LessThan, GetLessThanExpression },
			{ CompareOperator.LessThanOrEqual,GetLessThanOrEqualExpression },
			{ CompareOperator.Contains, GetContainsExpression },
			{ CompareOperator.ContainsNot, GetContainsNotExpression },
			{ CompareOperator.StartsWith, GetStartsWithExpression },
			{ CompareOperator.EndsWith, GetEndsWithExpression },
			{ CompareOperator.ContainedIn, GetContainedInExpression },
		};

		private static readonly Dictionary<CompareOperator, ExpressionType> _compareOperatorExpressionType = new()
		{
			{ CompareOperator.GreaterThan, ExpressionType.GreaterThan },
			{ CompareOperator.GreaterThanOrEqual, ExpressionType.GreaterThanOrEqual },
			{ CompareOperator.LessThan, ExpressionType.LessThan },
			{ CompareOperator.LessThanOrEqual, ExpressionType.LessThanOrEqual }
		};

		public static bool ExistsExpressionType(this CompareOperator compareOperator)
		{
			return _compareOperatorExpressionType.ContainsKey(compareOperator);
		}

		public static ExpressionType GetExpressionType(this CompareOperator compareOperator)
		{
			return _compareOperatorExpressionType[compareOperator];
		}

		public static bool ExistsOperation(this CompareOperator compareOperator)
		{
			return _compareOperatorExpressions.ContainsKey(compareOperator);
		}

		public static Expression BuildExpression(this CompareOperator compareOperator, MemberExpression member, ConstantExpression constantValue, WhereQueryEngineOptions options)
		{
			return _compareOperatorExpressions[compareOperator](member, constantValue, options);
		}

		private static Expression GetEqualExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			if (member.Type == TypeConstants.String && (options.CaseInsensitive ?? false))
			{
				return Expression.Equal(Expression.Call(member, MethodInfoConstants.StringToLower), constant);
			}

			return Expression.Equal(member, constant);
		}

		private static Expression GetNotEqualExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			if (member.Type == TypeConstants.String && (options.CaseInsensitive ?? false))
			{
				return Expression.NotEqual(Expression.Call(member, MethodInfoConstants.StringToLower), constant);
			}

			return Expression.NotEqual(member, constant);
		}

		private static Expression GetGreaterThanExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			return Expression.GreaterThan(member, constant);
		}

		private static Expression GetGreaterThanOrEqualExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			return Expression.GreaterThanOrEqual(member, constant);
		}

		private static Expression GetLessThanExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			return Expression.LessThan(member, constant);
		}

		private static Expression GetLessThanOrEqualExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			return Expression.LessThanOrEqual(member, constant);
		}

		private static Expression GetContainsExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			if (member.Type == TypeConstants.String)
			{
				var memberExpression = (options.CaseInsensitive ?? false)
					? Expression.Call(member, MethodInfoConstants.StringToLower)
					: (Expression)member;

				return Expression.AndAlso(Expression.NotEqual(member, ExpressionConstants.StringNull), Expression.Call(memberExpression, MethodInfoConstants.StringContains, constant));
			}

			if (member.Type.ImplementsIEnumerable() && member.Type.ImplementsInterface(TypeConstants.IList))
			{
				var genericType = member.Type.GetDataTypeFromIEnumerable();
				if (genericType != TypeConstants.String || !(options.CaseInsensitive ?? false))
				{
					var nullCheckExpression = Expression.NotEqual(member, ExpressionConstants.ObjectNull);
					var enumerableContainsExpression = Expression.Call(member, member.Type.GetMethod(nameof(IList.Contains), [genericType]), constant);

					return Expression.AndAlso(nullCheckExpression, enumerableContainsExpression);
				}


			}

			if (member.Type.ImplementsIEnumerable())
			{
				var genericType = member.Type.GetDataTypeFromIEnumerable();
				var nullCheckExpression = Expression.NotEqual(member, ExpressionConstants.ObjectNull);
				var parameterExpression = Expression.Parameter(genericType, "p" + DateTime.UtcNow.Millisecond);
				var equalsExpression = genericType == TypeConstants.String && (options.CaseInsensitive ?? false)
					? Expression.Equal(Expression.Call(parameterExpression, MethodInfoConstants.StringToLower), constant)
					: Expression.Equal(parameterExpression, constant)
					;
				var equalsLambdaExpression = Expression.Lambda(equalsExpression, [parameterExpression]);
				var anyMethodCallExpression = Expression.Call(MethodInfoConstants.GetGenericAny(genericType), member, equalsLambdaExpression);

				return Expression.AndAlso(nullCheckExpression, anyMethodCallExpression);
			}

			throw new NotSupportedException("The data type of the property has to be of type string or must implement 'IList'");
		}

		private static Expression GetContainsNotExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			if (member.Type == TypeConstants.String)
			{
				var memberExpression = (options.CaseInsensitive ?? false)
					? Expression.Call(member, MethodInfoConstants.StringToLower)
					: (Expression)member;

				return Expression.OrElse(Expression.Equal(member, ExpressionConstants.StringNull), Expression.Not(Expression.Call(memberExpression, MethodInfoConstants.StringContains, constant)));
			}

			if (member.Type.ImplementsIEnumerable() && member.Type.ImplementsInterface(TypeConstants.IList))
			{
				var genericType = member.Type.GetDataTypeFromIEnumerable();
				if (genericType != TypeConstants.String || !(options.CaseInsensitive ?? false))
				{
					var nullCheckExpression = Expression.Equal(member, ExpressionConstants.ObjectNull);
					var enumerableContainsExpression = Expression.Call(member, member.Type.GetMethod(nameof(IList.Contains), [genericType]), constant);

					return Expression.OrElse(nullCheckExpression, Expression.Not(enumerableContainsExpression));
				}
			}

			if (member.Type.ImplementsIEnumerable())
			{
				var genericType = member.Type.GetDataTypeFromIEnumerable();
				var nullCheckExpression = Expression.Equal(member, ExpressionConstants.ObjectNull);
				var parameterExpression = Expression.Parameter(genericType, "p" + DateTime.UtcNow.Millisecond);
				var notEqualsExpression = genericType == TypeConstants.String && (options.CaseInsensitive ?? false)
					? Expression.NotEqual(Expression.Call(parameterExpression, MethodInfoConstants.StringToLower), constant)
					: Expression.NotEqual(parameterExpression, constant)
					;
				var notEqualsLambdaExpression = Expression.Lambda(notEqualsExpression, [parameterExpression]);
				var allMethodCallExpression = Expression.Call(MethodInfoConstants.GetGenericAll(genericType), member, notEqualsLambdaExpression);

				return Expression.OrElse(nullCheckExpression, allMethodCallExpression);
			}

			throw new NotSupportedException("The data type of the property has to be of type string or must implement 'IList'");
		}

		private static Expression GetStartsWithExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			if (member.Type == TypeConstants.String)
			{
				var memberExpression = (options.CaseInsensitive ?? false)
					? Expression.Call(member, MethodInfoConstants.StringToLower)
					: (Expression)member;

				return Expression.AndAlso(Expression.NotEqual(member, ExpressionConstants.StringNull), Expression.Call(memberExpression, MethodInfoConstants.StringStartsWith, constant));
			}

			throw new NotSupportedException("The data type of the property has to be of type string");
		}

		private static Expression GetEndsWithExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			if (member.Type == TypeConstants.String)
			{
				var memberExpression = (options.CaseInsensitive ?? false)
					? Expression.Call(member, MethodInfoConstants.StringToLower)
					: (Expression)member;

				return Expression.AndAlso(Expression.NotEqual(member, ExpressionConstants.StringNull), Expression.Call(memberExpression, MethodInfoConstants.StringEndsWith, constant));
			}

			throw new NotSupportedException("The data type of the property has to be of type string");
		}

		private static Expression GetContainedInExpression(MemberExpression member, ConstantExpression constant, WhereQueryEngineOptions options)
		{
			var enumerableMemberMethod = constant.Type.GetMethod(nameof(IList.Contains), [member.Type]);
			Expression memberExpression;

			if (member.Type.IsEnum)
			{
				memberExpression = Expression.Convert(member, TypeConstants.Object);
			}
			else if (member.Type == TypeConstants.String && (options.CaseInsensitive ?? false))
			{
				memberExpression = Expression.Call(member, MethodInfoConstants.StringToLower);
			}
			else
			{
				memberExpression = member;
			}

			if (member.Type == TypeConstants.String)
			{
				return Expression.AndAlso(Expression.NotEqual(member, ExpressionConstants.StringNull), Expression.Call(constant, enumerableMemberMethod, memberExpression));
			}

			return Expression.Call(constant, enumerableMemberMethod, memberExpression);
		}
	}
}
