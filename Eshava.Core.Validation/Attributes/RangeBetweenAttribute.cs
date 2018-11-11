using System;

namespace Eshava.Core.Validation.Attributes
{
	public class RangeBetweenAttribute : Attribute
	{
		public RangeBetweenAttribute(string propertyNameFrom, string propertyNameTo)
		{
			PropertyNameFrom = propertyNameFrom;
			PropertyNameTo = propertyNameTo;
		}

		/// <summary>
		/// Specifies the property that contains the minimum value of a range of values.
		/// </summary>
		public string PropertyNameFrom { get; }
		/// <summary>
		/// Specifies the property that contains the maximum value of a range of values.
		/// </summary>
		public string PropertyNameTo { get; }
	}
}