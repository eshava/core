using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Models;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class RangeBetweenValidation
	{
		private static List<(Func<Type, bool> Check, Func<ValidationCheckParameters, PropertyInfo, PropertyInfo, ValidationCheckResult> Validate)> _validationRules =
			new List<(Func<Type, bool> Check, Func<ValidationCheckParameters, PropertyInfo, PropertyInfo, ValidationCheckResult> Validate)>
			{
				(type => type == typeof(float), (parameters, propertyInfoFrom, propertyInfoTo) => CheckRangeBetweenFloat(parameters, propertyInfoFrom, propertyInfoTo)),
				(type => type == typeof(double), (parameters, propertyInfoFrom, propertyInfoTo) => CheckRangeBetweenDouble(parameters, propertyInfoFrom, propertyInfoTo)),
				(type => type == typeof(decimal), (parameters, propertyInfoFrom, propertyInfoTo) => CheckRangeBetweenDecimal(parameters, propertyInfoFrom, propertyInfoTo)),
				(type => type == typeof(int), (parameters, propertyInfoFrom, propertyInfoTo) => CheckRangeBetweenInteger(parameters, propertyInfoFrom, propertyInfoTo)),
				(type => type == typeof(long), (parameters, propertyInfoFrom, propertyInfoTo) => CheckRangeBetweenLong(parameters, propertyInfoFrom, propertyInfoTo)),
				(type => type == typeof(DateTime), (parameters, propertyInfoFrom, propertyInfoTo) => CheckRangeBetweenDateTime(parameters, propertyInfoFrom, propertyInfoTo))
			};

		public static ValidationCheckResult CheckRangeBetween(ValidationCheckParameters parameters)
		{
			var dataType = parameters.PropertyInfo.GetDataType();
			var rangeBetween = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(RangeBetweenAttribute)) as RangeBetweenAttribute;

			if (parameters.PropertyValue == null || rangeBetween == null)
			{
				return new ValidationCheckResult();
			}

			parameters.AllowNull = parameters.PropertyInfo.PropertyType.IsDataTypeNullable();

			//Determining the property for the start and end value of the value range
			var propertyInfoFrom = parameters.DataType.GetProperty(rangeBetween.PropertyNameFrom);
			var propertyInfoTo = parameters.DataType.GetProperty(rangeBetween.PropertyNameTo);
			if (propertyInfoFrom == null || propertyInfoTo == null)
			{
				var resultFrom = propertyInfoFrom == null ? GetErrorResult(ValidationErrorType.PropertyNotFoundFrom, parameters.PropertyInfo.Name, rangeBetween.PropertyNameFrom, rangeBetween.PropertyNameTo) : null;
				var resultTo = propertyInfoTo == null ? GetErrorResult(ValidationErrorType.PropertyNotFoundTo, parameters.PropertyInfo.Name, rangeBetween.PropertyNameFrom, rangeBetween.PropertyNameTo) : null;

				if (resultFrom != null && resultTo != null)
				{
					resultFrom.ValidationErrors.Add(resultTo.ValidationErrors.Single());

					return resultFrom;
				}

				return resultFrom ?? resultTo;
			}

			var dataTypeFrom = propertyInfoFrom.GetDataType();
			

			//Check whether the data types match
			var dataTypeTo = propertyInfoTo.GetDataType();
			if (dataType != dataTypeFrom || dataType != dataTypeTo)
			{
				return GetErrorResult(ValidationErrorType.DataTypesNotEqual, parameters.PropertyInfo.Name, rangeBetween.PropertyNameFrom, rangeBetween.PropertyNameTo);
			}

			var validationRule = _validationRules.SingleOrDefault(rule => rule.Check(dataType));
			if (validationRule.Validate != null)
			{
				return validationRule.Validate(parameters, propertyInfoFrom, propertyInfoTo);
			}

			return GetErrorResult(ValidationErrorType.DataTypeNotSupported, parameters.PropertyInfo.Name, rangeBetween.PropertyNameFrom, rangeBetween.PropertyNameTo);
		}

		private static ValidationCheckResult CheckRangeBetweenDateTime(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as DateTime? ?? DateTime.MinValue;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as DateTime?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as DateTime?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDateTime, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenInteger(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as int? ?? 0;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as int?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as int?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeInteger, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenLong(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as long? ?? 0;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as long?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as long?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeLong, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenDecimal(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as decimal? ?? 0.0m;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as decimal?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as decimal?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDecimal, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenDouble(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as double? ?? 0.0;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as double?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as double?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDouble, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenFloat(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as float? ?? 0f;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as float?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as float?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeFloat, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}
		
		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string propertyName, string propertyNameReferenceFrom, string propertyNameReferenceTo)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationError>
				{
					new ValidationError
					{
						MethodType = ValidationMethodType.RangeBetween.ToString(),
						ErrorType = errorType.ToString(),
						PropertyName = propertyName,
						PropertyNameFrom = propertyNameReferenceFrom,
						PropertyNameTo = propertyNameReferenceTo
					}
				}
			};
		}
	}
}