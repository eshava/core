using System;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Attributes;
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

			if (decimalPlaces.DecimalPlaces >= 0)
			{
				var faktor = Convert.ToInt32(Math.Pow(10, decimalPlaces.DecimalPlaces));

				if (dataType == typeof(float) || dataType == typeof(double))
				{
					var valueDouble = Convert.ToDouble(parameters.PropertyValue);
					valueDouble *= faktor;

					if (!Equals(Math.Truncate(valueDouble), valueDouble))
					{
						return new ValidationCheckResult {ValidationError = $"{nameof(CheckDecimalPlaces)}->{parameters.PropertyInfo.Name}->FloatOrDoubleValue" };
					}
				}
				else if (dataType == typeof(decimal))
				{
					var valueDecimal = (decimal)parameters.PropertyValue;
					valueDecimal *= faktor;

					if (!Equals(Math.Truncate(valueDecimal), valueDecimal))
					{
						return new ValidationCheckResult {ValidationError = $"{nameof(CheckDecimalPlaces)}->{parameters.PropertyInfo.Name}->DecimalValue" };
					}
				}
			}

			return new ValidationCheckResult { IsValid = true };
		}
	}
}