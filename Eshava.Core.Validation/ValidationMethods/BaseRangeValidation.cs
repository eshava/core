using System;
using System.Collections.Generic;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class BaseRangeValidation
	{
		public static ValidationCheckResult CheckRangeValue(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			if (parameters.DataType == typeof(float))
			{
				return CheckRangeValueFloat(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (parameters.DataType == typeof(double))
			{
				return CheckRangeValueDouble(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (parameters.DataType == typeof(decimal))
			{
				return CheckRangeValueDecimal(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (parameters.DataType == typeof(int))
			{
				return CheckRangeValueInteger(parameters, propertyInfoFrom, propertyInfoTo);
			}

			if (parameters.DataType == typeof(DateTime))
			{
				return CheckRangeValueDateTime(parameters, propertyInfoFrom, propertyInfoTo);
			}

			return new ValidationCheckResult { ValidationError = $"{nameof(CheckRangeValue)}->{propertyInfoFrom.Name}->DataTypeNotSupported" };
		}

		private static ValidationCheckResult CheckRangeValueFloat(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = (float?)(valueFromObject == null ? (double?)null : Convert.ToDouble(valueFromObject));
			var valueTo = (float?)(valueToObject == null ? (double?)null : Convert.ToDouble(valueToObject));

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return new ValidationCheckResult { ValidationError = $"{nameof(CheckRangeValue)}->{propertyInfoFrom.Name}->CheckRangeValueFloatValue" };
		}

		private static ValidationCheckResult CheckRangeValueDouble(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (double?)null : Convert.ToDouble(valueFromObject);
			var valueTo = valueToObject == null ? (double?)null : Convert.ToDouble(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return new ValidationCheckResult { ValidationError = $"{nameof(CheckRangeValue)}->{propertyInfoFrom.Name}->CheckRangeValueDoubleValue" };
		}

		private static ValidationCheckResult CheckRangeValueDecimal(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (decimal?)null : Convert.ToDecimal(valueFromObject);
			var valueTo = valueToObject == null ? (decimal?)null : Convert.ToDecimal(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return new ValidationCheckResult { ValidationError = $"{nameof(CheckRangeValue)}->{propertyInfoFrom.Name}->CheckRangeValueDecimalValue" };
		}

		private static ValidationCheckResult CheckRangeValueInteger(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (int?)null : Convert.ToInt32(valueFromObject);
			var valueTo = valueToObject == null ? (int?)null : Convert.ToInt32(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return new ValidationCheckResult { ValidationError = $"{nameof(CheckRangeValue)}->{propertyInfoFrom.Name}->CheckRangeValueIntegerValue" };
		}

		private static ValidationCheckResult CheckRangeValueDateTime(ValidationCheckParameters parameters, PropertyInfo propertyInfoFrom, PropertyInfo propertyInfoTo)
		{
			var valueFromObject = propertyInfoFrom.GetValue(parameters.Model);
			var valueToObject = propertyInfoTo.GetValue(parameters.Model);
			var valueFrom = valueFromObject == null ? (DateTime?)null : Convert.ToDateTime(valueFromObject);
			var valueTo = valueToObject == null ? (DateTime?)null : Convert.ToDateTime(valueToObject);

			if (CheckRangeValue(valueFrom, valueTo, parameters.AllowNull))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return new ValidationCheckResult { ValidationError = $"{nameof(CheckRangeValue)}->{propertyInfoFrom.Name}->CheckRangeValueDateTimeValue" };
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
	}
}