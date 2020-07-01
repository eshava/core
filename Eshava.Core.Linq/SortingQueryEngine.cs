using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Interfaces;
using Eshava.Core.Linq.Models;

namespace Eshava.Core.Linq
{
	public class SortingQueryEngine : AbstractQueryEngine, ISortingQueryEngine
	{
		public OrderByCondition BuildSortCondition<T>(SortOrder sortOrder, Expression<Func<T, object>> expression) where T : class
		{
			var member = GetMemberExpressionAndParameter(expression);

			if (member.Expression != null && sortOrder != SortOrder.None)
			{
				return new OrderByCondition { SortOrder = sortOrder, Member = member.Expression, Parameter = member.Parameter };
			}

			return null;
		}

		public IEnumerable<OrderByCondition> BuildSortConditions<T>(QueryParameters queryParameters, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null) where T : class
		{
			var sortingConditions = new List<OrderByCondition>();

			if (!(queryParameters?.SortingQueryProperties?.Any() ?? false))
			{
				return sortingConditions;
			}

			var type = typeof(T);
			var parameter = Expression.Parameter(type, "p");
			var propertyInfos = type.GetProperties();

			if (mappings == null)
			{
				mappings = new Dictionary<string, List<Expression<Func<T, object>>>>();
			}

			foreach (var property in queryParameters.SortingQueryProperties)
			{
				if (mappings.ContainsKey(property.PropertyName))
				{
					var members = mappings[property.PropertyName].Select(GetMemberExpressionAndParameter).Where(m => m.Expression != null).ToList();
					members.ForEach(member => sortingConditions.Add(new OrderByCondition { SortOrder = property.SortOrder, Member = member.Expression, Parameter = member.Parameter }));
				}
				else
				{
					var expression = GetProperySortCondition(property.PropertyName, propertyInfos, parameter);
					if (expression != null)
					{
						sortingConditions.Add(new OrderByCondition { SortOrder = property.SortOrder, Member = expression, Parameter = parameter });
					}
				}
			}

			return sortingConditions;
		}

		public IQueryable<T> ApplySorting<T>(IQueryable<T> query, IEnumerable<OrderByCondition> conditions) where T : class
		{
			if (conditions?.Any() ?? false)
			{
				var orderBy = conditions.First();
				var orderedQuery = AddOrder(query, orderBy);
				var orderThenBy = conditions.Skip(1).ToList();

				if (orderThenBy.Any())
				{
					orderThenBy.ForEach(thenBy => orderedQuery = AddOrderThen(orderedQuery, thenBy));
				}

				query = orderedQuery;
			}

			return query;
		}

		public IOrderedQueryable<T> AddOrder<T>(IQueryable<T> query, OrderByCondition condition) where T : class
		{
			if (condition.SortOrder == SortOrder.Ascending)
			{
				return AddOrderBy(query, SortingType.OrderBy, condition.Member, condition.Parameter);
			}

			return AddOrderBy(query, SortingType.OrderByDescending, condition.Member, condition.Parameter);
		}

		public IOrderedQueryable<T> AddOrderThen<T>(IOrderedQueryable<T> query, OrderByCondition condition) where T : class
		{
			if (condition.SortOrder == SortOrder.Ascending)
			{
				return AddOrderBy(query, SortingType.OrderThenBy, condition.Member, condition.Parameter);
			}

			return AddOrderBy(query, SortingType.OrderThenByDescending, condition.Member, condition.Parameter);
		}

		private IOrderedQueryable<T> AddOrderBy<T>(IQueryable<T> source, SortingType type, MemberExpression expression, ParameterExpression parameter) where T : class
		{
			var command = default(string);
			switch (type)
			{
				case SortingType.OrderBy:
					command = nameof(Queryable.OrderBy);
					break;
				case SortingType.OrderThenBy:
					command = nameof(Queryable.ThenBy);
					break;
				case SortingType.OrderByDescending:
					command = nameof(Queryable.OrderByDescending);
					break;
				case SortingType.OrderThenByDescending:
					command = nameof(Queryable.ThenByDescending);
					break;
				default:
					throw new NotSupportedException();
			}

			var orderByExpression = Expression.Lambda(expression, parameter);
			var typeArguments = new Type[] { typeof(T), expression.Type };
			var resultExpression = Expression.Call(typeof(Queryable), command, typeArguments, source.Expression, Expression.Quote(orderByExpression));

			return source.Provider.CreateQuery<T>(resultExpression) as IOrderedQueryable<T>;
		}

		private (MemberExpression Expression, ParameterExpression Parameter) GetMemberExpressionAndParameter<T>(Expression<Func<T, object>> sortingExpression) where T : class
		{
			var memberExpression = GetMemberExpression(sortingExpression);
			
			return (memberExpression, sortingExpression.Parameters.First());
		}

		private MemberExpression GetProperySortCondition(string propertyName, IEnumerable<PropertyInfo> propertyInfos, ParameterExpression parameterExpression)
		{
			var propertyInfo = propertyInfos.SingleOrDefault(p => p.Name.Equals(propertyName));

			return propertyInfo == null ? null : Expression.MakeMemberAccess(parameterExpression, propertyInfo);
		}
	}
}