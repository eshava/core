using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Eshava.Core.Linq.Models
{
	internal class BuildQueryContainer<T> where T : class
	{
		public QueryParameters QueryParameters { get; set; }
		public PropertyInfo[] PropertyInfos { get; set; }
		public Dictionary<string, List<Expression<Func<T, object>>>> Mappings { get; set; }
		public ParameterExpression Parameter { get; set; }
	}
}