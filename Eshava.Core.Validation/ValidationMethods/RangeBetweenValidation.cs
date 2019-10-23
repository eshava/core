using System;
using System.Collections.Generic;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class RangeBetweenValidation
	{
		public static ValidationCheckResult CheckRangeBetween(ValidationCheckParameters parameters)
		{
			var dataType = parameters.PropertyInfo.GetDataType();
			var rangeBetween = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(RangeBetweenAttribute)) as RangeBetweenAttribute;

			if (parameters.PropertyValue == null || rangeBetween == null)
			{
				return new ValidationCheckResult { IsValid = true };
			}

			//Determining the proterty for the start value of the value range
			var propertyInfoFrom = parameters.DataType.GetProperty(rangeBetween.PropertyNameFrom);
			if (propertyInfoFrom == null)
			{
				return GetErrorResult(ValidationErrorType.PropertyNotFoundFrom, parameters.PropertyInfo.Name, rangeBetween.PropertyNameFrom, rangeBetween.PropertyNameTo);
			}

			//Determining the proterty for the end value of the value range
			var dataTypeFrom = propertyInfoFrom.GetDataType();
			var propertyInfoTo = parameters.DataType.GetProperty(rangeBetween.PropertyNameTo);
			if (propertyInfoTo == null)
			{
				return GetErrorResult(ValidationErrorType.PropertyNotFoundTo, parameters.PropertyInfo.Name, rangeBetween.PropertyNameFrom, rangeBetween.PropertyNameTo);
			}

			//Check whether the data types match
			var dataTypeTo = propertyInfoTo.GetDataType();
			if (dataType != dataTypeFrom || dataType != dataTypeTo || dataTypeFrom != dataTypeTo)
			{
				return GetErrorResult(ValidationErrorType.DataTypesNotEqual, parameters.PropertyInfo.Name, rangeBetween.PropertyNameFrom, rangeBetween.PropertyNameTo);
			}

			if (dataType == typeof(float))
			{
				return CheckRangeBetweenFloat(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (dataType == typeof(double))
			{
				return CheckRangeBetweenDouble(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (dataType == typeof(decimal))
			{
				return CheckRangeBetweenDecimal(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (dataType == typeof(int))
			{
				return CheckRangeBetweenInteger(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (dataType == typeof(long))
			{
				return CheckRangeBetweenLong(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (dataType == typeof(DateTime))
			{
				return CheckRangeBetweenDateTime(parameters, propertyInfoFrom, propertyInfoTo);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult CheckRangeBetweenDateTime(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as DateTime? ?? DateTime.MinValue;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as DateTime?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as DateTime?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, false, value))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return GetErrorResult(ValidationErrorType.DataTypeDateTime, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenInteger(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as int? ?? 0;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as int?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as int?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, false, value))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return GetErrorResult(ValidationErrorType.DataTypeInteger, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenLong(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as long? ?? 0;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as long?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as long?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, false, value))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return GetErrorResult(ValidationErrorType.DataTypeLong, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenDecimal(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as decimal? ?? 0.0m;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as decimal?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as decimal?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, false, value))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return GetErrorResult(ValidationErrorType.DataTypeDecimal, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenDouble(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as double? ?? 0.0;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as double?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as double?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, false, value))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return GetErrorResult(ValidationErrorType.DataTypeDouble, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeBetweenFloat(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var value = parameters.PropertyValue as float? ?? 0f;
			var valueFrom = propertyInfoFrom.GetValue(parameters.Model) as float?;
			var valueTo = propertyInfoTo.GetValue(parameters.Model) as float?;

			if (BaseRangeValidation.CheckRangeValue(valueFrom, valueTo, false, value))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return GetErrorResult(ValidationErrorType.DataTypeFloat, parameters.PropertyInfo.Name, propertyInfoFrom.Name, propertyInfoTo.Name);
		}
		
		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string propertyName, string propertyNameReferenceFrom, string propertyNameReferenceTo)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationCheckResultEntry>
				{
					new ValidationCheckResultEntry
					{
						MethodType = ValidationMethodType.RangeBetween,
						ErrorType = errorType,
						PropertyName = propertyName,
						PropertyNameFrom = propertyNameReferenceFrom,
						PropertyNameTo = propertyNameReferenceTo
					}
				}
			};
		}
	}
}