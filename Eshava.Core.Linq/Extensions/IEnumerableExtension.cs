using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eshava.Core.Linq.Extensions
{
	public static class IEnumerableExtension
	{
		public static async Task<IEnumerable<T>> WhereAsync<T>(this IEnumerable<T> items, Func<T, Task<bool>> expression)
		{
			var results = new List<T>();

			foreach (var item in items)
			{
				if (await expression(item))
				{
					results.Add(item);
				}
			}

			return results;
		}

		public static async Task<bool> AnyAsync<T>(this IEnumerable<T> items, Func<T, Task<bool>> expression)
		{
			foreach (var item in items)
			{
				if (await expression(item))
				{
					return true;
				}
			}

			return false;
		}
	}
}