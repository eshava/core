using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Linq.Attributes;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Interfaces;
using Eshava.Core.Linq.Models;

namespace Eshava.Core.Linq
{
	public class WhereQueryEngine : AbstractQueryEngine, IWhereQueryEngine
	{
		private static readonly Type _typeString = typeof(string);
		private static readonly Type _typeObject = typeof(object);
		private static readonly Type _typeIList = typeof(IList);
		private static readonly ConstantExpression _constantExpressionStringNull = Expression.Constant(null, _typeString);
		private static readonly ConstantExpression _constantExpressionObjectNull = Expression.Constant(null, _typeObject);
		private static readonly MethodInfo _methodInfoStringContains = _typeString.GetMethod("Contains", new[] { _typeString });

		private static readonly Dictionary<Type, Func<string, Type, CompareOperator, WhereQueryEngineOptions, ConstantExpression>> _constantExpressions = new Dictionary<Type, Func<string, Type, CompareOperator, WhereQueryEngineOptions, ConstantExpression>>
		{
			{ typeof(Guid), GetConstantGuid },
			{ typeof(string), GetConstantString },
			{ typeof(bool), GetConstantBoolean },
			{ typeof(int), GetConstantInteger },
			{ typeof(long), GetConstantLong },
			{ typeof(decimal), GetConstantDecimal },
			{ typeof(double), GetConstantDouble },
			{ typeof(float), GetConstantFloat },
			{ typeof(DateTime), GetConstantDateTime }
		};

		private static readonly Dictionary<CompareOperator, Func<MemberExpression, ConstantExpression, Expression>> _compareOperatorExpressions = new Dictionary<CompareOperator, Func<MemberExpression, ConstantExpression, Expression>>
		{
			{ CompareOperator.Equal, Expression.Equal },
			{ CompareOperator.NotEqual, Expression.NotEqual },
			{ CompareOperator.GreaterThan, Expression.GreaterThan },
			{ CompareOperator.GreaterThanOrEqual, Expression.GreaterThanOrEqual },
			{ CompareOperator.LessThan, Expression.LessThan },
			{ CompareOperator.LessThanOrEqual, Expression.LessThanOrEqual },
			{ CompareOperator.Contains, GetContainsExpression },
		};

		private readonly WhereQueryEngineOptions _options;

		public WhereQueryEngine(WhereQueryEngineOptions options)
		{
			_options = options;
		}

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
		public IEnumerable<Expression<Func<T, bool>>> BuildQueryExpressions<T>(QueryParameters queryParameters, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null) where T : class
		{
			if (queryParameters == null)
			{
				throw new ArgumentNullException(nameof(queryParameters));
			}

			var where = new List<Expression<Func<T, bool>>>();
			var hasPropertyQueries = queryParameters.WhereQueryProperties?.Any() ?? false;
			var hasGlobalSearchTerm = !queryParameters.SearchTerm.IsNullOrEmpty();

			if (!hasPropertyQueries && !hasGlobalSearchTerm)
			{
				return where;
			}

			if (mappings == null)
			{
				mappings = new Dictionary<string, List<Expression<Func<T, object>>>>();
			}

			var type = typeof(T);
			var queryContainer = new BuildQueryContainer<T>
			{
				Mappings = mappings,
				Parameter = Expression.Parameter(type, "p"),
				PropertyInfos = type.GetProperties(),
				QueryParameters = queryParameters
			};

			if (hasPropertyQueries)
			{
				where.AddRange(BuildPropertyQueryConditions(queryContainer));
			}

			if (hasGlobalSearchTerm)
			{
				var condition = BuildGlobalQueryCondition(queryContainer);
				if (condition != null)
				{
					where.Add(condition);
				}
			}

			return where;
		}

		/// <summary>
		/// Removes all properties for which a GUID search term was passed and set the operator to equal
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="queryProperties">Properties which contains search term</param>
		/// <param name="mappings">Property mappings for navigation to a subproperty</param>
		/// <returns></returns>
		public IWhereQueryEngine RemovePropertyMappings<T>(IEnumerable<WhereQueryProperty> queryProperties, Dictionary<string, List<Expression<Func<T, object>>>> mappings) where T : class
		{
			if (mappings == null)
			{
				return this;
			}

			foreach (var queryProperty in queryProperties)
			{
				if (mappings.ContainsKey(queryProperty.PropertyName) && Guid.TryParse(queryProperty.SearchTerm, out var _))
				{
					// The equals operator is the only working operator
					queryProperty.Operator = CompareOperator.Equal;
					mappings.Remove(queryProperty.PropertyName);
				}
			}

			return this;
		}
		
		private IEnumerable<Expression<Func<T, bool>>> BuildPropertyQueryConditions<T>(BuildQueryContainer<T> queryContainer) where T : class
		{
			var where = new List<Expression<Func<T, bool>>>();

			foreach (var property in queryContainer.QueryParameters.WhereQueryProperties)
			{
				Expression<Func<T, bool>> condition = null;

				if (queryContainer.Mappings.ContainsKey(property.PropertyName))
				{
					var members = queryContainer.Mappings[property.PropertyName].Select(m => GetMappingCondition(property, m)).Where(e => e != null).ToList();

					if (members.Count == 1)
					{
						condition = members.Single();
					}
					else if (members.Count > 1)
					{
						condition = JoinOrExpressions(members);
					}
				}
				else
				{
					condition = GetPropertyCondition<T>(property, queryContainer.PropertyInfos, queryContainer.Parameter);
				}

				if (condition != null)
				{
					where.Add(condition);
				}
			}

			return where;
		}

		private Expression<Func<T, bool>> BuildGlobalQueryCondition<T>(BuildQueryContainer<T> queryContainer) where T : class
		{
			var where = new List<Expression<Func<T, bool>>>();
			var property = new WhereQueryProperty { Operator = CompareOperator.Contains, SearchTerm = queryContainer.QueryParameters.SearchTerm };

			foreach (var propertyInfo in queryContainer.PropertyInfos)
			{
				property.PropertyName = propertyInfo.Name;

				if (queryContainer.Mappings.ContainsKey(propertyInfo.Name))
				{
					where.AddRange(queryContainer.Mappings[propertyInfo.Name].Select(m => GetMappingCondition(property, m, _typeString)).Where(e => e != null).ToList());
				}
				else if (propertyInfo.PropertyType == _typeString && propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() == null)
				{
					var condition = GetPropertyCondition<T>(property, queryContainer.PropertyInfos, queryContainer.Parameter);

					if (condition != null)
					{
						where.Add(condition);
					}
				}
			}

			return where.Any() ? JoinOrExpressions(where) : null;
		}

		private Expression<Func<T, bool>> GetMappingCondition<T>(WhereQueryProperty property, Expression<Func<T, object>> mappingExpression, Type expectedDataType = null) where T : class
		{
			var memberExpression = GetMemberExpression(mappingExpression);

			if (memberExpression == null || (expectedDataType != null && memberExpression.Type != expectedDataType))
			{
				return null;
			}

			var memberType = memberExpression.Type;
			if (memberType.ImplementsIEnumerable())
			{
				memberType = memberType.GetDataTypeFromIEnumerable();
			}

			var data = new ExpressionDataContainer
			{
				Member = memberExpression,
				Parameter = mappingExpression.Parameters.First(),
				Operator = property.Operator,
				ConstantValue = _constantExpressions[memberType.GetDataType()](property.SearchTerm, memberType, property.Operator, _options)
			};

			return GetConditionComparableByMemberExpression<T>(data);
		}

		private Expression<Func<T, bool>> GetPropertyCondition<T>(WhereQueryProperty property, IEnumerable<PropertyInfo> propertyInfos, ParameterExpression parameterExpression) where T : class
		{
			var propertyInfo = propertyInfos.SingleOrDefault(p => p.Name.Equals(property.PropertyName));
			if (propertyInfo == null || propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
			{
				return null;
			}

			var propertyType = propertyInfo.PropertyType;
			if (propertyInfo.PropertyType.ImplementsIEnumerable())
			{
				propertyType = propertyInfo.PropertyType.GetDataTypeFromIEnumerable();
			}

			var data = new ExpressionDataContainer
			{
				PropertyInfo = propertyInfo,
				Parameter = parameterExpression,
				Operator = property.Operator,
				ConstantValue = _constantExpressions[propertyType.GetDataType()](property.SearchTerm, propertyType, property.Operator, _options)
			};

			return GetConditionComparableByProperty<T>(data);
		}

		private static ConstantExpression GetConstantGuid(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (!Guid.TryParse(value, out var valueGuid) || compareOperator == CompareOperator.None)
			{
				return null;
			}

			return Expression.Constant(valueGuid, dataType);
		}

		private static ConstantExpression GetConstantString(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			return Expression.Constant(value, dataType);
		}

		private static ConstantExpression GetConstantBoolean(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			var boolean = value.ToBoolean();

			return Expression.Constant(boolean, dataType);
		}

		private static ConstantExpression GetConstantDecimal(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (!Decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueDecimal) || compareOperator == CompareOperator.None)
			{
				return null;
			}

			return Expression.Constant(valueDecimal, dataType);
		}

		private static ConstantExpression GetConstantDouble(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (!Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueDouble) || compareOperator == CompareOperator.None)
			{
				return null;
			}

			return Expression.Constant(valueDouble, dataType);
		}

		private static ConstantExpression GetConstantFloat(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (!Single.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var valueFloat) || compareOperator == CompareOperator.None)
			{
				return null;
			}

			return Expression.Constant(valueFloat, dataType);
		}

		private static ConstantExpression GetConstantInteger(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (!Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueInt) || compareOperator == CompareOperator.None)
			{
				return null;
			}

			return Expression.Constant(valueInt, dataType);
		}

		private static ConstantExpression GetConstantLong(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (!Int64.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueLong) || compareOperator == CompareOperator.None)
			{
				return null;
			}

			return Expression.Constant(valueLong, dataType);
		}

		private static ConstantExpression GetConstantDateTime(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (compareOperator == CompareOperator.None)
			{
				return null;
			}

			DateTime valueDateTime;
			if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture, DateTimeStyles.None, out valueDateTime)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out valueDateTime)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out valueDateTime)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture, DateTimeStyles.None, out valueDateTime)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out valueDateTime))
			{
				if (options.UseUtcDateTime)
				{
					valueDateTime = valueDateTime.ToUniversalTime();
				}

				return Expression.Constant(valueDateTime, dataType);
			}

			return null;
		}

		private Expression<Func<T, bool>> GetConditionComparableByProperty<T>(ExpressionDataContainer data) where T : class
		{
			data.Member = Expression.MakeMemberAccess(data.Parameter, data.PropertyInfo);

			return GetConditionComparableByMemberExpression<T>(data);
		}

		private Expression<Func<T, bool>> GetConditionComparableByMemberExpression<T>(ExpressionDataContainer data) where T : class
		{
			if (data.ConstantValue == null || !_compareOperatorExpressions.ContainsKey(data.Operator))
			{
				return null;
			}

			var expression = _compareOperatorExpressions[data.Operator](data.Member, data.ConstantValue);

			return Expression.Lambda<Func<T, bool>>(expression, data.Parameter);
		}

		private Expression<Func<T, bool>> JoinOrExpressions<T>(IList<Expression<Func<T, bool>>> where) where T : class
		{
			if (where.Count <= 1)
			{
				return where.SingleOrDefault();
			}

			BinaryExpression joinedExpressions = null;
			var parameter = Expression.Parameter(typeof(T), "p");

			for (var index = 1; index < where.Count; index++)
			{
				var currentCondition = ChangeParameterExpression(where[index], parameter);

				if (joinedExpressions == null)
				{
					var previousCondition = ChangeParameterExpression(where[index - 1], parameter);
					joinedExpressions = Expression.OrElse(previousCondition, currentCondition);
				}
				else
				{
					joinedExpressions = Expression.OrElse(joinedExpressions, currentCondition);
				}
			}

			return Expression.Lambda<Func<T, bool>>(joinedExpressions, parameter);
		}

		private Expression ChangeParameterExpression<T>(Expression<Func<T, bool>> condition, ParameterExpression parameter)
		{
			var previousVisitor = new ReplaceExpressionVisitor(condition.Parameters.First(), parameter);

			return previousVisitor.Visit(condition.Body);
		}

		private static Expression GetContainsExpression(MemberExpression member, ConstantExpression constant)
		{
			if (member.Type == _typeString)
			{
				return Expression.AndAlso(Expression.NotEqual(member, _constantExpressionStringNull), Expression.Call(member, _methodInfoStringContains, constant));
			}

			if (member.Type.ImplementsIEnumerable() && member.Type.ImplementsInterface(_typeIList))
			{
				var genericType = member.Type.GetDataTypeFromIEnumerable();
				var nullCheckExpression = Expression.NotEqual(member, _constantExpressionObjectNull);
				var enumerableContainsExpression = Expression.Call(member, member.Type.GetMethod("Contains", new[] { genericType }), constant);

				return Expression.AndAlso(nullCheckExpression, enumerableContainsExpression);
			}

			throw new NotSupportedException("The data type of the property has to be of type string or must implement 'IList'");
		}
	}
}