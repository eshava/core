using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eshava.Core.Extensions;
using Eshava.Core.Models;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal class RegularExpressionValidation
	{
		private static Type _typeString = typeof(string);

		public static ValidationCheckResult CheckRegularExpression(ValidationCheckParameters parameters)
		{
			if (parameters.PropertyInfo.PropertyType.GetDataTypeFromIEnumerable() != _typeString)
			{
				return new ValidationCheckResult();
			}

			var valueString = parameters.PropertyValue as string;
			if (valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult();
			}

			var regex = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(RegularExpressionAttribute)) as RegularExpressionAttribute;

			if (regex != null && !regex.IsValid(valueString))
			{
				return GetErrorResult(ValidationErrorType.RegularExpression, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string propertyName)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationError>
				{
					new ValidationError
					{
						MethodType = ValidationMethodType.String.ToString(),
						ErrorType = errorType.ToString(),
						PropertyName = propertyName
					}
				}
			};
		}
	}
}