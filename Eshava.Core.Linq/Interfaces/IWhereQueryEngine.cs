using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Eshava.Core.Linq.Models;

namespace Eshava.Core.Linq.Interfaces
{
	public interface IWhereQueryEngine
	{
		IEnumerable<Expression<Func<T, bool>>> BuildQueryExpressions<T>(QueryParameters queryParameters, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null) where T : class;
	}
}