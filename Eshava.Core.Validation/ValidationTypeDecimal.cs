using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eshava.Core.Validation
{
	public class ValidationTypeDecimal
	{
		public static ValidationResult ValidateGreaterOrEqualToZero(decimal value, ValidationContext context)
		{
			if (value >= Decimal.Zero)
			{
				return ValidationResult.Success;
			}

			return new ValidationResult($"The field {context.MemberName} must be greater than or equal to 0.", new List<string> { context.MemberName });
		}

		public static ValidationResult ValidateGreaterZero(decimal value, ValidationContext context)
		{
			if (value > Decimal.Zero)
			{
				return ValidationResult.Success;
			}

			return new ValidationResult($"The field {context.MemberName} must be greater than 0.", new List<string> { context.MemberName });
		}
	}
}