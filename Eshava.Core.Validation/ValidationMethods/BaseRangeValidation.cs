using System;
using System.Collections.Generic;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class BaseRangeValidation
	{
		public static ValidationCheckResult CheckRangeValue(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var datatypeFrom = propertyInfoFrom.GetDataType();
			var datatypeTo = propertyInfoTo.GetDataType();

			if (datatypeFrom != datatypeTo)
			{
				return GetErrorResult(ValidationErrorType.DataTypesNotEqual, propertyInfoFrom.Name, propertyInfoTo.Name);
			}

			if (datatypeFrom == typeof(float))
			{
				return CheckRangeValueFloat(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (datatypeFrom == typeof(double))
			{
				return CheckRangeValueDouble(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (datatypeFrom == typeof(decimal))
			{
				return CheckRangeValueDecimal(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (datatypeFrom == typeof(int) || parameters.DataType.IsEnum)
			{
				return CheckRangeValueInteger(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (datatypeFrom == typeof(long))
			{
				return CheckRangeValueLong(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (datatypeFrom == typeof(DateTime))
			{
				return CheckRangeValueDateTime(parameters, propertyInfoFrom, propertyInfoTo);
			}

			return GetErrorResult(ValidationErrorType.DataTypeNotSupported, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeValueFloat(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = (float?)(valueFromObject == null ? (double?)null : Convert.ToDouble(valueFromObject));
			var valueTo = (float?)(valueToObject == null ? (double?)null : Convert.ToDouble(valueToObject));

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeFloat, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeValueDouble(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (double?)null : Convert.ToDouble(valueFromObject);
			var valueTo = valueToObject == null ? (double?)null : Convert.ToDouble(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDouble, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeValueDecimal(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (decimal?)null : Convert.ToDecimal(valueFromObject);
			var valueTo = valueToObject == null ? (decimal?)null : Convert.ToDecimal(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDecimal, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeValueInteger(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (int?)null : Convert.ToInt32(valueFromObject);
			var valueTo = valueToObject == null ? (int?)null : Convert.ToInt32(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeInteger, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeValueLong(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (long?)null : Convert.ToInt64(valueFromObject);
			var valueTo = valueToObject == null ? (long?)null : Convert.ToInt64(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeLong, propertyInfoFrom.Name, propertyInfoTo.Name);
		}

		private static ValidationCheckResult CheckRangeValueDateTime(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (DateTime?)null : Convert.ToDateTime(valueFromObject);
			var valueTo = valueToObject == null ? (DateTime?)null : Convert.ToDateTime(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult();
			}

			return GetErrorResult(ValidationErrorType.DataTypeDateTime, propertyInfoFrom.Name, propertyInfoTo.Name);
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
				return (!valueFrom.HasValue || Comparer<T>.Default.Compare(valueFrom.Value, valueCurrent.Value) <= 0)
					   &&
					   (!valueTo.HasValue || Comparer<T>.Default.Compare(valueTo.Value, valueCurrent.Value) >= 0)
					   ;
			}

			return true;
		}

		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string propertyNameFrom, string propertyNameTo)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationCheckResultEntry>
				{
					new ValidationCheckResultEntry
					{
						MethodType = ValidationMethodType.Range,
						ErrorType = errorType,
						PropertyNameFrom = propertyNameFrom,
						PropertyNameTo = propertyNameTo
					}
				}
			};
		}
	}
}