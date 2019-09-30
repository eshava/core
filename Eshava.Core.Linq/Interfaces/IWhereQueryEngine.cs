using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Eshava.Core.Linq.Models;

namespace Eshava.Core.Linq.Interfaces
{
	public interface IWhereQueryEngine
	{
		/// <summary>
		/// Creates a list of where expression based on passed query parameters
		/// </summary>
		/// <remarks>
		/// Global search term is only supported for string properties in the data type class <see cref="T">T</see>.
		/// String properties in sub classes or enumerable string properties are not supported.
		/// </remarks>
		/// <typeparam name="T">Class data type</typeparam>
		/// <param name="queryParameters">queryParameters</param>
		/// <param name="mappings">Mappings for foreign key properties</param>
		/// <exception cref="ArgumentNullException">Thrown if <see cref="QueryParameters">queryParameters</see> is null.</exception>
		/// <returns> where expressions</returns>
		IEnumerable<Expression<Func<T, bool>>> BuildQueryExpressions<T>(QueryParameters queryParameters, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null) where T : class;

		/// <summary>
		/// Removes all properties for which a GUID search term was passed
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="queryProperties">Properties which contains search term</param>
		/// <param name="mappings">Property mappings for navigation to a subproperty</param>
		/// <returns></returns>
		IWhereQueryEngine RemovePropertyMappings<T>(IEnumerable<WhereQueryProperty> queryProperties, Dictionary<string, List<Expression<Func<T, object>>>> mappings) where T : class;
	}
}