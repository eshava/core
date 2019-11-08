using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;
using Eshava.Core.Validation.Extension;
using Eshava.Core.Dynamic.Fields.Interfaces;

namespace Eshava.Core.Dynamic.Fields.Validation.ValidationMethods
{
	internal static class StringValidation<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		private static Type _typeString = typeof(string);

		public static ValidationCheckResult CheckStringLength(ValidationCheckParameters<FD, FA, FV, T, D> parameters)
		{
			var valueString = parameters.Field.Value as string;
			if (parameters.Field.Type != _typeString || valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult();
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

			return new ValidationCheckResult();
		}

		public static ValidationCheckResult CheckMailAddress(ValidationCheckParameters<FD, FA, FV, T, D> parameters)
		{
			var valueString = parameters.Field.Value as string;
			if (parameters.Field.Type != _typeString || valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult();
			}

			var isEmail = parameters.GetConfigurations(FieldConfigurationType.Email).Any();
			if (isEmail && !valueString.IsEmailAddress())
			{
				return GetErrorResult(ValidationErrorType.NoWellFormedMailAddress, parameters.Field.Id);
			}

			return new ValidationCheckResult();
		}

		public static ValidationCheckResult CheckUrl(ValidationCheckParameters<FD, FA, FV, T, D> parameters)
		{
			var valueString = parameters.Field.Value as string;
			if (parameters.Field.Type != _typeString || valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult();
			}

			var isEmail = parameters.GetConfigurations(FieldConfigurationType.Url).Any();
			if (isEmail && !valueString.IsUrl())
			{
				return GetErrorResult(ValidationErrorType.NoWellFormedUri, parameters.Field.Id);
			}

			return new ValidationCheckResult();
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