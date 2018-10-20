using System;
using System.ComponentModel.DataAnnotations;
using Eshava.Core.Extensions;
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

			return new ValidationCheckResult { ValidationError = $"{nameof(CheckRange)}->{parameters.PropertyInfo.Name}->DataTypeNotSupported" };
		}

		private static ValidationCheckResult CheckRangeInteger(ValidationCheckParameters parameters, RangeAttribute range)
		{
			var max = Convert.ToInt32(range.Maximum);
			var min = Convert.ToInt32(range.Minimum);
			var valueInt = (int)parameters.PropertyValue;

			if (valueInt < min || valueInt > max)
			{
				return new ValidationCheckResult { ValidationError = $"{nameof(CheckRange)}->{parameters.PropertyInfo.Name}->IntegerValue" };
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
				return new ValidationCheckResult { ValidationError = $"{nameof(CheckRange)}->{parameters.PropertyInfo.Name}->FloatValue" };
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
				return new ValidationCheckResult { ValidationError = $"{nameof(CheckRange)}->{parameters.PropertyInfo.Name}->DoubleValue" };
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
				return new ValidationCheckResult { ValidationError = $"{nameof(CheckRange)}->{parameters.PropertyInfo.Name}->DecimalValue" };
			}

			return new ValidationCheckResult { IsValid = true };
		}
	}
}