using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Eshava.Core.Linq.Models;
using Eshava.Core.Models;

namespace Eshava.Core.Linq.Interfaces
{
	public interface IWhereQueryEngine
	{
		/// <summary>
		///  Creates a list of where expression based on passed filter object
		///  Hint: Only properties of type <see cref="FilterField"/> are considered
		/// </summary>
		/// <typeparam name="T">Target class data type</typeparam>
		/// <param name="filter">Filter object</param>
		/// <param name="globalSearchTerm">Search termn, will apply on all string properties</param>
		/// <param name="mappings">Mappings for foreign key properties</param>
		/// <returns></returns>
		ResponseData<IEnumerable<Expression<Func<T, bool>>>> BuildQueryExpressions<T>(object filter, string globalSearchTerm, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null) where T : class;

		/// <summary>
		/// Creates a list of where expression based on passed query parameters
		/// </summary>
		/// <remarks>
		/// Global search term is only supported for string properties in the data type class <see cref="T">T</see>.
		/// String properties in sub classes or enumerable string properties are not supported.
		/// </remarks>
		/// <typeparam name="T">Target class data type</typeparam>
		/// <param name="queryParameters">queryParameters</param>
		/// <param name="mappings">Mappings for foreign key properties</param>
		/// <exception cref="ArgumentNullException">Thrown if <see cref="QueryParameters">queryParameters</see> is null.</exception>
		/// <returns> where expressions</returns>
		ResponseData<IEnumerable<Expression<Func<T, bool>>>> BuildQueryExpressions<T>(QueryParameters queryParameters, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null) where T : class;

		/// <summary>
		/// Creates a list of where expression based on passed where query properties and search term
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="whereQueryProperties">filter statements</param>
		/// <param name="globalSearchTerm">Search termn, will apply on all string properties</param>
		/// <param name="mappings"></param>
		/// <returns></returns>
		ResponseData<IEnumerable<Expression<Func<T, bool>>>> BuildQueryExpressions<T>(IEnumerable<WhereQueryProperty> whereQueryProperties, string globalSearchTerm, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null) where T : class;

		/// <summary>
		/// Removes all properties for which a GUID search term was passed
		/// </summary>
		/// <typeparam name="T">Target class data type</typeparam>
		/// <param name="queryProperties">Properties which contains search term</param>
		/// <param name="mappings">Property mappings for navigation to a subproperty</param>
		/// <returns></returns>
		IWhereQueryEngine RemovePropertyMappings<T>(IEnumerable<WhereQueryProperty> queryProperties, Dictionary<string, List<Expression<Func<T, object>>>> mappings) where T : class;
	}
}