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
	internal static class EqualsValidation
	{
		public static ValidationCheckResult CheckEqualsTo(ValidationCheckParameters parameters)
		{
			var equalsTo = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(EqualsToAttribute)) as EqualsToAttribute;
			var notEqualsTo = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(NotEqualsToAttribute)) as NotEqualsToAttribute;

			if (parameters.NotEquals && notEqualsTo != null)
			{
				return SplitPropertyNamesAndCheckEqualsTo(parameters, notEqualsTo.PropertyName, notEqualsTo.DefaultValue);
			}

			if (!parameters.NotEquals && equalsTo != null)
			{
				return SplitPropertyNamesAndCheckEqualsTo(parameters, equalsTo.PropertyName);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult SplitPropertyNamesAndCheckEqualsTo(ValidationCheckParameters parameters, string propertyNames, object defaultValue = null)
		{
			var notEqualsProperties = propertyNames.Contains(",") ? propertyNames.Split(',') : new[] { propertyNames };
			var results = notEqualsProperties.Select(property => CheckEqualsTo(parameters, property, defaultValue)).ToList();

			if (results.All(result => result.IsValid))
			{
				return results.First();
			}

			return new ValidationCheckResult { ValidationErrors = results.SelectMany(r => r.ValidationErrors).ToList() };
		}

		private static ValidationCheckResult CheckEqualsTo(ValidationCheckParameters parameters, string propertyNameInfoEquals, object defaultValue = null)
		{
			var propertyInfoEquals = parameters.DataType.GetProperty(propertyNameInfoEquals);
			if (propertyInfoEquals == null)
			{
				return GetErrorResult(parameters, ValidationErrorType.PropertyNotFoundTo, propertyNameInfoEquals);
			}

			if (parameters.PropertyInfo.PropertyType == typeof(string))
			{
				return CheckEqualsToString(parameters, propertyInfoEquals, defaultValue);
			}

			return CheckEqualsToObject(parameters, propertyInfoEquals, defaultValue);
		}

		private static ValidationCheckResult CheckEqualsToString(ValidationCheckParameters parameters, PropertyInfo propertyInfoEquals, object defaultValue = null)
		{
			var valueString = (parameters.PropertyValue as string).ReturnNullByEmpty();
			var valueStringEquals = (propertyInfoEquals.GetValue(parameters.Model) as string).ReturnNullByEmpty();

			if (parameters.NotEquals && defaultValue != null)
			{
				if (Equals(valueString, valueStringEquals) && !Equals(valueString, defaultValue))
				{
					return GetErrorResult(parameters, ValidationErrorType.EqualsAndNotEqualToDefaultString, propertyInfoEquals.Name);
				}
			}
			else if (parameters.NotEquals && Equals(valueString, valueStringEquals) ||
					 !parameters.NotEquals && !Equals(valueString, valueStringEquals))
			{
				return GetErrorResult(parameters, parameters.NotEquals ? ValidationErrorType.EqualsString : ValidationErrorType.NotEqualsString, propertyInfoEquals.Name);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckEqualsToObject(ValidationCheckParameters parameters, PropertyInfo propertyInfoEquals, object defaultValue = null)
		{
			var valueToEqual = propertyInfoEquals.GetValue(parameters.Model);

			if (parameters.NotEquals && defaultValue != null)
			{
				if (Equals(parameters.PropertyValue, valueToEqual) && !Equals(parameters.PropertyValue, defaultValue))
				{
					return GetErrorResult(parameters, ValidationErrorType.EqualsAndNotEqualToDefault, propertyInfoEquals.Name);
				}
			}
			else if (parameters.NotEquals && Equals(parameters.PropertyValue, valueToEqual) ||
					 !parameters.NotEquals && !Equals(parameters.PropertyValue, valueToEqual))
			{

				return GetErrorResult(parameters, parameters.NotEquals ? ValidationErrorType.Equals : ValidationErrorType.NotEquals, propertyInfoEquals.Name);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult GetErrorResult(ValidationCheckParameters parameters, ValidationErrorType errorType, string propertyNameToEquals)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationError>
				{
					new ValidationError
					{
						MethodType = (parameters.NotEquals ? ValidationMethodType.NotEquals : ValidationMethodType.Equals).ToString(),
						ErrorType = errorType.ToString(),
						PropertyName = parameters.PropertyInfo.Name,
						PropertyNameTo = propertyNameToEquals
					}
				}
			};
		}
	}
}