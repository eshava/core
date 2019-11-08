using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Extension;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class StringValidation
	{
		private static Type _typeString = typeof(string);
		
		public static ValidationCheckResult CheckStringLength(ValidationCheckParameters parameters)
		{
			if (parameters.PropertyInfo.PropertyType.GetDataTypeFromIEnumerable() != _typeString)
			{
				return new ValidationCheckResult();
			}

			var valueString = parameters.PropertyValue as string;
			if (valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult();
			}

			var maxLength = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(MaxLengthAttribute)) as MaxLengthAttribute;
			var minLength = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(MinLengthAttribute)) as MinLengthAttribute;

			if (maxLength != null && valueString.Length > maxLength.Length)
			{
				return GetErrorResult(ValidationErrorType.GreaterMaxLength, parameters.PropertyInfo.Name);
			}

			if (minLength != null && valueString.Length < minLength.Length)
			{
				return GetErrorResult(ValidationErrorType.LowerMinLength, parameters.PropertyInfo.Name);
			}

			return new ValidationCheckResult();
		}

		public static ValidationCheckResult CheckMailAddress(ValidationCheckParameters parameters)
		{
			if (parameters.PropertyInfo.PropertyType.GetDataTypeFromIEnumerable() != _typeString)
			{
				return new ValidationCheckResult();
			}

			var dataType = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(DataTypeAttribute)) as DataTypeAttribute;

			if (dataType != null && dataType.DataType == DataType.EmailAddress)
			{
				var valueString = parameters.PropertyValue as string;
				if (!valueString.IsNullOrEmpty() && !valueString.IsEmailAddress())
				{
					return GetErrorResult(ValidationErrorType.NoWellFormedMailAddress, parameters.PropertyInfo.Name);
				}
			}

			return new ValidationCheckResult();
		}
		public static ValidationCheckResult CheckUrl(ValidationCheckParameters parameters)
		{
			if (parameters.PropertyInfo.PropertyType.GetDataTypeFromIEnumerable() != _typeString)
			{
				return new ValidationCheckResult();
			}

			var dataType = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(DataTypeAttribute)) as DataTypeAttribute;

			if (dataType != null && dataType.DataType == DataType.Url)
			{
				var valueString = parameters.PropertyValue as string;
				if (!valueString.IsNullOrEmpty() && !valueString.IsUrl())
				{
					return GetErrorResult(ValidationErrorType.NoWellFormedUri, parameters.PropertyInfo.Name);
				}
			}

			return new ValidationCheckResult();
		}

		private static ValidationCheckResult GetErrorResult(ValidationErrorType errorType, string propertyName)
		{
			return new ValidationCheckResult
			{
				ValidationErrors = new List<ValidationCheckResultEntry>
				{
					new ValidationCheckResultEntry
					{
						MethodType = ValidationMethodType.String,
						ErrorType = errorType,
						PropertyName = propertyName
					}
				}
			};
		}
	}
}