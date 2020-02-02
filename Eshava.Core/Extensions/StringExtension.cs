using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Eshava.Core.Extensions
{
	public static class StringExtension
	{
		public static bool IsNullOrEmpty(this string value) => String.IsNullOrEmpty(value);
		public static string SetValidDirectoryPathEnd(this string path) => path.EndsWith("\\") ? path : path + "\\";

		public static byte[] FromBase64(this string base64Value) => base64Value.IsNullOrEmpty() ? Array.Empty<byte>() : Convert.FromBase64String(base64Value);

		/// <summary>
		/// Measure the difference
		/// </summary>
		/// <param name="source">source string</param>
		/// <param name="target">target string</param>
		/// <returns>0 is different, 1 is equals</returns>
		public static double GetCompareResultByLevenshteinAlgorithm(this string source, string target)
		{
			if (source.IsNullOrEmpty())
			{
				return target.IsNullOrEmpty() ? 1 : 0;
			}

			if (target.IsNullOrEmpty())
			{
				return source.IsNullOrEmpty() ? 1 : 0;
			}

			source = source.ToLowerInvariant().Trim();
			target = target.ToLowerInvariant().Trim();

			var sourceLength = source.Length;
			var targetLength = target.Length;

			var distance = new int[sourceLength + 1, targetLength + 1];

			for (var i = 0; i <= sourceLength; distance[i, 0] = i++) { }
			for (var j = 0; j <= targetLength; distance[0, j] = j++) { }

			for (var i = 1; i <= sourceLength; i++)
			{
				for (var j = 1; j <= targetLength; j++)
				{
					var cost = target[j - 1] == source[i - 1] ? 0 : 1;
					distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
				}
			}

			double stepsToSame = distance[sourceLength, targetLength];

			return 1.0 - stepsToSame / Math.Max(source.Length, target.Length);
		}

		/// <summary>
		/// Compress string
		/// </summary>
		/// <param name="data">input string</param>
		/// <exception cref="ArgumentNullException">Thrown if data is null or empty</exception>
		/// <returns>compressed string as byte array</returns>
		public static byte[] Compress(this string data)
		{
			if (data.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(data));
			}

			using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				using (var outputStream = new MemoryStream())
				{
					using (var deflate = new DeflateStream(outputStream, CompressionMode.Compress))
					{
						inputStream.CopyTo(deflate);
					}

					return outputStream.ToArray();
				}
			}
		}

		/// <summary>
		/// Convert the passed string to a boolean equivalent
		/// </summary>
		/// <param name="value">string</param>
		/// <returns>boolean equivalent</returns>
		public static bool ToBoolean(this string value)
		{
			if (value.IsNullOrEmpty())
			{
				return false;
			}

			value = value.ToLower().Trim();

			if (value.Equals("true") || value.Equals("1"))
			{
				return true;
			}

			return !Boolean.TryParse(value, out var temp) && temp;
		}

		public static string ReturnNullByEmpty(this string value)
		{
			return value.IsNullOrEmpty() ? null : value;
		}

		public static T GetEnumType<T>(this string typeValue, T defaultValue) where T : Enum
		{
			foreach (var enumMember in (T[])Enum.GetValues(typeof(T)))
			{
				if (enumMember.ToString() == typeValue || (Int32.TryParse(typeValue, out var typeValueAsInt) && typeValueAsInt == Convert.ToInt32(enumMember)))
				{
					return enumMember;
				}
			}

			return defaultValue;
		}
	}
}