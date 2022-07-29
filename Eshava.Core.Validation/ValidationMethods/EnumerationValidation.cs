using System;
using System.Collections.Generic;
using Eshava.Core.Extensions;
using Eshava.Core.Models;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class EnumerationValidation
	{
		public static ValidationCheckResult CheckEnumeration(ValidationCheckParameters parameters)
		{
			var dataType = parameters.PropertyInfo.GetDataType();
			if (!dataType.IsEnum || parameters.PropertyValue == null)
			{
				return new ValidationCheckResult();
			}

			var enumAttribute = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(EnumerationAttribute)) as EnumerationAttribute;
			if (enumAttribute?.SkipValidation ?? false)
			{
				return new ValidationCheckResult();
			}

			var invalidateZero = enumAttribute?.InvalidateZero ?? false;
			var propertyValue = Convert.ToInt32(parameters.PropertyValue);
			var isValidEnumValue = false;

			foreach (var enumValue in Enum.GetValues(dataType))
			{
				if ((int)enumValue == propertyValue && (propertyValue != 0 || !invalidateZero))
				{
					isValidEnumValue = true;

					break;
				}
			};

			if (!isValidEnumValue)
			{
				return GetErrorResult(parameters.PropertyInfo.Name, parameters.PropertyValue.ToString());
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult GetErrorResult(string propertyName, string propertyValue)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationError>
				{
					new ValidationError
					{
						MethodType = ValidationMethodType.Enumeration.ToString(),
						ErrorType = ValidationErrorType.Invalid.ToString(),
						PropertyName = propertyName,
						Value = propertyValue
					}
				}
			};
		}
	}
}