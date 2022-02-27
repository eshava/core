using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Models;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.ValidationMethods
{
	internal static class RangeValidation<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		private static List<(Func<Type, bool> Check, Func<ValidationCheckParameters<FD, FA, FV, T, D>, IFieldConfiguration<T>, IFieldConfiguration<T>, ValidationCheckResult> Validate)> _validationRules =
			new List<(Func<Type, bool> Check, Func<ValidationCheckParameters<FD, FA, FV, T, D>, IFieldConfiguration<T>, IFieldConfiguration<T>, ValidationCheckResult> Validate)>
			{
				(type => type == typeof(float), (parameters, minimumRule, maximumRule) => CheckRangeFloat(parameters, minimumRule, maximumRule)),
				(type => type == typeof(double), (parameters, minimumRule, maximumRule) => CheckRangeDouble(parameters, minimumRule, maximumRule)),
				(type => type == typeof(decimal), (parameters, minimumRule, maximumRule) => CheckRangeDecimal(parameters, minimumRule, maximumRule)),
				(type => type == typeof(int) || type.IsEnum, (parameters, minimumRule, maximumRule) => CheckRangeInteger(parameters, minimumRule, maximumRule)),
				(type => type == typeof(long), (parameters, minimumRule, maximumRule) => CheckRangeLong(parameters, minimumRule, maximumRule))
			};

		public static ValidationCheckResult CheckRange(ValidationCheckParameters<FD, FA, FV, T, D> parameters)
		{
			var minimumRule = parameters.GetConfigurations(FieldConfigurationType.Minimum).FirstOrDefault();
			var maximumRule = parameters.GetConfigurations(FieldConfigurationType.Maximum).FirstOrDefault();

			if (parameters.Field.Value == null || (minimumRule == null && maximumRule == null))
			{
				return new ValidationCheckResult();
			}

			var validationRule = _validationRules.SingleOrDefault(rule => rule.Check(parameters.Field.Type));
			if (validationRule.Validate != null)
			{
				return validationRule.Validate(parameters, minimumRule, maximumRule);
			}

			return GetErrorResult(ValidationErrorType.DataTypeNotSupported, parameters.Field.Id);
		}

		private static ValidationCheckResult CheckRangeInteger(ValidationCheckParameters<FD, FA, FV, T, D> parameters, IFieldConfiguration<T> minimumRule, IFieldConfiguration<T> maximumRule)
		{
			var valueInteger = (int)parameters.Field.Value;
			var minimum = minimumRule.ValueInteger ?? 0;
			var maximum = maximumRule.ValueInteger ?? 0;

			if (valueInteger < minimum || valueInteger > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeInteger, parameters.Field.Id);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckRangeLong(ValidationCheckParameters<FD, FA, FV, T, D> parameters, IFieldConfiguration<T> minimumRule, IFieldConfiguration<T> maximumRule)
		{
			var valueInt = (long)parameters.Field.Value;
			var minimum = minimumRule.ValueLong ?? 0;
			var maximum = maximumRule.ValueLong ?? 0;

			if (valueInt < minimum || valueInt > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeLong, parameters.Field.Id);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckRangeFloat(ValidationCheckParameters<FD, FA, FV, T, D> parameters, IFieldConfiguration<T> minimumRule, IFieldConfiguration<T> maximumRule)
		{
			var valueFloat = (float)parameters.Field.Value;
			var minimum = minimumRule.ValueFloat ?? 0;
			var maximum = maximumRule.ValueFloat ?? 0;

			if (valueFloat < minimum || valueFloat > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeFloat, parameters.Field.Id);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckRangeDouble(ValidationCheckParameters<FD, FA, FV, T, D> parameters, IFieldConfiguration<T> minimumRule, IFieldConfiguration<T> maximumRule)
		{
			var valueDouble = (double)parameters.Field.Value;
			var minimum = minimumRule.ValueDouble ?? 0;
			var maximum = maximumRule.ValueDouble ?? 0;

			if (valueDouble < minimum || valueDouble > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeDouble, parameters.Field.Id);
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult CheckRangeDecimal(ValidationCheckParameters<FD, FA, FV, T, D> parameters, IFieldConfiguration<T> minimumRule, IFieldConfiguration<T> maximumRule)
		{
			var valueDecimal = (decimal)parameters.Field.Value;
			var minimum = minimumRule.ValueDecimal ?? 0;
			var maximum = maximumRule.ValueDecimal ?? 0;

			if (valueDecimal < minimum || valueDecimal > maximum)
			{
				return GetErrorResult(ValidationErrorType.DataTypeDecimal, parameters.Field.Id);
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
						MethodType = ValidationMethodType.RangeHardCoded.ToString(),
						ErrorType = errorType.ToString(),
						PropertyName = fieldId
					}
				}
			};
		}
	}
}