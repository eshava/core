using System;

namespace Eshava.Core.Validation.Attributes
{
	public class NotEqualsToAttribute : Attribute
	{
		public NotEqualsToAttribute(string propertyName)
		{
			PropertyName = propertyName;
		}

		public NotEqualsToAttribute(string propertyName, object defaultValue)
		{
			PropertyName = propertyName;
			DefaultValue = defaultValue;
		}

		public string PropertyName { get; }
		public object DefaultValue { get; }
	}
}