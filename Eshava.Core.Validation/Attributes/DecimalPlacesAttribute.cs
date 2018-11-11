using System;

namespace Eshava.Core.Validation.Attributes
{
	public class DecimalPlacesAttribute : Attribute
	{
		public DecimalPlacesAttribute(int decimalPlaces)
		{
			DecimalPlaces = decimalPlaces;
		}
		public int DecimalPlaces { get; }
	}
}