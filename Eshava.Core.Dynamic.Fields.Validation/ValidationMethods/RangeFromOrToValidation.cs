using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Validation.Extensions;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.ValidationMethods
{
	internal static class RangeFromOrToValidation<T, D>
	{
		public static ValidationCheckResult CheckRangeFrom(ValidationCheckParameters<T, D> parameters)
		{
			return CheckRangeFromOrTo(parameters, FieldConfigurationType.RangeFrom, false);
		}

		public static ValidationCheckResult CheckRangeTo(ValidationCheckParameters<T, D> parameters)
		{
			return CheckRangeFromOrTo(parameters, FieldConfigurationType.RangeTo, true);
		}

		private static ValidationCheckResult CheckRangeFromOrTo(ValidationCheckParameters<T, D> parameters, FieldConfigurationType configurationType, bool invertProperties)
		{
			var rangeRules = parameters.GetConfigurations(configurationType);
			if (!rangeRules.Any())
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var fieldSourceNames = rangeRules.Where(r => !r.ValueString.IsNullOrEmpty()).Select(r => r.ValueString.Trim()).ToList();
			var results = new List<ValidationCheckResultEntry>();

			foreach (var fieldSourceName in fieldSourceNames)
			{
				var baseFieldSource = fieldSourceName.GetFieldSource(parameters);
				if (baseFieldSource == null)
				{
					results.Add(GetErrorResult(ValidationErrorType.PropertyNotFoundFrom, fieldSourceName, parameters.Field.Id));

					continue;
				}

				//Check whether the data types match
				if (parameters.Field.Type != baseFieldSource.Type)
				{
					results.Add(GetErrorResult(ValidationErrorType.DataTypesNotEqual, baseFieldSource.Id, parameters.Field.Id));

					continue;
				}

				parameters.AllowNull = parameters.GetConfigurations(FieldConfigurationType.AllowNull).Any();

				if (invertProperties)
				{
					var result = BaseRangeValidation.CheckRangeValue(parameters, parameters.Field, baseFieldSource);
					if (!result.IsValid)
					{
						results.AddRange(result.ValidationErrors);
					}
				}
				else
				{
					var result = BaseRangeValidation.CheckRangeValue(parameters, baseFieldSource, parameters.Field);
					if (!result.IsValid)
					{
						results.AddRange(result.ValidationErrors);
					}
				}
			}

			if (results.Count == 0)
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return new ValidationCheckResult { ValidationErrors = results };
		}

		private static ValidationCheckResultEntry GetErrorResult(ValidationErrorType errorType, string fieldFromId, string fieldToId)
		{
			return new ValidationCheckResultEntry
			{
				MethodType = ValidationMethodType.RangeFromOrTo,
				ErrorType = errorType,
				PropertyNameFrom = fieldFromId,
				PropertyNameTo = fieldToId
			};
		}
	}
}