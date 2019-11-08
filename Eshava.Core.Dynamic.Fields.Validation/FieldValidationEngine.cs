using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation.Interfaces;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Dynamic.Fields.Validation.ValidationMethods;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields
{
	public class FieldValidationEngine<T, D> : IFieldValidationEngine<T, D>
	{
		private readonly List<Func<ValidationCheckParameters<T, D>, ValidationCheckResult>> _validationMethods = new List<Func<ValidationCheckParameters<T, D>, ValidationCheckResult>>
		{
			RequiredValidation<T, D>.CheckRequired,
			StringValidation<T, D>.CheckStringLength,
			StringValidation<T, D>.CheckMailAddress,
			StringValidation<T, D>.CheckUrl,
			DecimalPlacesValidation<T, D>.CheckDecimalPlaces,
			RangeValidation<T, D>.CheckRange,
			RangeFromOrToValidation<T, D>.CheckRangeFrom,
			RangeFromOrToValidation<T, D>.CheckRangeTo,
			RangeBetweenValidation<T, D>.CheckRangeBetween,
			parameter => { parameter.NotEquals = false; return EqualsValidation<T, D>.CheckEqualsTo(parameter); },
			parameter => { parameter.NotEquals = true; return EqualsValidation<T, D>.CheckEqualsTo(parameter); }
		};

		public ValidationCheckResult Validate(FieldInformation<T, D> fieldInformation, AnalysisResult analysisResult)
		{
			if ((fieldInformation?.IsValid ?? false) || (analysisResult?.Result?.Count ?? 0) == 0)
			{
				return new ValidationCheckResult
				{
					ValidationErrors = new List<ValidationCheckResultEntry>
					{
						new ValidationCheckResultEntry
						{
							MethodType = ValidationMethodType.Input,
							ErrorType =  ValidationErrorType.IsNull
						}
					}
				};
			}

			var results = new List<ValidationCheckResult>();
			foreach (var fieldValue in fieldInformation.Values)
			{
				var fieldAssignment = fieldInformation.GetAssignment(fieldValue.AssignmentId);
				var fieldDefinition = fieldAssignment == null ? null : fieldInformation.GetDefinition(fieldAssignment.DefinitionId);

				if (fieldAssignment == null || fieldDefinition == null)
				{
					results.Add(new ValidationCheckResult
					{
						IsValid = false,
						ValidationErrors = new List<ValidationCheckResultEntry>
						{
							new ValidationCheckResultEntry
							{
								PropertyName = fieldValue.Id.ToString(),
								 ErrorType = ValidationErrorType.IsNull,
								 MethodType = ValidationMethodType.Input,
								 PropertyNameFrom = nameof(IFieldDefinition<T>) + ": " + (fieldDefinition == null ? "missing" : fieldDefinition.Id.ToString()),
								 PropertyNameTo = nameof(IFieldAssignment<T, D>) + ": " + (fieldAssignment == null ? "missing" : fieldAssignment.Id.ToString())
							}
						}
					});
				}

				var baseField = analysisResult.Result[fieldAssignment.Id.ToString()];
				var validationParameter = new ValidationCheckParameters<T, D>
				{
					AnalysisResult = analysisResult,
					FieldDefinition = fieldDefinition,
					FieldInformation = fieldInformation,
					Field = baseField,
					PropertyInfo = fieldValue
				};

				var validationMethodResult = _validationMethods.Select(method => method(validationParameter)).ToList();
				if (validationMethodResult.Any())
				{
					results.AddRange(validationMethodResult);
				}
			}

			if (results.All(result => result.IsValid))
			{
				return results.First();
			}

			return new ValidationCheckResult { ValidationErrors = results.SelectMany(r => r.ValidationErrors).ToList() };
		}
	}
}