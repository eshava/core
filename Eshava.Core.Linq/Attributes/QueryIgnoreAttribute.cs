using System;

namespace Eshava.Core.Linq.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class QueryIgnoreAttribute : Attribute
	{
	}
}