using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation.Extensions;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.ValidationMethods
{
	internal static class RangeBetweenValidation<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		private static List<(Func<Type, bool> Check, Func<ValidationCheckParameters<FD, FA, FV, T, D>, BaseField, BaseField, ValidationCheckResult> Validate)> _validationRules =
			new List<(Func<Type, bool> Check, Func<ValidationCheckParameters<FD, FA, FV, T, D>, BaseField, BaseField, ValidationCheckResult> Validate)>
			{
				(type => type == typeof(float), (parameters, fieldFrom, fieldTo) => CheckRangeBetweenFloat(parameters, fieldFrom, fieldTo)),
				(type => type == typeof(double), (parameters, fieldFrom, fieldTo) => CheckRangeBetweenDouble(parameters, fieldFrom, fieldTo)),
				(type => type == typeof(decimal), (parameters, fieldFrom, fieldTo) => CheckRangeBetweenDecimal(parameters, fieldFrom, fieldTo)),
				(type => type == typeof(int), (parameters, fieldFrom, fieldTo) => CheckRangeBetweenInteger(parameters, fieldFrom, fieldTo)),
				(type => type == typeof(long), (parameters, fieldFrom, fieldTo) => CheckRangeBetweenLong(parameters, fieldFrom, fieldTo)),
				(type => type == typeof(DateTime), (parameters, fieldFrom, fieldTo) => CheckRangeBetweenDateTime(parameters, fieldFrom, fieldTo))
			};

		public static ValidationCheckResult CheckRangeBetween(ValidationCheckParameters<FD, FA, FV, T, D> parameters)
		{
			var hasRangeRule = parameters.GetConfigurations(FieldConfigurationType.RangeBetween).Any();
			var rangeFromRule = parameters.GetConfigurations(FieldConfigurationType.RangeBetweenFrom).FirstOrDefault();
			var rangeToRule = parameters.GetConfigurations(FieldConfigurationType.RangeBetweenTo).FirstOrDefault();
			if (!hasRangeRule || parameters.Field.Value == null)
			{
				return new ValidationCheckResult();
			}

			parameters.AllowNull = parameters.GetConfigurations(FieldConfigurationType.AllowNull).Any();

			//Determining the field for the start and end value of the value range
			var rangeFromField = rangeFromRule?.ValueString.GetFieldSource(parameters);
			var rangeToField = rangeToRule?.ValueString.GetFieldSource(parameters);
			if (rangeFromField == null || rangeToField == null)
			{
				var resultFrom = rangeFromField == null ? GetErrorResult(ValidationErrorType.PropertyNotFoundFrom, parameters.Field.Id, rangeFromRule?.ValueString ?? "Missing", rangeToRule?.ValueString ?? "Missing") : null;
				var resultTo = rangeToField == null ? GetErrorResult(ValidationErrorType.PropertyNotFoundTo, parameters.Field.Id, rangeFromRule?.ValueString ?? "Missing", rangeToRule?.ValueString ?? "Missing") : null;

				if (resultFrom != null && resultTo != null)
				{
					resultFrom.ValidationErrors.Add(resultTo.ValidationErrors.Single());

					return resultFrom;
				}

				return resultFrom ?? resultTo;
			}
	
			//Check whether the data types match
			if (parameters.Field.Type != rangeFromField.Type || parameters.Field.Type != rangeToField.Type)
			{
				return GetErrorResult(ValidationErrorType.DataTypesNotEqual, parameters.Field.Id, rangeFromRule.ValueString, rangeToRule.ValueString);
			}

			var validationRule = _validationRules.SingleOrDefault(rule => rule.Check(parameters.Field.Type));
			if (validationRule.Validate != null)
			{
				return validationRule.Validate(parameters, rangeFromField, rangeToField);
			}

			return GetErrorResult(ValidationErrorType.DataTypeNotSupported, parameters.Field.Id, rangeFromRule.ValueString, rangeToRule.ValueString);
		}

		private static ValidationCheckResult CheckRangeBetweenDateTime(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField rangeFromField, BaseField rangeToField)
		{
			var value = parameters.Field.Value as DateTime? ?? DateTime.MinValue;
			var valueFrom = rangeFromField.Value as DateTime?;
			var valueTo = rangeToField.Value as DateTime?;

			if (BaseRangeValidation<FD, FA, FV, T, D>.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDateTime, parameters.Field.Id, rangeFromField.Id, rangeToField.Id);
		}

		private static ValidationCheckResult CheckRangeBetweenInteger(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField rangeFromField, BaseField rangeToField)
		{
			var value = parameters.Field.Value as int? ?? 0;
			var valueFrom = rangeFromField.Value as int?;
			var valueTo = rangeToField.Value as int?;

			if (BaseRangeValidation<FD, FA, FV, T, D>.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeInteger, parameters.Field.Id, rangeFromField.Id, rangeToField.Id);
		}

		private static ValidationCheckResult CheckRangeBetweenLong(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField rangeFromField, BaseField rangeToField)
		{
			var value = parameters.Field.Value as long? ?? 0;
			var valueFrom = rangeFromField.Value as long?;
			var valueTo = rangeToField.Value as long?;

			if (BaseRangeValidation<FD, FA, FV, T, D>.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeLong, parameters.Field.Id, rangeFromField.Id, rangeToField.Id);
		}

		private static ValidationCheckResult CheckRangeBetweenDecimal(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField rangeFromField, BaseField rangeToField)
		{
			var value = parameters.Field.Value as decimal? ?? 0.0m;
			var valueFrom = rangeFromField.Value as decimal?;
			var valueTo = rangeToField.Value as decimal?;

			if (BaseRangeValidation<FD, FA, FV, T, D>.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDecimal, parameters.Field.Id, rangeFromField.Id, rangeToField.Id);
		}

		private static ValidationCheckResult CheckRangeBetweenDouble(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField rangeFromField, BaseField rangeToField)
		{
			var value = parameters.Field.Value as double? ?? 0.0;
			var valueFrom = rangeFromField.Value as double?;
			var valueTo = rangeToField.Value as double?;

			if (BaseRangeValidation<FD, FA, FV, T, D>.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDouble, parameters.Field.Id, rangeFromField.Id, rangeToField.Id);
		}

		private static ValidationCheckResult CheckRangeBetweenFloat(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField rangeFromField, BaseField rangeToField)
		{
			var value = parameters.Field.Value as float? ?? 0f;
			var valueFrom = rangeFromField.Value as float?;
			var valueTo = rangeToField.Value as float?;

			if (BaseRangeValidation<FD, FA, FV, T, D>.CheckRangeValue(valueFrom, valueTo, parameters.AllowNull, value))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeFloat, parameters.Field.Id, rangeFromField.Id, rangeToField.Id);
		}

		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string fieldId, string fieldFromId, string fieldToId)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationCheckResultEntry>
				{
					new ValidationCheckResultEntry
					{
						MethodType = ValidationMethodType.RangeBetween,
						ErrorType = errorType,
						PropertyName = fieldId,
						PropertyNameFrom = fieldFromId,
						PropertyNameTo = fieldToId
					}
				}
			};
		}
	}
}