using System;
using Eshava.Core.Extensions;

namespace Eshava.Core.Validation.Extension
{
	public static class TypeExtension
	{
		public static bool IsComplexDataType(this Type type)
		{
			return type.IsClass && type != typeof(DateTime) && type != typeof(string);
		}

		public static bool IsEnum(this Type type)
		{
			type = type.GetDataType();

			return type.IsEnum;
		}

		public static bool IsNumber(this Type type)
		{
			type = type.GetDataType();

			return type == typeof(int) || type == typeof(long) || type == typeof(decimal) || type == typeof(double) || type == typeof(float);
		}

		public static bool IsBoolean(this Type type)
		{
			type = type.GetDataType();

			return type == typeof(bool);
		}

		public static bool IsDateTime(this Type type)
		{
			type = type.GetDataType();

			return type == typeof(DateTime);
		}

		public static bool IsGuid(this Type type)
		{
			type = type.GetDataType();

			return type == typeof(Guid);
		}
	}
}