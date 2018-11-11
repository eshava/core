using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.ValidationMethods
{
	internal static class StringValidation
	{
		public static ValidationCheckResult CheckStringLength(ValidationCheckParameters parameters)
		{
			if (parameters.PropertyInfo.PropertyType.GetDataTypeFromIEnumerable() != typeof(string))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var valueString = parameters.PropertyValue as string;
			if (valueString.IsNullOrEmpty())
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var maxLength = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(MaxLengthAttribute)) as MaxLengthAttribute;
			var minLength = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(MinLengthAttribute)) as MinLengthAttribute;

			if (maxLength != null && valueString.Length > maxLength.Length)
			{
				return new ValidationCheckResult { ValidationError = $"{nameof(CheckStringLength)}->{parameters.PropertyInfo.Name}->GreaterMaxLength" };
			}

			if (minLength != null && valueString.Length < minLength.Length)
			{
				return new ValidationCheckResult { ValidationError = $"{nameof(CheckStringLength)}->{parameters.PropertyInfo.Name}->LowerMinLength" };
			}

			return new ValidationCheckResult { IsValid = true };
		}

		public static ValidationCheckResult CheckMailAddress(ValidationCheckParameters parameters)
		{
			if (parameters.PropertyInfo.PropertyType != typeof(string))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var dataType = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(DataTypeAttribute)) as DataTypeAttribute;

			if (dataType != null && dataType.DataType == DataType.EmailAddress)
			{
				var valueString = parameters.PropertyValue as string;
				if (!valueString.IsNullOrEmpty())
				{
					var regex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
											@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);
					var match = regex.Match(valueString);

					if (!match.Success)
					{
						return new ValidationCheckResult { ValidationError = $"{nameof(CheckMailAddress)}->{parameters.PropertyInfo.Name}->NotMatch" };
					}
				}
			}

			return new ValidationCheckResult { IsValid = true };
		}
		public static ValidationCheckResult CheckUrl(ValidationCheckParameters parameters)
		{
			if (parameters.PropertyInfo.PropertyType != typeof(string))
			{
				return new ValidationCheckResult { IsValid = true };
			}

			var dataType = Attribute.GetCustomAttribute(parameters.PropertyInfo, typeof(DataTypeAttribute)) as DataTypeAttribute;

			if (dataType != null && dataType.DataType == DataType.Url)
			{
				var valueString = parameters.PropertyValue as string;
				if (!valueString.IsNullOrEmpty())
				{
					if (valueString.Contains("@"))
					{
						return new ValidationCheckResult { ValidationError = $"{nameof(CheckUrl)}->{parameters.PropertyInfo.Name}->NoWellFormedUri" };
					}

					if (!valueString.StartsWith("http://") && !valueString.StartsWith("https://"))
					{
						valueString = "http://" + valueString;
					}

					var isValid = Uri.TryCreate(valueString, UriKind.Absolute, out var outputUri) &&
								  outputUri.Host.Replace("www.", "").Split('.').Length > 1 &&
								  outputUri.HostNameType == UriHostNameType.Dns &&
								  outputUri.Host.Length > outputUri.Host.LastIndexOf(".", StringComparison.Ordinal) + 1 &&
								  255 >= valueString.Length;

					if (!isValid)
					{
						return new ValidationCheckResult { ValidationError = $"{nameof(CheckUrl)}->{parameters.PropertyInfo.Name}->NoWellFormedUri" };
					}

				}
			}

			return new ValidationCheckResult { IsValid = true };
		}
	}
}