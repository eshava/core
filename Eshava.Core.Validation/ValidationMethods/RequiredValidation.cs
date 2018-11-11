using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class RequiredValidation
	{
		public static ValidationCheckResult CheckRequired(ValidationCheckParameters parameters)
		{
			if (Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(RequiredAttribute)) != null)
			{
				if (parameters.PropertyValue == null)
				{
					return new ValidationCheckResult { ValidationError = $"{nameof(CheckRequired)}->{parameters.PropertyInfo.Name}->ValueIsNull" };
				}

				if (parameters.PropertyInfo.PropertyType == typeof(string))
				{
					var valueString = parameters.PropertyValue as string;

					if (valueString.IsNullOrEmpty())
					{
						return new ValidationCheckResult { ValidationError = $"{nameof(CheckRequired)}->{parameters.PropertyInfo.Name}->StringValueIsNullOrEmpty" };
					}
				}

				if (parameters.PropertyInfo.PropertyType.ImplementsIEnumerable())
				{
					var elements = parameters.PropertyValue as IEnumerable;
					if (elements.Cast<object>().Any())
					{
						return new ValidationCheckResult { IsValid = true };
					}

					return new ValidationCheckResult { ValidationError = $"{nameof(CheckRequired)}->{parameters.PropertyInfo.Name}->EmptyIEnumerable" };
				}
			}

			return new ValidationCheckResult { IsValid = true };
		}
	}
}