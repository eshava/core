using System;

namespace Eshava.Core.Validation.Attributes
{
	public class EqualsToAttribute : Attribute
	{
		public EqualsToAttribute(string propertyName)
		{
			PropertyName = propertyName;
		}

		public string PropertyName { get; }
	}
}