using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;
using Eshava.Core.Validation.Extension;

namespace Eshava.Core.Dynamic.Fields.Validation.ValidationMethods
{
	internal static class StringValidation<T, D>
	{
		private static Type _typeString = typeof(string);

		public static ValidationCheckResult CheckStringLength(ValidationCheckParameters<T, D> parameters)
		{
			var valueString = parameters.Field.Value as string;
			if (parameters.Field.Type != _typeString || valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var maxLength = parameters.GetConfigurations(FieldConfigurationType.MaxLength).FirstOrDefault();
			var minLength = parameters.GetConfigurations(FieldConfigurationType.MinLength).FirstOrDefault();

			if (maxLength != null && valueString.Length > maxLength.ValueInteger)
			{
				return GetErrorResult(ValidationErrorType.GreaterMaxLength, parameters.Field.Id);
			}

			if (minLength != null && valueString.Length < minLength.ValueInteger)
			{
				return GetErrorResult(ValidationErrorType.LowerMinLength, parameters.Field.Id);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		public static ValidationCheckResult CheckMailAddress(ValidationCheckParameters<T, D> parameters)
		{
			var valueString = parameters.Field.Value as string;
			if (parameters.Field.Type != _typeString || valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var isEmail = parameters.GetConfigurations(FieldConfigurationType.Email).Any();
			if (isEmail && !valueString.IsEmailAddress())
			{
				return GetErrorResult(ValidationErrorType.NoWellFormedMailAddress, parameters.Field.Id);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		public static ValidationCheckResult CheckUrl(ValidationCheckParameters<T, D> parameters)
		{
			var valueString = parameters.Field.Value as string;
			if (parameters.Field.Type != _typeString || valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var isEmail = parameters.GetConfigurations(FieldConfigurationType.Url).Any();
			if (isEmail && !valueString.IsUrl())
			{
				return GetErrorResult(ValidationErrorType.NoWellFormedUri, parameters.Field.Id);
			}

			return new ValidationCheckResult { IsValid = true };
		}

		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string fieldId)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationCheckResultEntry>
				{
					new ValidationCheckResultEntry
					{
						MethodType = ValidationMethodType.String,
						ErrorType = errorType,
						PropertyName = fieldId
					}
				}
			};
		}
	}
}