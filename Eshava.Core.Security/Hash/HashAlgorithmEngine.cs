using System;
using System.Security.Cryptography;
using Eshava.Core.Security.Hash.Interfaces;

namespace Eshava.Core.Security.Hash
{
	public class HashAlgorithmEngine : IHashAlgorithmEngine
	{
		private readonly HashAlgorithm _hashProvider;

		public HashAlgorithmEngine(HashAlgorithm hashProvider)
		{
			_hashProvider = hashProvider;
		}

		public byte[] Compute(byte[] hashOne, byte[] hashTwo)
		{
			// Allocate memory to store both the Data and Salt together
			var hashCombined = new byte[hashOne.Length + hashTwo.Length];

			// Copy both the data and salt into the new array
			Array.Copy(hashOne, hashCombined, hashOne.Length);
			Array.Copy(hashTwo, 0, hashCombined, hashOne.Length, hashTwo.Length);

			// Calculate the hash
			// Compute hash value of our plain text with appended salt.
			return _hashProvider.ComputeHash(hashCombined);
		}

		public bool Verify(byte[] hashOne, byte[] hashTwo, byte[] hashToVerify)
		{
			var newHash = Compute(hashOne, hashTwo);

			if (newHash.Length != hashToVerify.Length)
			{
				return false;
			}

			for (var index = 0; index < hashToVerify.Length; index++)
			{
				if (!hashToVerify[index].Equals(newHash[index]))
				{
					return false;
				}
			}

			return true;
		}
	}
}