using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class RangeValidation
	{
		public static ValidationCheckResult CheckRange(ValidationCheckParameters parameters)
		{
			var dataType = parameters.PropertyInfo.GetDataType();
			var range = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(RangeAttribute)) as RangeAttribute;

			if (parameters.PropertyValue == null || range == null)
			{
				return new ValidationCheckResult { IsValid = true };
			}

			if (dataType == typeof(float))
			{
				return CheckRangeFloat(parameters, range);
			}

			if (dataType == typeof(double))
			{
				return CheckRangeDouble(parameters, range);
			}

			if (dataType == typeof(decimal))
			{
				return CheckRangeDecimal(parameters, range);
			}

			if (dataType == typeof(int) || dataType.IsEnum)
			{
				return CheckRangeInteger(parameters, range);
			}

			if (dataType == typeof(long))
			{
				return CheckRangeLong(parameters, range);
			}

			return GetErrorResult(ValidationErrorType.DataTypeNotSupported, parameters.PropertyInfo.Name);
		}

		private static ValidationCheckResult CheckRangeInteger(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var max = Convert.ToInt32(range.Maximum);
			var min = Convert.ToInt32(range.Minimum);
			var valueInt = (int)parameters.PropertyValue;

			if (valueInt < min || valueInt > max)
			{
				return GetErrorResult(ValidationErrorType.DataTypeInteger, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult CheckRangeLong(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var max = Convert.ToInt64(range.Maximum);
			var min = Convert.ToInt64(range.Minimum);
			var valueInt = (long)parameters.PropertyValue;

			if (valueInt < min || valueInt > max)
			{
				return GetErrorResult(ValidationErrorType.DataTypeLong, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult CheckRangeFloat(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var max = Convert.ToDouble(range.Maximum);
			var min = Convert.ToDouble(range.Minimum);
			var valueFloat = (float)parameters.PropertyValue;

			if (valueFloat < min || valueFloat > max)
			{
				return GetErrorResult(ValidationErrorType.DataTypeFloat, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult CheckRangeDouble(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var max = Convert.ToDouble(range.Maximum);
			var min = Convert.ToDouble(range.Minimum);
			var valueDouble = (double)parameters.PropertyValue;

			if (valueDouble < min || valueDouble > max)
			{
				return GetErrorResult(ValidationErrorType.DataTypeDouble, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult CheckRangeDecimal(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var max = Convert.ToDecimal(range.Maximum);
			var min = Convert.ToDecimal(range.Minimum);
			var valueDecimal = (decimal)parameters.PropertyValue;

			if (valueDecimal < min || valueDecimal > max)
			{
				return GetErrorResult(ValidationErrorType.DataTypeDecimal, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string propertyName)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationCheckResultEntry>
				{
					new ValidationCheckResultEntry
					{
						MethodType = ValidationMethodType.RangeHardCoded,
						ErrorType = errorType,
						PropertyName = propertyName
					}
				}
			};
		}
	}
}