using System;

namespace Eshava.Core.Validation.Attributes
{
	public abstract class AbstractRangeFromOrToAttribute : Attribute
	{
		public AbstractRangeFromOrToAttribute(string propertyName, bool allowNull)
		{
			PropertyName = propertyName;
			AllowNull = allowNull;
		}

		/// <summary>
		/// Specifies the field that contains the minimum/maximum value of a range of values.
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// Specifies whether a value of the rank must not be set.
		/// </summary>
		public bool AllowNull { get; }
	}
}