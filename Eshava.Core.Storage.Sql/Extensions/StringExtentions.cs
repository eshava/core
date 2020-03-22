using System;

namespace Eshava.Core.Storage.Sql.Extensions
{
	internal static class StringExtentions
	{
		public static bool IsNullOrEmpty(this string value) => String.IsNullOrEmpty(value);
		public static string SetValidDirectoryPathEnd(this string path) => path.EndsWith("\\") ? path : path + "\\";
	}
}