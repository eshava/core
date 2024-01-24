using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Eshava.Core.Linq.Constants
{
	internal static class MethodInfoConstants
	{
		public static readonly MethodInfo StringContains = TypeConstants.String.GetMethod(nameof(String.Contains), [TypeConstants.String]);
		public static readonly MethodInfo StringStartsWith = TypeConstants.String.GetMethod(nameof(String.StartsWith), [TypeConstants.String]);
		public static readonly MethodInfo StringEndsWith = TypeConstants.String.GetMethod(nameof(String.EndsWith), [TypeConstants.String]);
		public static readonly MethodInfo StringToLower = TypeConstants.String.GetMethod(nameof(String.ToLower), []);

		public static readonly MethodInfo Any = typeof(Enumerable)
				.GetMethods()
				.FirstOrDefault(m => m.Name == "Any"
					&& m.GetParameters().Length == 2
					&& m.GetParameters()[0].ParameterType.IsGenericType
					&& m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
					&& m.GetParameters()[1].ParameterType.IsGenericType
					&& m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
				)
				;

		public static readonly MethodInfo All = typeof(Enumerable)
				.GetMethods()
				.FirstOrDefault(m => m.Name == "All"
					&& m.GetParameters().Length == 2
					&& m.GetParameters()[0].ParameterType.IsGenericType
					&& m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
					&& m.GetParameters()[1].ParameterType.IsGenericType
					&& m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
				)
				;

		public static MethodInfo GetGenericAny(Type type)
		{
			return Any.MakeGenericMethod(type);
		}

		public static MethodInfo GetGenericAll(Type type)
		{
			return All.MakeGenericMethod(type);
		}
	}
}