using System;
using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation.Models;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.ValidationMethods
{
	internal static class BaseRangeValidation<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		public static ValidationCheckResult CheckRangeValue(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldFrom, BaseField fieldTo)
		{
			if (fieldFrom.Type != fieldTo.Type)
			{
				return GetErrorResult(ValidationErrorType.DataTypesNotEqual, fieldFrom.Id, fieldTo.Id);
			}

			if (fieldFrom.Type == typeof(float))
			{
				return CheckRangeValueFloat(parameters, fieldFrom, fieldTo);
			}

			if (fieldFrom.Type == typeof(double))
			{
				return CheckRangeValueDouble(parameters, fieldFrom, fieldTo);
			}

			if (fieldFrom.Type == typeof(decimal))
			{
				return CheckRangeValueDecimal(parameters, fieldFrom, fieldTo);
			}

			if (fieldFrom.Type == typeof(int) || fieldFrom.Type.IsEnum)
			{
				return CheckRangeValueInteger(parameters, fieldFrom, fieldTo);
			}

			if (fieldFrom.Type == typeof(long))
			{
				return CheckRangeValueLong(parameters, fieldFrom, fieldTo);
			}

			if (fieldFrom.Type == typeof(DateTime))
			{
				return CheckRangeValueDateTime(parameters, fieldFrom, fieldTo);
			}

			return GetErrorResult(ValidationErrorType.DataTypeNotSupported, fieldFrom.Id, fieldTo.Id);
		}

		private static ValidationCheckResult CheckRangeValueFloat(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldFrom, BaseField fieldTo)
		{
			var valueFromObject = fieldFrom.Value;
			var valueToObject = fieldTo.Value;
			var valueFrom = (float?)(valueFromObject == null ? (double?)null : Convert.ToDouble(valueFromObject));
			var valueTo = (float?)(valueToObject == null ? (double?)null : Convert.ToDouble(valueToObject));

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeFloat, fieldFrom.Id, fieldTo.Id);
		}

		private static ValidationCheckResult CheckRangeValueDouble(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldFrom, BaseField fieldTo)
		{
			var valueFromObject = fieldFrom.Value;
			var valueToObject = fieldTo.Value;
			var valueFrom = valueFromObject == null ? (double?)null : Convert.ToDouble(valueFromObject);
			var valueTo = valueToObject == null ? (double?)null : Convert.ToDouble(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDouble, fieldFrom.Id, fieldTo.Id);
		}

		private static ValidationCheckResult CheckRangeValueDecimal(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldFrom, BaseField fieldTo)
		{
			var valueFromObject = fieldFrom.Value;
			var valueToObject = fieldTo.Value;
			var valueFrom = valueFromObject == null ? (decimal?)null : Convert.ToDecimal(valueFromObject);
			var valueTo = valueToObject == null ? (decimal?)null : Convert.ToDecimal(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDecimal, fieldFrom.Id, fieldTo.Id);
		}

		private static ValidationCheckResult CheckRangeValueInteger(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldFrom, BaseField fieldTo)
		{
			var valueFromObject = fieldFrom.Value;
			var valueToObject = fieldTo.Value;
			var valueFrom = valueFromObject == null ? (int?)null : Convert.ToInt32(valueFromObject);
			var valueTo = valueToObject == null ? (int?)null : Convert.ToInt32(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeInteger, fieldFrom.Id, fieldTo.Id);
		}

		private static ValidationCheckResult CheckRangeValueLong(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldFrom, BaseField fieldTo)
		{
			var valueFromObject = fieldFrom.Value;
			var valueToObject = fieldTo.Value;
			var valueFrom = valueFromObject == null ? (long?)null : Convert.ToInt64(valueFromObject);
			var valueTo = valueToObject == null ? (long?)null : Convert.ToInt64(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeLong, fieldFrom.Id, fieldTo.Id);
		}

		private static ValidationCheckResult CheckRangeValueDateTime(ValidationCheckParameters<FD, FA, FV, T, D> parameters, BaseField fieldFrom, BaseField fieldTo)
		{
			var valueFromObject = fieldFrom.Value;
			var valueToObject = fieldTo.Value;
			var valueFrom = valueFromObject == null ? (DateTime?)null : Convert.ToDateTime(valueFromObject);
			var valueTo = valueToObject == null ? (DateTime?)null : Convert.ToDateTime(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDateTime, fieldFrom.Id, fieldTo.Id);
		}

		public static bool CheckRangeValue<T>(T? valueFrom, T? valueTo, bool allowNull, T? valueCurrent = null) where T : struct
		{
			if (typeof(T).IsDataTypeNullable())
			{
				throw new NotSupportedException("Nullable data types are not supported.");
			}

			if ((valueFrom.HasValue && !valueTo.HasValue && !allowNull) ||
				(!valueFrom.HasValue && valueTo.HasValue && !allowNull) ||
				(valueFrom.HasValue && valueTo.HasValue && Comparer<T>.Default.Compare(valueFrom.Value, valueTo.Value) == 1))
			{
				return false;
			}

			if (valueCurrent.HasValue)
			{
				//No limit has been set, so the current value is always valid.
				if (!valueFrom.HasValue)
				{
					return true;
				}

				return Comparer<T>.Default.Compare(valueFrom.Value, valueCurrent.Value) <= 0 && Comparer<T>.Default.Compare(valueTo.Value, valueCurrent.Value) >= 0;
			}

			return true;
		}

		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string fieldFromId, string fieldToId)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationCheckResultEntry>
				{
					new ValidationCheckResultEntry
					{
						MethodType = ValidationMethodType.Range,
						ErrorType = errorType,
						PropertyNameFrom = fieldFromId,
						PropertyNameTo = fieldToId
					}
				}
			};
		}
	}
}