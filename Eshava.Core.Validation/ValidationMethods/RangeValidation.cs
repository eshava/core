using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class RangeValidation
	{
		private static List<(Func<Type, bool> Check, Func<ValidationCheckParameters, RangeAttribute, ValidationCheckResult> Validate)> _validationRules =
			new List<(Func<Type, bool> Check, Func<ValidationCheckParameters, RangeAttribute, ValidationCheckResult> Validate)>
			{
				(type => type == typeof(float), (parameters, range) => CheckRangeFloat(parameters, range)),
				(type => type == typeof(double), (parameters, range) => CheckRangeDouble(parameters, range)),
				(type => type == typeof(decimal), (parameters, range) => CheckRangeDecimal(parameters, range)),
				(type => type == typeof(int) || type.IsEnum, (parameters, range) => CheckRangeInteger(parameters, range)),
				(type => type == typeof(long), (parameters, range) => CheckRangeLong(parameters, range))
			};

		public static ValidationCheckResult CheckRange(ValidationCheckParameters parameters)
		{
			var dataType = parameters.PropertyInfo.GetDataType();
			var range = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(RangeAttribute)) as RangeAttribute;

			if (parameters.PropertyValue == null || range == null)
			{
				return new ValidationCheckResult();
			}

			var validationRule = _validationRules.SingleOrDefault(rule => rule.Check(dataType));
			if (validationRule.Validate != null)
			{
				return validationRule.Validate(parameters, range);
			}

			return GetErrorResult(ValidationErrorType.DataTypeNotSupported, parameters.PropertyInfo.Name);
		}

		private static ValidationCheckResult CheckRangeInteger(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var maximum = Convert.ToInt32(range.Maximum);
			var minimum = Convert.ToInt32(range.Minimum);
			var valueInteger = (int)parameters.PropertyValue;

			if (valueInteger < minimum || valueInteger > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeInteger, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckRangeLong(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var maximum = Convert.ToInt64(range.Maximum);
			var minimum = Convert.ToInt64(range.Minimum);
			var valueLong = (long)parameters.PropertyValue;

			if (valueLong < minimum || valueLong > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeLong, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckRangeFloat(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var maximum = Convert.ToDouble(range.Maximum, CultureInfo.InvariantCulture);
			var minimum = Convert.ToDouble(range.Minimum, CultureInfo.InvariantCulture);
			var valueFloat = (float)parameters.PropertyValue;

			if (valueFloat < minimum || valueFloat > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeFloat, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckRangeDouble(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var maximum = Convert.ToDouble(range.Maximum, CultureInfo.InvariantCulture);
			var minimum = Convert.ToDouble(range.Minimum, CultureInfo.InvariantCulture);
			var valueDouble = (double)parameters.PropertyValue;

			if (valueDouble < minimum || valueDouble > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeDouble, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckRangeDecimal(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var maximum = Convert.ToDecimal(range.Maximum, CultureInfo.InvariantCulture);
			var minimum = Convert.ToDecimal(range.Minimum, CultureInfo.InvariantCulture);
			var valueDecimal = (decimal)parameters.PropertyValue;

			if (valueDecimal < minimum || valueDecimal > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeDecimal, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult();
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