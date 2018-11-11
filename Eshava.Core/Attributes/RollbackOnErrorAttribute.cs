using System;

namespace Eshava.Core.Attributes
{
	public class RollbackOnErrorAttribute : Attribute
	{
		public RollbackOnErrorAttribute(object expectedValue)
		{
			ExpectedValue = expectedValue;
			PropertyName = null;
			IsObject = false;
		}

		public RollbackOnErrorAttribute(object expectedValue, string propertyname)
		{
			ExpectedValue = expectedValue;
			PropertyName = propertyname;
			IsObject = true;
		}

		public object ExpectedValue { get; }
		public string PropertyName { get; }
		public bool IsObject { get; }
	}
}