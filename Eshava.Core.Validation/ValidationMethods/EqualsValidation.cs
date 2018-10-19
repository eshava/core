using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Attributes;
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
				var notEqualsProperties = notEqualsTo.PropertyName.Contains(",") ? notEqualsTo.PropertyName.Split(',') : new[] { notEqualsTo.PropertyName };
				var results = notEqualsProperties.Select(property => CheckEqualsTo(parameters, parameters.DataType.GetProperty(property), notEqualsTo.DefaultValue)).ToList();

				if (results.All(result => result.IsValid))
				{
					return results.First();
				}

				return new ValidationCheckResult { ValidationError = String.Join(Environment.NewLine, results.Where(result => !result.IsValid).Select(result => result.ValidationError)) };
			}

			if (!parameters.NotEquals && equalsTo != null)
			{
				var propertyInfoEquals = parameters.DataType.GetProperty(equalsTo.PropertyName);

				return CheckEqualsTo(parameters, propertyInfoEquals);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult CheckEqualsTo(ValidationCheckParameters parameters, PropertyInfo propertyInfoEquals, object defaultValue = null)
		{
			if (propertyInfoEquals == null)
			{
				return GetErrorResult(parameters, $"{nameof(propertyInfoEquals)}->ShouldNotBeNull");
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
					return GetErrorResult(parameters, "NotEqualsStringValue");
				}
			}
			else if (parameters.NotEquals && Equals(valueString, valueStringEquals) ||
					 !parameters.NotEquals && !Equals(valueString, valueStringEquals))
			{
				return GetErrorResult(parameters, "EqualsOrNotEqualsStringValue");
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult CheckEqualsToObject(ValidationCheckParameters parameters, PropertyInfo propertyInfoEquals, object defaultValue = null)
		{
			if (parameters.NotEquals && defaultValue != null)
			{
				if (Equals(parameters.PropertyValue, propertyInfoEquals.GetValue(parameters.Model)) && !Equals(parameters.PropertyValue, defaultValue))
				{
					return GetErrorResult(parameters, "NotEqualsUnknownDataType");
				}
			}
			else if (parameters.NotEquals && Equals(parameters.PropertyValue, propertyInfoEquals.GetValue(parameters.Model)) ||
					 !parameters.NotEquals && !Equals(parameters.PropertyValue, propertyInfoEquals.GetValue(parameters.Model)))
			{
				return GetErrorResult(parameters, "EqualsOrNotEqualsUnknownDataType");
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult GetErrorResult(ValidationCheckParameters parameters, string error, [CallerMemberName] string memberName = null)
		{
			return new ValidationCheckResult { ValidationError = $"{memberName}->{nameof(parameters.NotEquals)}={parameters.NotEquals}->{parameters.PropertyInfo.Name}->{error}" };
		}
	}
}