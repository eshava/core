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
		private static readonly Type _typeEnum = typeof(Enum);
		private static readonly Type _typeIList = typeof(IList);
		private static readonly ConstantExpression _constantExpressionStringNull = Expression.Constant(null, _typeString);
		private static readonly ConstantExpression _constantExpressionObjectNull = Expression.Constant(null, _typeObject);
		private static readonly MethodInfo _methodInfoStringContains = _typeString.GetMethod("Contains", new[] { _typeString });
		private static readonly MethodInfo _methodInfoStringStartsWith = _typeString.GetMethod("StartsWith", new[] { _typeString });
		private static readonly MethodInfo _methodInfoStringEndsWith = _typeString.GetMethod("EndsWith", new[] { _typeString });
		private static readonly ConstantExpression _constantExpressionCompareTo = Expression.Constant(0, typeof(int));

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
			{ typeof(DateTime), GetConstantDateTime },
			{ typeof(Enum), GetConstantEnum }
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
			{ CompareOperator.ContainsNot, GetContainsNotExpression },
			{ CompareOperator.StartsWith, GetStartsWithExpression },
			{ CompareOperator.EndsWith, GetEndsWithExpression },
			{ CompareOperator.ContainedIn, GetContainedInExpression },
		};

		private static readonly Dictionary<CompareOperator, ExpressionType> _compareOperatorExpressionType = new Dictionary<CompareOperator, ExpressionType>
		{
			{ CompareOperator.GreaterThan, ExpressionType.GreaterThan },
			{ CompareOperator.GreaterThanOrEqual, ExpressionType.GreaterThanOrEqual },
			{ CompareOperator.LessThan, ExpressionType.LessThan },
			{ CompareOperator.LessThanOrEqual, ExpressionType.LessThanOrEqual }
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
				var conditions = new List<Expression<Func<T, bool>>>();

				if (queryContainer.Mappings.ContainsKey(property.PropertyName))
				{
					var members = queryContainer.Mappings[property.PropertyName].Select(m => JoinAndExpressions(GetMappingCondition(property, m))).Where(e => e != null).ToList();

					if (members.Count == 1)
					{
						conditions.Add(members.Single());
					}
					else if (members.Count > 1)
					{
						conditions.Add(JoinOrExpressions(members));
					}
				}
				else
				{
					conditions.AddRange(GetPropertyCondition<T>(property, queryContainer.PropertyInfos, queryContainer.Parameter));
				}

				if (conditions.Any())
				{
					where.AddRange(conditions);
				}
			}

			return where;
		}

		private Expression<Func<T, bool>> BuildGlobalQueryCondition<T>(BuildQueryContainer<T> queryContainer) where T : class
		{
			var where = new List<Expression<Func<T, bool>>>();

			var searchTermParts = new[] { queryContainer.QueryParameters.SearchTerm };
			if (_options.ContainsSearchSplitBySpace)
			{
				searchTermParts = queryContainer.QueryParameters.SearchTerm.Split(' ').Where(t => !t.IsNullOrEmpty()).ToArray();
			}

			var andExpressions = new List<Expression<Func<T, bool>>>();
			foreach (var searchTermPart in searchTermParts)
			{
				var property = new WhereQueryProperty { Operator = CompareOperator.Contains, SearchTerm = searchTermPart };
				var orExpressions = new List<Expression<Func<T, bool>>>();

				foreach (var propertyInfo in queryContainer.PropertyInfos)
				{
					property.PropertyName = propertyInfo.Name;

					if (queryContainer.Mappings.ContainsKey(propertyInfo.Name))
					{
						orExpressions.AddRange(queryContainer.Mappings[propertyInfo.Name].SelectMany(m => GetMappingCondition(property, m, _typeString)).Where(e => e != null).ToList());
					}
					else if (propertyInfo.PropertyType == _typeString && propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() == null)
					{
						var conditions = GetPropertyCondition<T>(property, queryContainer.PropertyInfos, queryContainer.Parameter);
						if (conditions.Any())
						{
							orExpressions.AddRange(conditions);
						}
					}
				}

				if (orExpressions.Count > 0)
				{
					andExpressions.Add(JoinOrExpressions(orExpressions));
				}
			}

			return JoinAndExpressions(andExpressions);
		}

		private IList<Expression<Func<T, bool>>> GetMappingCondition<T>(WhereQueryProperty property, Expression<Func<T, object>> mappingExpression, Type expectedDataType = null) where T : class
		{
			var memberExpression = GetMemberExpression(mappingExpression);

			if (memberExpression == null || (expectedDataType != null && memberExpression.Type != expectedDataType))
			{
				return Array.Empty<Expression<Func<T, bool>>>();
			}

			var memberType = memberExpression.Type;
			if (memberType.ImplementsIEnumerable())
			{
				memberType = memberType.GetDataTypeFromIEnumerable();
			}

			var expression = new List<Expression<Func<T, bool>>>();
			var searchTermParts = new[] { property.SearchTerm };
			if (_options.ContainsSearchSplitBySpace && memberType == _typeString && property.Operator == CompareOperator.Contains)
			{
				searchTermParts = property.SearchTerm.Split(' ').Where(t => !t.IsNullOrEmpty()).ToArray();
			}

			foreach (var searchTermPart in searchTermParts)
			{
				var data = new ExpressionDataContainer
				{
					Member = memberExpression,
					Parameter = mappingExpression.Parameters.First(),
					Operator = property.Operator,
					ConstantValue = _constantExpressions[memberType.GetDataType()](searchTermPart, memberType, property.Operator, _options)
				};

				expression.Add(GetConditionComparableByMemberExpression<T>(data));
			}

			return expression;
		}

		private IEnumerable<Expression<Func<T, bool>>> GetPropertyCondition<T>(WhereQueryProperty property, IEnumerable<PropertyInfo> propertyInfos, ParameterExpression parameterExpression) where T : class
		{
			var propertyInfo = propertyInfos.SingleOrDefault(p => p.Name.Equals(property.PropertyName));
			if (propertyInfo == null || propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
			{
				return Array.Empty<Expression<Func<T, bool>>>();
			}

			var propertyType = propertyInfo.PropertyType;
			if (propertyInfo.PropertyType.ImplementsIEnumerable())
			{
				propertyType = propertyInfo.PropertyType.GetDataTypeFromIEnumerable();
			}

			var expression = new List<Expression<Func<T, bool>>>();
			var searchTermParts = new[] { property.SearchTerm };
			if (_options.ContainsSearchSplitBySpace && propertyType == _typeString && property.Operator == CompareOperator.Contains)
			{
				searchTermParts = property.SearchTerm.Split(' ').Where(t => !t.IsNullOrEmpty()).ToArray();
			}

			foreach (var searchTermPart in searchTermParts)
			{
				var dataType = propertyType.GetDataType();
				if (dataType.IsEnum)
				{
					dataType = _typeEnum;
				}

				var data = new ExpressionDataContainer
				{
					PropertyInfo = propertyInfo,
					Parameter = parameterExpression,
					Operator = property.Operator,
					ConstantValue = _constantExpressions[dataType](searchTermPart, propertyType, property.Operator, _options)
				};

				var condition = GetConditionComparableByProperty<T>(data);
				if (condition != null)
				{
					expression.Add(condition);
				}
			}

			return expression;
		}

		private static ConstantExpression GetConstantGuid(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<Guid>();
				foreach (var item in value.Split('|'))
				{
					if (Guid.TryParse(item, out var valueItemGuid))
					{
						values.Add(valueItemGuid);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Guid.TryParse(value, out var valueGuid))
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

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = value.Split('|').ToList();

				return Expression.Constant(values, values.GetType());
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
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<decimal>();
				foreach (var item in value.Split('|'))
				{
					if (Decimal.TryParse(item, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueItemDecimal))
					{
						values.Add(valueItemDecimal);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueDecimal))
			{
				return null;
			}

			return Expression.Constant(valueDecimal, dataType);
		}

		private static ConstantExpression GetConstantDouble(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<double>();
				foreach (var item in value.Split('|'))
				{
					if (Double.TryParse(item, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueItemDouble))
					{
						values.Add(valueItemDouble);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var valueDouble))
			{
				return null;
			}

			return Expression.Constant(valueDouble, dataType);
		}

		private static ConstantExpression GetConstantFloat(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<float>();
				foreach (var item in value.Split('|'))
				{
					if (Single.TryParse(item, NumberStyles.Float, CultureInfo.InvariantCulture, out var valueItemFloat))
					{
						values.Add(valueItemFloat);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Single.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var valueFloat))
			{
				return null;
			}

			return Expression.Constant(valueFloat, dataType);
		}

		private static ConstantExpression GetConstantInteger(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<int>();
				foreach (var item in value.Split('|'))
				{
					if (Int32.TryParse(item, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueItemInteger))
					{
						values.Add(valueItemInteger);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueInt))
			{
				return null;
			}

			return Expression.Constant(valueInt, dataType);
		}

		private static ConstantExpression GetConstantLong(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<long>();
				foreach (var item in value.Split('|'))
				{
					if (Int64.TryParse(item, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueItemLong))
					{
						values.Add(valueItemLong);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!Int64.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valueLong))
			{
				return null;
			}

			return Expression.Constant(valueLong, dataType);
		}

		private static ConstantExpression GetConstantEnum(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = value.Split('|').Select(v => Enum.Parse(dataType, v)).ToList();
				try
				{
					return Expression.Constant(values, values.GetType());
				}
				catch
				{
					return null;
				}
			}

			try
			{
				return Expression.Constant(Enum.Parse(dataType, value), dataType);
			}
			catch
			{
				return null;
			}
		}

		private static ConstantExpression GetConstantDateTime(string value, Type dataType, CompareOperator compareOperator, WhereQueryEngineOptions options)
		{
			if (value.IsNullOrEmpty() || compareOperator == CompareOperator.None)
			{
				return null;
			}

			if (compareOperator == CompareOperator.ContainedIn)
			{
				var values = new List<DateTime>();
				foreach (var item in value.Split('|'))
				{
					if (TryParseDateTime(item, options, out var valueItemDateTime))
					{
						values.Add(valueItemDateTime);
					}
				}

				return Expression.Constant(values, values.GetType());
			}

			if (!TryParseDateTime(value, options, out var valueDateTime))
			{
				return null;
			}

			return Expression.Constant(valueDateTime, dataType);
		}

		private static bool TryParseDateTime(string value, WhereQueryEngineOptions options, out DateTime result)
		{
			if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture, DateTimeStyles.None, out result)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture, DateTimeStyles.None, out result)
				|| DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
			{

				if (options.UseUtcDateTime)
				{
					result = result.ToUniversalTime();
				}

				return true;
			}

			return false;
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

			if (data.Member.Type.IsEnum && _compareOperatorExpressionType.ContainsKey(data.Operator))
			{
				var enumCompareToExpression = Expression.Call(data.Member, data.Member.Type.GetMethod("CompareTo", new[] { data.Member.Type }), Expression.Convert(data.ConstantValue, _typeObject));
				var binaryExpression = Expression.MakeBinary(_compareOperatorExpressionType[data.Operator], enumCompareToExpression, _constantExpressionCompareTo);

				return Expression.Lambda<Func<T, bool>>(binaryExpression, data.Parameter);
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

		private Expression<Func<T, bool>> JoinAndExpressions<T>(IList<Expression<Func<T, bool>>> where) where T : class
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
					joinedExpressions = Expression.And(previousCondition, currentCondition);
				}
				else
				{
					joinedExpressions = Expression.And(joinedExpressions, currentCondition);
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

		private static Expression GetContainsNotExpression(MemberExpression member, ConstantExpression constant)
		{
			if (member.Type == _typeString)
			{
				return Expression.OrElse(Expression.Equal(member, _constantExpressionStringNull), Expression.Not(Expression.Call(member, _methodInfoStringContains, constant)));
			}

			if (member.Type.ImplementsIEnumerable() && member.Type.ImplementsInterface(_typeIList))
			{
				var genericType = member.Type.GetDataTypeFromIEnumerable();
				var nullCheckExpression = Expression.Equal(member, _constantExpressionObjectNull);
				var enumerableContainsExpression = Expression.Call(member, member.Type.GetMethod("Contains", new[] { genericType }), constant);

				return Expression.OrElse(nullCheckExpression, Expression.Not(enumerableContainsExpression));
			}

			throw new NotSupportedException("The data type of the property has to be of type string or must implement 'IList'");
		}

		private static Expression GetStartsWithExpression(MemberExpression member, ConstantExpression constant)
		{
			if (member.Type == _typeString)
			{
				return Expression.AndAlso(Expression.NotEqual(member, _constantExpressionStringNull), Expression.Call(member, _methodInfoStringStartsWith, constant));
			}

			throw new NotSupportedException("The data type of the property has to be of type string");
		}

		private static Expression GetEndsWithExpression(MemberExpression member, ConstantExpression constant)
		{
			if (member.Type == _typeString)
			{
				return Expression.AndAlso(Expression.NotEqual(member, _constantExpressionStringNull), Expression.Call(member, _methodInfoStringEndsWith, constant));
			}

			throw new NotSupportedException("The data type of the property has to be of type string");
		}

		private static Expression GetContainedInExpression(MemberExpression member, ConstantExpression constant)
		{
			var enumerableMemberMethod = constant.Type.GetMethod("Contains", new[] { member.Type });
			Expression memberExpression;

			if (member.Type.IsEnum)
			{
				memberExpression = Expression.Convert(member, _typeObject);
			}
			else
			{
				memberExpression = member;
			}

			return Expression.Call(constant, enumerableMemberMethod, memberExpression);
		}
	}
}