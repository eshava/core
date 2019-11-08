using System;
using System.Reflection;

namespace Eshava.Core.Validation.Extension
{
	public static class PropertyInfoExtension
	{
		public static bool HasAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
		{
			return Attribute.GetCustomAttribute(propertyInfo, typeof(T)) != null;
		}
	}
}