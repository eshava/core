using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Extensions;
using Eshava.Core.Models;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class EnumerationValidation
	{
		public static ValidationCheckResult CheckEnumeration(ValidationCheckParameters parameters)
		{
			var dataType = parameters.PropertyInfo.GetDataType();
			if (!dataType.IsEnum || parameters.PropertyValue == null)
			{
				return new ValidationCheckResult();
			}

			var enumAttribute = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(EnumerationAttribute)) as EnumerationAttribute;
			if (enumAttribute?.SkipValidation ?? false)
			{
				return new ValidationCheckResult();
			}

			var invalidateZero = enumAttribute?.InvalidateZero ?? false;
            var flagMode = (enumAttribute?.FlagMode ?? false) || dataType.IsDefined(typeof(FlagsAttribute), false);
			var propertyValue = ConvertToUInt64(parameters.PropertyValue);
			var isValidEnumValue = flagMode
				? IsValidFlagEnumValue(dataType, propertyValue, invalidateZero)
				: IsValidEnumValue(dataType, propertyValue, invalidateZero);

			if (!isValidEnumValue)
			{
				return GetErrorResult(parameters.PropertyInfo.Name, parameters.PropertyValue.ToString());
			}

			return new ValidationCheckResult();
		}

		private static bool IsValidEnumValue(Type dataType, ulong propertyValue, bool invalidateZero)
		{
			foreach (var enumValue in Enum.GetValues(dataType))
			{
				if (ConvertToUInt64(enumValue) == propertyValue && (propertyValue != 0 || !invalidateZero))
				{
					return true;
				}
			}

			return false;
		}

		private static bool IsValidFlagEnumValue(Type dataType, ulong propertyValue, bool invalidateZero)
		{
			if (IsValidEnumValue(dataType, propertyValue, invalidateZero))
			{
				return true;
			}

			if (propertyValue == 0)
			{
				return false;
			}

			var enumValues = Enum.GetValues(dataType)
				.Cast<object>()
				.Select(ConvertToUInt64)
				.Where(enumValue => enumValue != 0 && (enumValue & ~propertyValue) == 0)
				.Distinct()
				.OrderByDescending(enumValue => enumValue)
				.ToArray();

			return IsValidFlagCombination(propertyValue, enumValues, 0, 0);
		}

		private static bool IsValidFlagCombination(ulong propertyValue, ulong[] enumValues, ulong currentValue, int startIndex)
		{
			if (currentValue == propertyValue)
			{
				return true;
			}

			for (var index = startIndex; index < enumValues.Length; index++)
			{
				var nextValue = currentValue | enumValues[index];
				if (nextValue == currentValue)
				{
					continue;
				}

				if (IsValidFlagCombination(propertyValue, enumValues, nextValue, index + 1))
				{
					return true;
				}
			}

			return false;
		}

		private static ulong ConvertToUInt64(object value)
		{
			var valueType = value.GetType();
			var typeCode = Type.GetTypeCode(valueType.IsEnum ? Enum.GetUnderlyingType(valueType) : valueType);

			return typeCode switch
			{
				TypeCode.SByte => unchecked((ulong)Convert.ToSByte(value)),
				TypeCode.Byte => Convert.ToByte(value),
				TypeCode.Int16 => unchecked((ulong)Convert.ToInt16(value)),
				TypeCode.UInt16 => Convert.ToUInt16(value),
				TypeCode.Int32 => unchecked((ulong)Convert.ToInt32(value)),
				TypeCode.UInt32 => Convert.ToUInt32(value),
				TypeCode.Int64 => unchecked((ulong)Convert.ToInt64(value)),
				TypeCode.UInt64 => Convert.ToUInt64(value),
				_ => throw new InvalidOperationException($"The type '{valueType.FullName}' is not a supported enum type.")
			};
		}

		private static ValidationCheckResult GetErrorResult(string propertyName, string propertyValue)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationError>
				{
					new ValidationError
					{
						MethodType = ValidationMethodType.Enumeration.ToString(),
						ErrorType = ValidationErrorType.Invalid.ToString(),
						PropertyName = propertyName,
						Value = propertyValue
					}
				}
			};
		}
	}
}