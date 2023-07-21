using System;

namespace Eshava.Core.Storage.Sql.Extensions
{
	internal static class StringExtentions
	{
		public static bool IsNullOrEmpty(this string value) => String.IsNullOrEmpty(value);
	}
}