using System;
using System.IO;
using System.IO.Compression;

namespace Eshava.Core.Extensions
{
	public static class ByteArrayExtension
	{
		public static string ToBase64(this byte[] value) => value == null || value.Length == 0 ? null : Convert.ToBase64String(value);

		/// <summary>
		/// Decompress byte array
		/// </summary>
		/// <param name="data">input byte array</param>
		/// <exception cref="ArgumentNullException">Thrown if data is null or empty</exception>
		/// <returns>compressed string as byte array</returns>
		public static string DecompressString(this byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				throw new ArgumentNullException(nameof(data));
			}

			using (var inputStream = new MemoryStream(data))
			{
				using (var outputStream = new MemoryStream())
				{
					using (var deflate = new DeflateStream(inputStream, CompressionMode.Decompress))
					{
						deflate.CopyTo(outputStream);
					}

					outputStream.Position = 0;
					var reader = new StreamReader(outputStream);

					return reader.ReadToEnd();
				}
			}
		}
	}
}