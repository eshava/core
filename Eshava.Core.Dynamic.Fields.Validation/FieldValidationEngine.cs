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

namespace Eshava.Core.Dynamic.Fields.Validation
{
	public class FieldValidationEngine<FD, FA, FV, T, D> : IFieldValidationEngine<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : class, IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		private readonly List<Func<ValidationCheckParameters<FD, FA, FV, T, D>, ValidationCheckResult>> _validationMethods = new List<Func<ValidationCheckParameters<FD, FA, FV, T, D>, ValidationCheckResult>>
		{
			RequiredValidation<FD, FA, FV, T, D>.CheckRequired,
			StringValidation<FD, FA, FV, T, D>.CheckStringLength,
			StringValidation<FD, FA, FV, T, D>.CheckMailAddress,
			StringValidation<FD, FA, FV, T, D>.CheckUrl,
			DecimalPlacesValidation<FD, FA, FV, T, D>.CheckDecimalPlaces,
			RangeValidation<FD, FA, FV, T, D>.CheckRange,
			RangeFromOrToValidation<FD, FA, FV, T, D>.CheckRangeFrom,
			RangeFromOrToValidation<FD, FA, FV, T, D>.CheckRangeTo,
			RangeBetweenValidation<FD, FA, FV, T, D>.CheckRangeBetween,
			parameter => { parameter.NotEquals = false; return EqualsValidation<FD, FA, FV, T, D>.CheckEqualsTo(parameter); },
			parameter => { parameter.NotEquals = true; return EqualsValidation<FD, FA, FV, T, D>.CheckEqualsTo(parameter); }
		};

		public ValidationCheckResult Validate(FieldInformation<FD, FA, FV, T, D> fieldInformation, AnalysisResult analysisResult)
		{
			if (!(fieldInformation?.IsValid ?? false) || (analysisResult?.Result?.Count ?? 0) == 0)
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
				var fieldDefinition = fieldAssignment == default ? default : fieldInformation.GetDefinition(fieldAssignment.DefinitionId);

				if (fieldAssignment == null || fieldDefinition == null)
				{
					results.Add(new ValidationCheckResult
					{
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
				var validationParameter = new ValidationCheckParameters<FD, FA, FV, T, D>
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