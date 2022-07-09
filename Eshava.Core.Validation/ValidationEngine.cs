using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Models;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Extension;
using Eshava.Core.Validation.Interfaces;
using Eshava.Core.Validation.Models;
using Eshava.Core.Validation.ValidationMethods;

namespace Eshava.Core.Validation
{
	public class ValidationEngine : IValidationEngine
	{
		private readonly List<Func<ValidationCheckParameters, ValidationCheckResult>> _validationMethods = new List<Func<ValidationCheckParameters, ValidationCheckResult>>
		{
			RequiredValidation.CheckRequired,
			StringValidation.CheckStringLength,
			StringValidation.CheckMailAddress,
			StringValidation.CheckUrl,
			DecimalPlacesValidation.CheckDecimalPlaces,
			RangeValidation.CheckRange,
			RangeFromOrToValidation.CheckRangeFrom,
			RangeFromOrToValidation.CheckRangeTo,
			RangeBetweenValidation.CheckRangeBetween,
			RegularExpressionValidation.CheckRegularExpression,
			parameter => { parameter.NotEquals = false; return EqualsValidation.CheckEqualsTo(parameter); },
			parameter => { parameter.NotEquals = true; return EqualsValidation.CheckEqualsTo(parameter); }
		};

		public ValidationCheckResult Validate(object model)
		{
			if (model == null)
			{
				return new ValidationCheckResult
				{
					ValidationErrors = new List<ValidationError>
					{
						new ValidationError
						{
							MethodType = ValidationMethodType.Input.ToString(),
							ErrorType =  ValidationErrorType.IsNull.ToString(),
							PropertyName = nameof(model)
						}
					}
				};
			}

			var results = new List<ValidationCheckResult>();
			var modelType = model.GetType();

			foreach (var propertyInfo in modelType.GetProperties())
			{
				if (propertyInfo.HasAttribute<ValidationIgnoreAttribute>())
				{
					continue;
				}

				var propertyValue = propertyInfo.GetValue(model);
				var validationParameter = new ValidationCheckParameters
				{
					Model = model,
					DataType = modelType,
					PropertyInfo = propertyInfo
				};

				if (propertyValue != null && propertyInfo.PropertyType.ImplementsIEnumerable())
				{
					results.AddRange(ValidateEnumerable(propertyValue, propertyInfo, validationParameter));
				}
				validationParameter.PropertyValue = propertyValue;

				if (propertyValue != null && propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string) && !propertyInfo.PropertyType.ImplementsIEnumerable())
				{
					results.Add(Validate(propertyValue));
				}
				else
				{
					var validationMethodResult = _validationMethods.Select(method =>
					{
						validationParameter.DataType = modelType;

						return method(validationParameter);
					}).ToList();

					if (validationMethodResult.Any())
					{
						results.AddRange(validationMethodResult);
					}
				}
			}

			if (results.All(result => result.IsValid))
			{
				return results.First();
			}

			return new ValidationCheckResult { ValidationErrors = results.SelectMany(r => r.ValidationErrors).ToList() };
		}

		private IEnumerable<ValidationCheckResult> ValidateEnumerable(object propertyValue, PropertyInfo propertyInfo, ValidationCheckParameters validationParameter)
		{
			var results = new List<ValidationCheckResult>();
			var dataType = propertyInfo.PropertyType.GetDataTypeFromIEnumerable();

			if (dataType.IsClass && propertyValue is IEnumerable elements)
			{
				if (dataType == typeof(string))
				{
					results.AddRange(elements.Cast<string>().Select(s =>
					{
						validationParameter.PropertyValue = s;

						return StringValidation.CheckStringLength(validationParameter);
					}).ToList());
				}
				else
				{
					results.AddRange(elements.Cast<object>().Select(Validate).ToList());
				}
			}

			return results;
		}
	}
}