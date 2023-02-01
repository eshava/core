using System;

namespace Eshava.Core.Linq.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class AllowedComplexFilterFieldAttribute : Attribute
	{
		public AllowedComplexFilterFieldAttribute(string field)
		{
			Field = field;
		}

		public string Field { get; }
	}
}