using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation.Extensions;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.ValidationMethods
{
	internal static class EqualsValidation<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		public static ValidationCheckResult CheckEqualsTo(ValidationCheckParameters<FD, FA, FV, T, D> parameters)
		{
			var equalRules = parameters.GetConfigurations(FieldConfigurationType.EqualsTo);
			var notEqualRules = parameters.GetConfigurations(FieldConfigurationType.NotEqualsTo);
			var notEqualDefaultValue = parameters.GetConfigurations(FieldConfigurationType.NotEqualsDefault).FirstOrDefault();

			if (parameters.NotEquals && notEqualRules.Any())
			{
				return CheckEqualsTo(parameters, notEqualRules, notEqualDefaultValue?.ValueString);
			}

			if (!parameters.NotEquals && equalRules.Any())
			{
				return CheckEqualsTo(parameters, equalRules);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckEqualsTo(ValidationCheckParameters<FD, FA, FV, T, D> parameters, IEnumerable<IFieldConfiguration<T>> rules, object defaultValue = null)
		{
			var results = rules.Select(rule => CheckEqualsTo(parameters, rule.ValueString, defaultValue)).ToList();
			if (results.All(result => result.IsValid))
			{
				return results.First();
			}

			return new ValidationCheckResult { ValidationErrors = results.SelectMany(r => r.ValidationErrors).ToList() };
		}

		private static ValidationCheckResult CheckEqualsTo(ValidationCheckParameters<FD, FA, FV, T, D> parameters, string propertyNameInfoEquals, object defaultValue = null)
		{
			var fieldEquals = propertyNameInfoEquals.GetFieldSource(parameters);
			if (fieldEquals == null)
			{
				return GetErrorResult(parameters, ValidationErrorType.PropertyNotFoundTo, propertyNameInfoEquals);
			}

			if (parameters.Field.Type == typeof(string))
			{
				return CheckEqualsToString(parameters, fieldEquals, defaultValue);
			}

			return CheckEqualsToObject(parameters, fieldEquals, defaultValue);
		}

		private static ValidationCheckResult CheckEqualsToString(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldEquals, object defaultValue = null)
		{
			var valueString = (parameters.Field.Value as string).ReturnNullByEmpty();
			var valueStringEquals = (fieldEquals.Value as string).ReturnNullByEmpty();

			if (parameters.NotEquals && defaultValue != null)
			{
				if (Equals(valueString, valueStringEquals) && !Equals(valueString, defaultValue))
				{
					return GetErrorResult(parameters, ValidationErrorType.EqualsAndNotEqualToDefaultString, fieldEquals.Id);
				}
			}
			else if (parameters.NotEquals && Equals(valueString, valueStringEquals) ||
					 !parameters.NotEquals && !Equals(valueString, valueStringEquals))
			{
				return GetErrorResult(parameters, parameters.NotEquals ? ValidationErrorType.EqualsString : ValidationErrorType.NotEqualsString, fieldEquals.Id);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckEqualsToObject(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldEquals, object defaultValue = null)
		{
			if (parameters.NotEquals && defaultValue != null)
			{
				if (Equals(parameters.Field.Value, fieldEquals.Value) && !Equals(parameters.Field.Value, defaultValue))
				{
					return GetErrorResult(parameters, ValidationErrorType.EqualsAndNotEqualToDefault, fieldEquals.Id);
				}
			}
			else if (parameters.NotEquals && Equals(parameters.Field.Value, fieldEquals.Value) ||
					 !parameters.NotEquals && !Equals(parameters.Field.Value, fieldEquals.Value))
			{

				return GetErrorResult(parameters, parameters.NotEquals ? ValidationErrorType.Equals : ValidationErrorType.NotEquals, fieldEquals.Id);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult GetErrorResult(ValidationCheckParameters<FD, FA, FV, T, D> parameters, ValidationErrorType errorType, string fieldToEqualsId)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationCheckResultEntry>
				{
					new ValidationCheckResultEntry
					{
						MethodType = parameters.NotEquals ? ValidationMethodType.NotEquals : ValidationMethodType.Equals,
						ErrorType = errorType,
						PropertyName = parameters.Field.Id,
						PropertyNameTo = fieldToEqualsId
					}
				}
			};
		}
	}
}