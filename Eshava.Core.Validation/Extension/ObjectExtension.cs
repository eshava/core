using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Eshava.Core.Extensions;

namespace Eshava.Core.Validation.Extension
{
	public static class ObjectExtension
	{
		public static object CleanUpAndSubstringStringProperties(this object model)
		{
			if (model == null)
			{
				return null;
			}

			foreach (var propertyInfo in model.GetType().GetProperties().Where(p => p.CanRead))
			{
				if (propertyInfo.PropertyType == typeof(string))
				{
					if (propertyInfo.CanWrite)
					{
						var valueString = propertyInfo.GetValue(model) as string;
						propertyInfo.SetValue(model, GetCleanedAndTruncatedString(propertyInfo, valueString));
					}
				}
				else if (propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.ImplementsIEnumerable())
				{
					var subModel = propertyInfo.GetValue(model);
					CleanUpAndSubstringStringProperties(model, subModel, propertyInfo);
				}
			}

			return model;
		}

		private static void CleanUpAndSubstringStringProperties(object model, object propertyValue, PropertyInfo propertyInfo)
		{
			if (propertyValue == null)
			{
				return;
			}

			if (propertyInfo.PropertyType.ImplementsIEnumerable())
			{
				var dataType = propertyInfo.PropertyType.GetDataTypeFromIEnumerable();

				if (dataType.IsClass && propertyValue is IEnumerable elements)
				{
					if (dataType == typeof(string))
					{
						if (propertyInfo.CanWrite)
						{
							IEnumerable<string> newEnumerable = elements.Cast<string>().Select(s => GetCleanedAndTruncatedString(propertyInfo, s)).ToList();
							propertyInfo.SetValue(model, newEnumerable);
						}
					}
					else
					{
						foreach (var element in elements)
						{
							CleanUpAndSubstringStringProperties(element);
						}
					}
				}
			}
			else
			{
				CleanUpAndSubstringStringProperties(propertyValue);
			}
		}

		private static string GetCleanedAndTruncatedString(PropertyInfo propertyInfo, string valueString)
		{
			if (valueString.IsNullOrEmpty())
			{
				return null;
			}

			var maxLength = Attribute.GetCustomAttribute(propertyInfo, typeof(MaxLengthAttribute)) as MaxLengthAttribute;

			if (maxLength != null && valueString.Length > maxLength.Length)
			{
				valueString = valueString.Substring(0, maxLength.Length).Trim();
			}

			return valueString.ReturnNullByEmpty();
		}
	}
}