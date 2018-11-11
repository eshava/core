using System;
using System.Reflection;

namespace Eshava.Core.Extensions
{
	public static class PropertyInfoExtension
	{
		/// <summary>
		/// Return the data type of the property
		/// </summary>
		/// <param name="propertyInfo">propertyInfo</param>
		/// <exception cref="ArgumentNullException">Thrown if propertyInfo is null</exception>
		/// <returns>data type</returns>
		public static Type GetDataType(this PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException(nameof(propertyInfo));
			}

			return propertyInfo.PropertyType.GetDataType();
		}
	}
}