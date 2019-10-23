using System;
using System.Collections.Generic;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class DecimalPlacesValidation
	{
		public static ValidationCheckResult CheckDecimalPlaces(ValidationCheckParameters parameters)
		{
			var dataType = parameters.PropertyInfo.GetDataType();
			var decimalPlaces = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(DecimalPlacesAttribute)) as DecimalPlacesAttribute;

			if (parameters.PropertyValue == null || decimalPlaces == null)
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var decimalPlacesValue = decimalPlaces.DecimalPlaces;
			if (decimalPlacesValue < 0)
			{
				decimalPlacesValue = 0;
			}

			if (decimalPlacesValue >= 0)
			{
				var faktor = Convert.ToInt32(Math.Pow(10, decimalPlacesValue));

				if (dataType == typeof(float) || dataType == typeof(double))
				{
					var valueDouble = Convert.ToDouble(parameters.PropertyValue);
					valueDouble *= faktor;

					if (!Equals(Math.Truncate(valueDouble), valueDouble))
					{
						return GetErrorResult(ValidationErrorType.DataTypeFloatOrDouble, parameters.PropertyInfo.Name);
					}
				}
				else if (dataType == typeof(decimal))
				{
					var valueDecimal = (decimal)parameters.PropertyValue;
					valueDecimal *= faktor;

					if (!Equals(Math.Truncate(valueDecimal), valueDecimal))
					{
						return GetErrorResult(ValidationErrorType.DataTypeDecimal, parameters.PropertyInfo.Name);
					}
				}
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
						MethodType = ValidationMethodType.DecimalPlaces,
						ErrorType = errorType,
						PropertyName = propertyName
					}
				}
			};
		}
	}
}