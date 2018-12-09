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
				return AddOrderBy(query, condition.Member, condition.Parameter);
			}

			return AddOrderByDescending(query, condition.Member, condition.Parameter);
		}

		public IOrderedQueryable<T> AddOrderThen<T>(IOrderedQueryable<T> query, OrderByCondition condition) where T : class
		{
			if (condition.SortOrder == SortOrder.Ascending)
			{
				return AddOrderThenBy(query, condition.Member, condition.Parameter);
			}

			return AddOrderThenByDescending(query, condition.Member, condition.Parameter);
		}

		private IOrderedQueryable<T> AddOrderBy<T>(IQueryable<T> source, MemberExpression expression, ParameterExpression parameter) where T : class
		{
			var dataTypeMappings = new Dictionary<Type, Func<IQueryable<T>, MemberExpression, ParameterExpression, IOrderedQueryable<T>>>
			{
				{ typeof(string), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, string>>(m, p)) },
				{ typeof(bool), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, bool>>(m, p)) },
				{ typeof(bool?), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, bool?>>(m, p)) },
				{ typeof(int), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, int>>(m, p)) },
				{ typeof(int?), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, int?>>(m, p)) },
				{ typeof(long), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, long>>(m, p)) },
				{ typeof(long?), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, long?>>(m, p)) },
				{ typeof(decimal), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, decimal>>(m, p)) },
				{ typeof(decimal?), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, decimal?>>(m, p)) },
				{ typeof(double), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, double>>(m, p)) },
				{ typeof(double?), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, double?>>(m, p)) },
				{ typeof(float), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, float>>(m, p)) },
				{ typeof(float?), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, float?>>(m, p)) },
				{ typeof(DateTime), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, DateTime>>(m, p)) },
				{ typeof(DateTime?), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, DateTime?>>(m, p)) },
				{ typeof(Guid), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, Guid>>(m, p)) },
				{ typeof(Guid?), (s, m, p) => s.OrderBy(Expression.Lambda<Func<T, Guid?>>(m, p)) }
			};

			return dataTypeMappings[expression.Type](source, expression, parameter);
		}

		private IOrderedQueryable<T> AddOrderByDescending<T>(IQueryable<T> source, MemberExpression expression, ParameterExpression parameter) where T : class
		{
			var dataTypeMappings = new Dictionary<Type, Func<IQueryable<T>, MemberExpression, ParameterExpression, IOrderedQueryable<T>>>
			{
				{ typeof(string), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, string>>(m, p)) },
				{ typeof(bool), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, bool>>(m, p)) },
				{ typeof(bool?), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, bool?>>(m, p)) },
				{ typeof(int), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, int>>(m, p)) },
				{ typeof(int?), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, int?>>(m, p)) },
				{ typeof(long), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, int>>(m, p)) },
				{ typeof(long?), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, int?>>(m, p)) },
				{ typeof(decimal), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, decimal>>(m, p)) },
				{ typeof(decimal?), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, decimal?>>(m, p)) },
				{ typeof(double), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, double>>(m, p)) },
				{ typeof(double?), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, double?>>(m, p)) },
				{ typeof(float), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, float>>(m, p)) },
				{ typeof(float?), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, float?>>(m, p)) },
				{ typeof(DateTime), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, DateTime>>(m, p)) },
				{ typeof(DateTime?), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, DateTime?>>(m, p)) },
				{ typeof(Guid), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, Guid>>(m, p)) },
				{ typeof(Guid?), (s, m, p) => s.OrderByDescending(Expression.Lambda<Func<T, Guid?>>(m, p)) }
			};

			return dataTypeMappings[expression.Type](source, expression, parameter);
		}

		private IOrderedQueryable<T> AddOrderThenBy<T>(IOrderedQueryable<T> source, MemberExpression expression, ParameterExpression parameter) where T : class
		{
			var dataTypeMappings = new Dictionary<Type, Func<IOrderedQueryable<T>, MemberExpression, ParameterExpression, IOrderedQueryable<T>>>
			{
				{ typeof(string), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, string>>(m, p)) },
				{ typeof(bool), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, bool>>(m, p)) },
				{ typeof(bool?), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, bool?>>(m, p)) },
				{ typeof(int), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, int>>(m, p)) },
				{ typeof(int?), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, int?>>(m, p)) },
				{ typeof(long), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, long>>(m, p)) },
				{ typeof(long?), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, long?>>(m, p)) },
				{ typeof(decimal), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, decimal>>(m, p)) },
				{ typeof(decimal?), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, decimal?>>(m, p)) },
				{ typeof(double), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, double>>(m, p)) },
				{ typeof(double?), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, double?>>(m, p)) },
				{ typeof(float), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, float>>(m, p)) },
				{ typeof(float?), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, float?>>(m, p)) },
				{ typeof(DateTime), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, DateTime>>(m, p)) },
				{ typeof(DateTime?), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, DateTime?>>(m, p)) },
				{ typeof(Guid), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, Guid>>(m, p)) },
				{ typeof(Guid?), (s, m, p) => s.ThenBy(Expression.Lambda<Func<T, Guid?>>(m, p)) }
			};

			return dataTypeMappings[expression.Type](source, expression, parameter);
		}

		private IOrderedQueryable<T> AddOrderThenByDescending<T>(IOrderedQueryable<T> source, MemberExpression expression, ParameterExpression parameter) where T : class
		{
			var dataTypeMappings = new Dictionary<Type, Func<IOrderedQueryable<T>, MemberExpression, ParameterExpression, IOrderedQueryable<T>>>
			{
				{ typeof(string), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, string>>(m, p)) },
				{ typeof(bool), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, bool>>(m, p)) },
				{ typeof(bool?), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, bool?>>(m, p)) },
				{ typeof(int), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, int>>(m, p)) },
				{ typeof(int?), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, int?>>(m, p)) },
				{ typeof(long), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, long>>(m, p)) },
				{ typeof(long?), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, long?>>(m, p)) },
				{ typeof(decimal), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, decimal>>(m, p)) },
				{ typeof(decimal?), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, decimal?>>(m, p)) },
				{ typeof(double), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, double>>(m, p)) },
				{ typeof(double?), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, double?>>(m, p)) },
				{ typeof(float), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, float>>(m, p)) },
				{ typeof(float?), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, float?>>(m, p)) },
				{ typeof(DateTime), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, DateTime>>(m, p)) },
				{ typeof(DateTime?), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, DateTime?>>(m, p)) },
				{ typeof(Guid), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, Guid>>(m, p)) },
				{ typeof(Guid?), (s, m, p) => s.ThenByDescending(Expression.Lambda<Func<T, Guid?>>(m, p)) }
			};

			return dataTypeMappings[expression.Type](source, expression, parameter);
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