using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Eshava.Core.Models;

namespace Eshava.Core
{
	public static class StringFactory
	{
		private static readonly (int Min, int Max) _rangeNumbers = (48, 57);
		private static readonly (int Min, int Max) _rangeUppercase = (65, 90);
		private static readonly (int Min, int Max) _rangeLowercase = (97, 122);
		private static readonly (int Min, int Max) _rangeSpecialCharacters1 = (33, 47);
		private static readonly (int Min, int Max) _rangeSpecialCharacters2 = (58, 64);
		private static readonly (int Min, int Max) _rangeSpecialCharacters3 = (91, 96);
		private static readonly (int Min, int Max) _rangeSpecialCharacters4 = (123, 126);

		/// <summary>
		/// Generate a random string of specified length
		/// </summary>
		/// <param name="numberOfSigns">Length of the string</param>
		/// <returns>random string</returns>
		public static string GenerateRandomString(int numberOfSigns)
		{
			return GenerateRandomString(numberOfSigns, new List<(int Min, int Max)> { _rangeNumbers, _rangeUppercase, _rangeLowercase });
		}

		/// <summary>
		/// Generate a random string of specified length
		/// </summary>
		/// <param name="numberOfSigns">Length of the string</param>
		/// <param name="options"></param>
		/// <returns>random string</returns>
		public static string GenerateRandomString(int numberOfSigns, RandomStringOptions options)
		{
			if (options.NoCharacterTypeEnabled)
			{
				throw new NotSupportedException("At least one character type must be enabled");
			}

			var ranges = new List<(int Min, int Max)>();

			if (options.IncludeNumbers)
			{
				ranges.Add(_rangeNumbers);
			}

			if (options.IncludeUppercase)
			{
				ranges.Add(_rangeUppercase);
			}

			if (options.IncludeLowercase)
			{
				ranges.Add(_rangeLowercase);
			}

			if (options.IncludeSpecialCharacter)
			{
				ranges.Add(_rangeSpecialCharacters1);
				ranges.Add(_rangeSpecialCharacters2);
				ranges.Add(_rangeSpecialCharacters3);
				ranges.Add(_rangeSpecialCharacters4);
			}

			return GenerateRandomString(numberOfSigns, ranges);
		}

		private static string GenerateRandomString(int numberOfSigns, List<(int Min, int Max)> ranges)
		{
			if (numberOfSigns <= 0)
			{
				throw new NotSupportedException(nameof(numberOfSigns) + ": A number greater than zero is expected.");
			}

			var text = new StringBuilder();
			var random = new Random();

			var min = ranges.Min(t => t.Min);
			var max = ranges.Max(t => t.Max);

			for (var index = 0; index < numberOfSigns; index++)
			{
				int sign;

				do
				{
					sign = (random.Next(min, Int32.MaxValue) % (max - min)) + min;
				} while (ranges.All(t => t.Min > sign || t.Max < sign));

				text.Append(Convert.ToChar((byte)sign).ToString(CultureInfo.InvariantCulture));
			}

			return text.ToString();
		}
	}
}