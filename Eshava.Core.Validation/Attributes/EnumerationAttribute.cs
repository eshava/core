using System;

namespace Eshava.Core.Validation.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class EnumerationAttribute : Attribute
	{
		public EnumerationAttribute(bool skipValidation = false, bool invalidateZero = false, bool flagMode = false)
		{
			SkipValidation = skipValidation;
			InvalidateZero = invalidateZero;
			FlagMode = flagMode;
		}

		public bool SkipValidation { get; }
		public bool InvalidateZero { get; }
		public bool FlagMode { get; }
	}
}