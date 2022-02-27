using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Extensions;
using Eshava.Core.Models;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.ValidationMethods
{
	internal static class RequiredValidation<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		public static ValidationCheckResult CheckRequired(ValidationCheckParameters<FD, FA, FV, T, D> parameters)
		{
			var isRequired = parameters.GetConfigurations(FieldConfigurationType.Required).Any();
			if (isRequired)
			{
				if (parameters.Field.Value == null)
				{
					return GetErrorResult(ValidationErrorType.IsNull, parameters.Field.Id);
				}

				if (parameters.Field.Type == typeof(string))
				{
					var valueString = parameters.Field.Value as string;

					if (valueString.IsNullOrEmpty())
					{
						return GetErrorResult(ValidationErrorType.IsEmpty, parameters.Field.Id);
					}
				}
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string fieldId)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationError>
				{
					new ValidationError
					{
						MethodType = ValidationMethodType.Required.ToString(),
						ErrorType = errorType.ToString(),
						PropertyName = fieldId
					}
				}
			};
		}
	}
}