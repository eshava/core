/*
 * Source: https://gist.github.com/regularcoder/8254723
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Eshava.Core.Security.Checksum
{
	/// <summary>
	/// Calculates Fletcher's checksums
	/// Sample outputs: 
	/// Fletcher16: "abcde" -> 51440
	/// Fletcher32: "abcde" -> 3948201259
	/// Fletcher64: "abcde" -> 14034561336514601929
	/// </summary>
	public class FletcherChecksumEngine : IChecksumEngine
	{
		private readonly FletcherChecksumMode _mode;

		public FletcherChecksumEngine(FletcherChecksumMode mode)
		{
			_mode = mode;
		}

		public string Type => "Fletcher";

		/// <summary>
		/// Get Fletcher's checksum
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public ulong GetChecksum(string input)
		{
			//Fletcher 16: Read a single byte
			//Fletcher 32: Read a 16 bit block (two bytes)
			//Fletcher 64: Read a 32 bit block (four bytes)
			var bytesPerCycle = ((int)_mode) / 16;

			//2^x gives max value that can be stored in x bits
			//no of bits here is 8 * bytesPerCycle (8 bits to a byte)
			var modValue = (ulong)(Math.Pow(2, 8 * bytesPerCycle) - 1);

			//ASCII encoding conveniently gives us 1 byte per character 
			var inputAsBytes = Encoding.ASCII.GetBytes(input);

			ulong sum1 = 0;
			ulong sum2 = 0;
			foreach (var block in Blockify(inputAsBytes, bytesPerCycle))
			{
				sum1 = (sum1 + block) % modValue;
				sum2 = (sum2 + sum1) % modValue;
			}

			return sum1 + (sum2 * (modValue + 1));
		}

		/// <summary>
		/// Calculate Fletcher's checksum and compare with passed checksum
		/// </summary>
		/// <param name="input"></param>
		/// <param name="checksum"></param>
		/// <returns></returns>
		public bool VerifyChecksum(string input, ulong checksum)
		{
			return GetChecksum(input) == checksum;
		}

		/// <summary>
		/// Transforms byte array into an enumeration of blocks of 'blockSize' bytes
		/// </summary>
		/// <param name="inputAsBytes"></param>
		/// <param name="blockSize"></param>
		/// <returns></returns>
		private static IEnumerable<ulong> Blockify(IReadOnlyList<byte> inputAsBytes, int blockSize)
		{
			var inputIndex = 0;

			//UInt64 used since that is the biggest possible value we can return.
			//Using an unsigned type is important - otherwise an arithmetic overflow will result
			ulong block = 0;

			//Run through all the bytes			
			while (inputIndex < inputAsBytes.Count)
			{
				//Keep stacking them side by side by shifting left and OR-ing				
				block = block << 8 | inputAsBytes[inputIndex];

				inputIndex++;

				//Return a block whenever we meet a boundary
				if (inputIndex % blockSize == 0 || inputIndex == inputAsBytes.Count)
				{
					yield return block;

					//Set to 0 for next iteration
					block = 0;
				}
			}
		}
	}
}