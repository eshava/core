using System;
using System.Collections;
using System.Linq;

namespace Eshava.Core.Extensions
{
	public static class TypeExtension
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <exception cref="ArgumentNullException">Thrown if type is null</exception>
		/// <returns></returns>
		public static Type GetDataType(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type.IsDataTypeNullable())
			{
				type = Nullable.GetUnderlyingType(type);
			}

			return type;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <exception cref="ArgumentNullException">Thrown if type is null</exception>
		/// <returns></returns>
		public static Type GetDataTypeFromIEnumerable(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type.ImplementsIEnumerable())
			{
				type = type.GetGenericArguments()[0];
			}

			return type;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <exception cref="ArgumentNullException">Thrown if type is null</exception>
		/// <returns></returns>
		public static bool IsDataTypeNullable(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <exception cref="ArgumentNullException">Thrown if type is null</exception>
		/// <returns></returns>
		public static bool ImplementsIEnumerable(this Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return type.IsGenericType && ImplementsInterface(type, typeof(IEnumerable));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="interfaceType"></param>
		/// <exception cref="ArgumentNullException">Thrown if type or interfaceType is null</exception>
		/// <returns></returns>
		public static bool ImplementsInterface(this Type type, Type interfaceType)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (interfaceType == null)
			{
				throw new ArgumentNullException(nameof(interfaceType));
			}

			return type.GetInterfaces().Any(t => t == interfaceType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="args"></param>
		/// <exception cref="ArgumentNullException">Thrown if type is null</exception>
		/// <returns></returns>
		public static object CreateInstance(this Type type, params object[] args)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return Activator.CreateInstance(type, args);
		}
	}
}