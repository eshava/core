using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Linq.Attributes;
using Eshava.Core.Linq.Constants;
using Eshava.Core.Linq.Enums;
using Eshava.Core.Linq.Extensions;
using Eshava.Core.Linq.Interfaces;
using Eshava.Core.Linq.Models;
using Eshava.Core.Models;

namespace Eshava.Core.Linq
{
	public class WhereQueryEngine : AbstractQueryEngine, IWhereQueryEngine
	{
		private readonly WhereQueryEngineOptions _globalOptions;

		public WhereQueryEngine(WhereQueryEngineOptions options)
		{
			_globalOptions = options;
		}

		/// <summary>
		///  Creates a list of where expression based on passed filter object
		///  Hint: Only properties of type <see cref="FilterField"/> are considered
		/// </summary>
		/// <typeparam name="T">Target class data type</typeparam>
		/// <param name="filter">Filter object</param>
		/// <param name="globalSearchTerm">Search termn, will apply on all string properties</param>
		/// <param name="mappings">Mappings for foreign key properties</param>
		/// <returns></returns>
		public ResponseData<IEnumerable<Expression<Func<T, bool>>>> BuildQueryExpressions<T>(object filter, string globalSearchTerm, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null, WhereQueryEngineOptions options = null) where T : class
		{
			options = PrepareOptions(options, _globalOptions);

			if (filter == null)
			{
				return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDINPUT);
			}

			var whereQueryProperties = new List<WhereQueryProperty>();
			var invalidFilterFields = new List<ValidationError>();

			foreach (var filterField in filter.GetType().GetProperties())
			{
				if (filterField.PropertyType != TypeConstants.FilterField && !filterField.PropertyType.IsSubclassOf(TypeConstants.FilterField))
				{
					continue;
				}

				var completeField = filterField.GetValue(filter) as ComplexFilterField;
				var field = filterField.GetValue(filter) as FilterField;
				if (completeField == null && field == null)
				{
					continue;
				}

				var allowedCompareOperators = filterField.GetCustomAttributes<AllowedCompareOperatorAttribute>()?.Select(attribute => attribute.CompareOperator).ToList();
				var allowedFitlerFields = filterField.GetCustomAttributes<AllowedComplexFilterFieldAttribute>()?.Select(attribute => attribute.Field).ToList();

				var allowedCompareOperatorsHashSet = allowedCompareOperators == null
					? new HashSet<CompareOperator>()
					: new HashSet<CompareOperator>(allowedCompareOperators);

				var allowedFitlerFieldsHashSet = allowedFitlerFields == null
					? new HashSet<string>()
					: new HashSet<string>(allowedFitlerFields);

				if (completeField == null)
				{
					completeField = new ComplexFilterField
					{
						Field = filterField.Name,
						Operator = field.Operator,
						SearchTerm = field.SearchTerm,
						LinkOperator = LinkOperator.None
					};

					allowedFitlerFieldsHashSet.Add(filterField.Name);
				}

				var whereQueryProperty = ConvertFilterFieldToWhereQueryProperty(completeField, invalidFilterFields, allowedCompareOperatorsHashSet, allowedFitlerFieldsHashSet, options);
				if (whereQueryProperty == null)
				{
					continue;
				}

				whereQueryProperties.Add(whereQueryProperty);
			}

			if (invalidFilterFields.Count > 0)
			{
				return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDFILTER, validationErrors: invalidFilterFields);
			}

			return BuildQueryExpressions(whereQueryProperties, globalSearchTerm, mappings, options);
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
		public ResponseData<IEnumerable<Expression<Func<T, bool>>>> BuildQueryExpressions<T>(QueryParameters queryParameters, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null, WhereQueryEngineOptions options = null) where T : class
		{
			if (queryParameters == null)
			{
				return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDINPUT);
			}

			var where = new List<Expression<Func<T, bool>>>();
			var hasPropertyQueries = queryParameters.WhereQueryProperties?.Any() ?? false;
			var hasGlobalSearchTerm = !queryParameters.SearchTerm.IsNullOrEmpty();

			if (!hasPropertyQueries && !hasGlobalSearchTerm)
			{
				return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(where);
			}

			return BuildQueryExpressions(queryParameters.WhereQueryProperties, queryParameters.SearchTerm, mappings, options);
		}



		/// <summary>
		/// Creates a list of where expression based on passed where query properties and search term
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="whereQueryProperties">filter statements</param>
		/// <param name="globalSearchTerm">Search termn, will apply on all string properties</param>
		/// <param name="mappings"></param>
		/// <returns></returns>
		public ResponseData<IEnumerable<Expression<Func<T, bool>>>> BuildQueryExpressions<T>(IEnumerable<WhereQueryProperty> whereQueryProperties, string globalSearchTerm, Dictionary<string, List<Expression<Func<T, object>>>> mappings = null, WhereQueryEngineOptions options = null) where T : class
		{
			options = PrepareOptions(options, _globalOptions);

			var where = new List<Expression<Func<T, bool>>>();
			var hasPropertyQueries = whereQueryProperties?.Any() ?? false;
			var hasGlobalSearchTerm = !globalSearchTerm.IsNullOrEmpty();

			if (!hasPropertyQueries && !hasGlobalSearchTerm)
			{
				return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(where);
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
				GlobalSearchTerm = globalSearchTerm,
				WhereQueryProperties = whereQueryProperties ?? new List<WhereQueryProperty>(),
				Options = options
			};

			if (hasPropertyQueries)
			{
				var conditionResult = BuildPropertyQueryConditions(queryContainer);
				if (conditionResult.IsFaulty)
				{
					return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(conditionResult);
				}

				if (conditionResult.Data?.Any() ?? false)
				{
					where.AddRange(conditionResult.Data);
				}
			}

			if (hasGlobalSearchTerm)
			{
				var conditionResult = BuildGlobalQueryCondition(queryContainer);
				if (conditionResult.IsFaulty)
				{
					return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(conditionResult);
				}

				if (conditionResult.Data != null)
				{
					where.Add(conditionResult.Data);
				}
			}

			return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(where);
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

		private ResponseData<IEnumerable<Expression<Func<T, bool>>>> BuildPropertyQueryConditions<T>(BuildQueryContainer<T> queryContainer) where T : class
		{
			var where = new List<Expression<Func<T, bool>>>();

			foreach (var property in queryContainer.WhereQueryProperties)
			{
				var propertyResult = BuildPropertyQueryConditions(property, queryContainer);
				if (propertyResult.IsFaulty)
				{
					return propertyResult;
				}

				if (propertyResult.Data.Any())
				{
					where.AddRange(propertyResult.Data);
				}
			}

			return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(where);
		}

		private ResponseData<IEnumerable<Expression<Func<T, bool>>>> BuildPropertyQueryConditions<T>(WhereQueryProperty property, BuildQueryContainer<T> queryContainer) where T : class
		{
			var where = new List<Expression<Func<T, bool>>>();

			// operator overrules link operator
			if (property.Operator != CompareOperator.None)
			{
				// process field property
				var propertyResult = ProcessProperty(property, queryContainer);
				if (propertyResult.IsFaulty)
				{
					return propertyResult;
				}

				if (propertyResult.Data.Any())
				{
					where.AddRange(propertyResult.Data);
				}
			}
			else if (property.LinkOperator == LinkOperator.None)
			{
				// invalid property 
				if (queryContainer.Options.SkipInvalidWhereQueries ?? false)
				{
					return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(new List<Expression<Func<T, bool>>>());
				}

				return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDLINKOPERATOR);
			}
			else if (!(property.LinkOperations?.Any() ?? false))
			{
				// invalid property 
				if (queryContainer.Options.SkipInvalidWhereQueries ?? false)
				{
					return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(new List<Expression<Func<T, bool>>>());
				}

				return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.LINKOPERATIONSREQUIRED);
			}
			else
			{
				// process group property
				var groupConditions = new List<Expression<Func<T, bool>>>();
				foreach (var linkOperationProperty in property.LinkOperations)
				{
					var propertyResult = BuildPropertyQueryConditions(linkOperationProperty, queryContainer);
					if (propertyResult.IsFaulty)
					{
						return propertyResult;
					}

					groupConditions.AddRange(propertyResult.Data);
				}

				if (groupConditions.Any())
				{
					Expression<Func<T, bool>> joinResult = null;
					switch (property.LinkOperator)
					{
						case LinkOperator.And:
							joinResult = JoinAndExpressions(groupConditions);
							break;
						case LinkOperator.Or:
							joinResult = JoinOrExpressions(groupConditions);
							break;
					}

					if (joinResult != null)
					{
						where.Add(joinResult);
					}
				}
			}

			return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(where);
		}

		private ResponseData<Expression<Func<T, bool>>> BuildGlobalQueryCondition<T>(BuildQueryContainer<T> queryContainer) where T : class
		{
			var where = new List<Expression<Func<T, bool>>>();
			var validationErrors = new List<ValidationError>();

			var searchTermParts = new[] { queryContainer.GlobalSearchTerm };
			if (queryContainer.Options.ContainsSearchSplitBySpace ?? false)
			{
				searchTermParts = queryContainer.GlobalSearchTerm.Split(' ').Where(t => !t.IsNullOrEmpty()).ToArray();
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

						foreach (var mapping in queryContainer.Mappings[propertyInfo.Name])
						{
							var mappingResult = GetMappingCondition(property, mapping, queryContainer.Options, TypeConstants.String);
							if (mappingResult.IsFaulty)
							{
								return ResponseData<Expression<Func<T, bool>>>.CreateFaultyResponse(mappingResult);
							}

							orExpressions.AddRange(mappingResult.Data);
						}
					}
					else if (propertyInfo.PropertyType == TypeConstants.String && propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() == null)
					{
						var conditionsResult = GetPropertyCondition<T>(property, queryContainer.PropertyInfos, queryContainer.Parameter, queryContainer.Options);
						if (conditionsResult.IsFaulty)
						{
							return ResponseData<Expression<Func<T, bool>>>.CreateFaultyResponse(conditionsResult);
						}

						if (conditionsResult.Data.Any())
						{
							orExpressions.AddRange(conditionsResult.Data);
						}
					}
				}

				foreach (var mappings in queryContainer.Mappings)
				{
					if (queryContainer.PropertyInfos.Any(propertyInfo => propertyInfo.Name == mappings.Key))
					{
						continue;
					}

					foreach (var mapping in mappings.Value)
					{
						var mappingResult = GetMappingCondition(property, mapping, queryContainer.Options, TypeConstants.String);
						if (mappingResult.IsFaulty)
						{
							return ResponseData<Expression<Func<T, bool>>>.CreateFaultyResponse(mappingResult);
						}

						orExpressions.AddRange(mappingResult.Data);
					}
				}

				if (orExpressions.Count > 0)
				{
					andExpressions.Add(JoinOrExpressions(orExpressions));
				}
			}

			return new ResponseData<Expression<Func<T, bool>>>
			{
				Data = JoinAndExpressions(andExpressions)
			};
		}

		private ResponseData<IList<Expression<Func<T, bool>>>> GetMappingCondition<T>(WhereQueryProperty property, Expression<Func<T, object>> mappingExpression, WhereQueryEngineOptions options, Type expectedDataType = null) where T : class
		{
			var memberExpression = GetMemberExpression(mappingExpression);
			if (memberExpression == null)
			{
				if (options.SkipInvalidWhereQueries ?? false)
				{
					return new ResponseData<IList<Expression<Func<T, bool>>>>(new List<Expression<Func<T, bool>>>());
				}

				return ResponseData<IList<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDDATA)
					.AddValidationError(property.PropertyName, MessageConstants.INVALIDPROPERTYMAPPING);
			}

			if (expectedDataType != null && memberExpression.Type != expectedDataType)
			{
				if (options.SkipInvalidWhereQueries ?? false)
				{
					return new ResponseData<IList<Expression<Func<T, bool>>>>(new List<Expression<Func<T, bool>>>());
				}

				return ResponseData<IList<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDDATA)
					.AddValidationError(property.PropertyName, MessageConstants.INVALIDPROPERTYMAPPINGTYPE);
			}

			var memberType = memberExpression.Type;
			if (memberType.ImplementsIEnumerable())
			{
				memberType = memberType.GetDataTypeFromIEnumerable();
			}

			var expressions = new List<Expression<Func<T, bool>>>();
			var searchTermParts = new[] { property.SearchTerm };
			if ((options.ContainsSearchSplitBySpace ?? false) && memberType == TypeConstants.String && property.Operator == CompareOperator.Contains)
			{
				searchTermParts = property.SearchTerm.Split(' ').Where(t => !t.IsNullOrEmpty()).ToArray();
			}

			var validationErrors = new List<ValidationError>();
			foreach (var searchTermPart in searchTermParts)
			{
				var dataType = memberType.GetDataType();
				if (dataType.IsEnum)
				{
					dataType = TypeConstants.Enum;
				}

				var data = new ExpressionDataContainer
				{
					Member = memberExpression,
					Parameter = mappingExpression.Parameters.First(),
					Operator = property.Operator,
					ConstantValue = dataType.GetConstantExpression(searchTermPart, memberType, property.Operator, options),
					Options = options
				};

				var expressionResult = GetConditionComparableByMemberExpression<T>(data);
				if (expressionResult.IsFaulty)
				{
					if (!(options.SkipInvalidWhereQueries ?? false))
					{
						validationErrors.AddRange(expressionResult.ValidationErrors);
					}
				}
				else
				{
					expressions.Add(expressionResult.Data);
				}
			}

			if (validationErrors.Count > 0)
			{
				return ResponseData<IList<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDDATA, validationErrors: validationErrors);
			}

			return new ResponseData<IList<Expression<Func<T, bool>>>>(expressions);
		}

		private ResponseData<IEnumerable<Expression<Func<T, bool>>>> GetPropertyCondition<T>(WhereQueryProperty property, IEnumerable<PropertyInfo> propertyInfos, ParameterExpression parameterExpression, WhereQueryEngineOptions options) where T : class
		{
			var propertyInfo = propertyInfos.SingleOrDefault(p => p.Name.Equals(property.PropertyName));
			if (propertyInfo == null)
			{
				if (options.SkipInvalidWhereQueries ?? false)
				{
					return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(new List<Expression<Func<T, bool>>>());
				}

				return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDDATA, validationErrors: new List<ValidationError>
				{
					new ValidationError
					{
						PropertyName = property.PropertyName,
						ErrorType = MessageConstants.INVALIDPROPERTY
					}
				});
			}

			if (propertyInfo.GetCustomAttribute<QueryIgnoreAttribute>() != null)
			{
				return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(new List<Expression<Func<T, bool>>>());
			}

			var propertyType = propertyInfo.PropertyType;
			if (propertyInfo.PropertyType.ImplementsIEnumerable())
			{
				propertyType = propertyInfo.PropertyType.GetDataTypeFromIEnumerable();
			}

			var expressions = new List<Expression<Func<T, bool>>>();
			var searchTermParts = new[] { property.SearchTerm };
			if ((options.ContainsSearchSplitBySpace ?? false) && propertyType == TypeConstants.String && property.Operator == CompareOperator.Contains)
			{
				searchTermParts = property.SearchTerm.Split(' ').Where(t => !t.IsNullOrEmpty()).ToArray();
			}

			var validationErrors = new List<ValidationError>();
			foreach (var searchTermPart in searchTermParts)
			{
				var dataType = propertyType.GetDataType();
				if (dataType.IsEnum)
				{
					dataType = TypeConstants.Enum;
				}

				var data = new ExpressionDataContainer
				{
					PropertyInfo = propertyInfo,
					Parameter = parameterExpression,
					Operator = property.Operator,
					ConstantValue = dataType.GetConstantExpression(searchTermPart, propertyType, property.Operator, options),
					Options = options
				};

				var conditionResult = GetConditionComparableByProperty<T>(data);
				if (conditionResult.IsFaulty)
				{
					if (!(options.SkipInvalidWhereQueries ?? false))
					{
						validationErrors.AddRange(conditionResult.ValidationErrors);
					}
				}
				else
				{
					expressions.Add(conditionResult.Data);
				}
			}

			if (validationErrors.Count > 0)
			{
				return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(MessageConstants.INVALIDDATA, validationErrors: validationErrors);
			}

			return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(expressions);
		}

		private ResponseData<Expression<Func<T, bool>>> GetConditionComparableByProperty<T>(ExpressionDataContainer data) where T : class
		{
			data.Member = Expression.MakeMemberAccess(data.Parameter, data.PropertyInfo);

			return GetConditionComparableByMemberExpression<T>(data);
		}

		private ResponseData<Expression<Func<T, bool>>> GetConditionComparableByMemberExpression<T>(ExpressionDataContainer data) where T : class
		{
			if (data.ConstantValue == null && !(data.Member.Type == TypeConstants.String || data.Member.Type.IsDataTypeNullable()))
			{
				return ResponseData<Expression<Func<T, bool>>>.CreateFaultyResponse(MessageConstants.INVALIDDATA, validationErrors: new List<ValidationError>
				{
					new ValidationError
					{
						PropertyName = data.Member.Member.Name,
						ErrorType = MessageConstants.INVALIDFILTERVALUE
					}
				});
			}

			if (!data.Operator.ExistsOperation())
			{
				return ResponseData<Expression<Func<T, bool>>>.CreateFaultyResponse(MessageConstants.INVALIDDATA, validationErrors: new List<ValidationError>
				{
					new ValidationError
					{
						PropertyName = data.Member.Member.Name,
						Value = data.Operator.ToString(),
						ErrorType = MessageConstants.INVALIDOPERATOR
					}
				});
			}

			if (data.Member.Type.IsEnum && data.Operator.ExistsExpressionType())
			{
				var enumCompareToExpression = Expression.Call(data.Member, data.Member.Type.GetMethod("CompareTo", [data.Member.Type]), Expression.Convert(data.ConstantValue, TypeConstants.Object));
				var binaryExpression = Expression.MakeBinary(data.Operator.GetExpressionType(), enumCompareToExpression, ExpressionConstants.CompareTo);

				return new ResponseData<Expression<Func<T, bool>>>
				{
					Data = Expression.Lambda<Func<T, bool>>(binaryExpression, data.Parameter)
				};
			}

			var expression = default(Expression);
			try
			{
				expression = data.Operator.BuildExpression(data.Member, data.ConstantValue, data.Options);
			}
			catch (Exception ex)
			{
				return ResponseData<Expression<Func<T, bool>>>.CreateFaultyResponse(MessageConstants.INVALIDDATA, rawMessage: ex.Message, validationErrors: new List<ValidationError>
				{
					new ValidationError
					{
						PropertyName = data.Member.Member.Name,
						Value = data.Operator.ToString(),
						ErrorType = MessageConstants.INVALIDFILTERVALUE
					}
				});
			}

			return new ResponseData<Expression<Func<T, bool>>>
			{
				Data = Expression.Lambda<Func<T, bool>>(expression, data.Parameter)
			};
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

		private WhereQueryProperty ConvertFilterFieldToWhereQueryProperty(ComplexFilterField filterField, IList<ValidationError> invalidFilterFields, HashSet<CompareOperator> allowedCompareOperators, HashSet<string> allowedFields, WhereQueryEngineOptions options)
		{
			if (filterField == null)
			{
				if (!(options.SkipInvalidWhereQueries ?? false))
				{
					invalidFilterFields.Add(new ValidationError
					{
						PropertyName = nameof(FilterField),
						ErrorType = MessageConstants.ISNULL
					});
				}

				return null;
			}

			// operator overrules link operator
			if (filterField.Operator != CompareOperator.None)
			{
				if (allowedFields.Count > 0 && !allowedFields.Contains(filterField.Field))
				{
					if (!(options.SkipInvalidWhereQueries ?? false))
					{
						invalidFilterFields.Add(new ValidationError
						{
							PropertyName = filterField.Field,
							ErrorType = MessageConstants.NOTALLOWED
						});
					}

					return null;
				}

				if (allowedCompareOperators.Count > 0 && !allowedCompareOperators.Contains(filterField.Operator))
				{
					if (!(options.SkipInvalidWhereQueries ?? false))
					{
						invalidFilterFields.Add(new ValidationError
						{
							PropertyName = filterField.Field,
							Value = filterField.Operator.ToString(),
							ErrorType = MessageConstants.INVALIDOPERATOR
						});
					}

					return null;
				}

				return new WhereQueryProperty
				{
					PropertyName = filterField.Field,
					Operator = filterField.Operator,
					SearchTerm = filterField.SearchTerm
				};
			}

			if (filterField.LinkOperator == LinkOperator.None)
			{
				if (!(options.SkipInvalidWhereQueries ?? false))
				{
					invalidFilterFields.Add(new ValidationError
					{
						PropertyName = filterField.Field,
						ErrorType = MessageConstants.INVALIDLINKOPERATOR
					});
				}

				return null;
			}

			if (!(filterField.LinkOperations?.Any() ?? false))
			{
				if (!(options.SkipInvalidWhereQueries ?? false))
				{
					invalidFilterFields.Add(new ValidationError
					{
						PropertyName = filterField.Field,
						ErrorType = MessageConstants.LINKOPERATIONSREQUIRED
					});
				}

				return null;
			}

			var whereQueryPropertyGroup = new WhereQueryProperty
			{
				Operator = CompareOperator.None,
				LinkOperator = filterField.LinkOperator,
				LinkOperations = new List<WhereQueryProperty>()
			};

			foreach (var linkOperationFilterField in filterField.LinkOperations)
			{
				var whereQueryProperty = ConvertFilterFieldToWhereQueryProperty(linkOperationFilterField, invalidFilterFields, allowedCompareOperators, allowedFields, options);
				if (whereQueryProperty != null)
				{
					whereQueryPropertyGroup.LinkOperations.Add(whereQueryProperty);
				}
			}

			if (whereQueryPropertyGroup.LinkOperations.Count > 0)
			{
				return whereQueryPropertyGroup;
			}

			if (!(options.SkipInvalidWhereQueries ?? false))
			{
				invalidFilterFields.Add(new ValidationError
				{
					PropertyName = filterField.Field,
					ErrorType = MessageConstants.NOVALIDLINKOPERATIONS
				});
			}

			return null;
		}

		private ResponseData<IEnumerable<Expression<Func<T, bool>>>> ProcessProperty<T>(WhereQueryProperty property, BuildQueryContainer<T> queryContainer) where T : class
		{
			var conditions = new List<Expression<Func<T, bool>>>();

			if (queryContainer.Mappings.ContainsKey(property.PropertyName))
			{
				var members = new List<Expression<Func<T, bool>>>();
				foreach (var mapping in queryContainer.Mappings[property.PropertyName])
				{
					var mappingResult = GetMappingCondition(property, mapping, queryContainer.Options);
					if (mappingResult.IsFaulty)
					{
						return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(mappingResult);
					}

					var joinResult = JoinAndExpressions(mappingResult.Data);
					if (joinResult != null)
					{
						members.Add(joinResult);
					}
				}

				if (members.Count == 1)
				{
					conditions.Add(members.Single());
				}
				else if (members.Count > 1)
				{
					var joinResult = JoinOrExpressions(members);
					if (joinResult != null)
					{
						conditions.Add(joinResult);
					}
				}
			}
			else
			{
				var conditionResult = GetPropertyCondition<T>(property, queryContainer.PropertyInfos, queryContainer.Parameter, queryContainer.Options);
				if (conditionResult.IsFaulty)
				{
					return ResponseData<IEnumerable<Expression<Func<T, bool>>>>.CreateFaultyResponse(conditionResult);
				}

				conditions.AddRange(conditionResult.Data);
			}

			return new ResponseData<IEnumerable<Expression<Func<T, bool>>>>(conditions);
		}

		private static WhereQueryEngineOptions PrepareOptions(WhereQueryEngineOptions local, WhereQueryEngineOptions global)
		{
			if (local is null)
			{
				local = new WhereQueryEngineOptions();
			}

			if (!local.UseUtcDateTime.HasValue)
			{
				local.UseUtcDateTime = global.UseUtcDateTime;
			}

			if (!local.ContainsSearchSplitBySpace.HasValue)
			{
				local.ContainsSearchSplitBySpace = global.ContainsSearchSplitBySpace;
			}

			if (!local.SkipInvalidWhereQueries.HasValue)
			{
				local.SkipInvalidWhereQueries = global.SkipInvalidWhereQueries;
			}

			if (!local.CaseInsensitive.HasValue)
			{
				local.CaseInsensitive = global.CaseInsensitive;
			}

			return local;
		}
	}
}