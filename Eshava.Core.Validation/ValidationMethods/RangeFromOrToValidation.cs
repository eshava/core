using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Enums;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class RangeFromOrToValidation
	{
		public static ValidationCheckResult CheckRangeFrom(ValidationCheckParameters parameters)
		{
			return CheckRangeFromOrTo<RangeFromAttribute>(parameters, false);
		}

		public static ValidationCheckResult CheckRangeTo(ValidationCheckParameters parameters)
		{
			return CheckRangeFromOrTo<RangeToAttribute>(parameters, true);
		}

		private static ValidationCheckResult CheckRangeFromOrTo<T>(ValidationCheckParameters parameters, bool invertProperties, [CallerMemberName] string memberName = null) where T : AbstractRangeFromOrToAttribute
		{
			var propertyInfoTarget = parameters.PropertyInfo;
			var rangeSource = Attribute.GetCustomAttribute(propertyInfoTarget, typeof(T)) as AbstractRangeFromOrToAttribute;
			var dataType = parameters.PropertyInfo.GetDataType();

			if (rangeSource == null)
			{
				return new ValidationCheckResult { IsValid = true };
			}

			//Determining the proterty for the start value of the value range
			var propertiesSource = rangeSource.PropertyName.Contains(",") ? rangeSource.PropertyName.Split(',') : new[] { rangeSource.PropertyName };
			var results = new List<ValidationCheckResultEntry>();

			foreach (var propertySource in propertiesSource)
			{
				var propertyInfoSource = parameters.DataType.GetProperty(propertySource.Trim());

				if (propertyInfoSource == null)
				{
					results.Add(GetErrorResult(ValidationErrorType.PropertyNotFoundFrom, propertySource.Trim(), propertyInfoTarget.Name));

					continue;
				}

				var dataTypeFrom = propertyInfoSource.PropertyType.GetDataType();

				//Check whether the data types match
				if (dataType != dataTypeFrom)
				{
					results.Add(GetErrorResult(ValidationErrorType.DataTypesNotEqual, propertyInfoSource.Name, propertyInfoTarget.Name));

					continue;
				}

				parameters.AllowNull = rangeSource.AllowNull;

				if (invertProperties)
				{
					var result = BaseRangeValidation.CheckRangeValue(parameters, propertyInfoTarget, propertyInfoSource);
					if (!result.IsValid)
					{
						results.AddRange(result.ValidationErrors);
					}
				}
				else
				{
					var result = BaseRangeValidation.CheckRangeValue(parameters, propertyInfoSource, propertyInfoTarget);
					if (!result.IsValid)
					{
						results.AddRange(result.ValidationErrors);
					}
				}
			}

			if (results.Count == 0)
			{
				return new ValidationCheckResult { IsValid = true };
			}

			return new ValidationCheckResult { ValidationErrors = results };
		}

		private static ValidationCheckResultEntry GetErrorResult(ValidationErrorType errorType, string propertyNameFrom, string propertyNameTo)
		{
			return new ValidationCheckResultEntry
			{
				MethodType = ValidationMethodType.RangeFromOrTo,
				ErrorType = errorType,
				PropertyNameFrom = propertyNameFrom,
				PropertyNameTo = propertyNameTo
			};
		}
	}
}