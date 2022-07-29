using System;

namespace Eshava.Core.Validation.Attributes
{
	public class EnumerationAttribute : Attribute
	{
		public EnumerationAttribute(bool skipValidation = false, bool invalidateZero = false)
		{
			SkipValidation = skipValidation;
			InvalidateZero = invalidateZero;
		}

		public bool SkipValidation { get; }
		public bool InvalidateZero { get; }
	}
}