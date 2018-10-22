using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Models;

namespace Eshava.Core.Linq.Interfaces
{
	public interface ISortingQueryEngine
	{
		OrderByCondition BuildSortCondition<T>(SortOrder sortOrder, Expression<Func<T, object>> expression) where T : class;
		IEnumerable<OrderByCondition> BuildSortConditions<T>(QueryParameters queryParameters, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null) where T : class;
		IQueryable<T> ApplySorting<T>(IQueryable<T> query, List<OrderByCondition> conditions) where T : class;
		IOrderedQueryable<T> AddOrder<T>(IQueryable<T> query, OrderByCondition condition) where T : class;
		IOrderedQueryable<T> AddOrderThen<T>(IOrderedQueryable<T> query, OrderByCondition condition) where T : class;
		IOrderedQueryable<T> AddOrderBy<T>(IQueryable<T> source, MemberExpression expression, ParameterExpression parameter) where T : class;
		IOrderedQueryable<T> AddOrderByDescending<T>(IQueryable<T> source, MemberExpression expression, ParameterExpression parameter) where T : class;
		IOrderedQueryable<T> AddOrderThenBy<T>(IOrderedQueryable<T> source, MemberExpression expression, ParameterExpression parameter) where T : class;
		IOrderedQueryable<T> AddOrderThenByDescending<T>(IOrderedQueryable<T> source, MemberExpression expression, ParameterExpression parameter) where T : class;
	}
}