using System;
using System.Security.Cryptography;

namespace Eshava.Core.Security.Hash
{
	public sealed class PasswordAlgorithmEngine
	{
		private const int SALT_SIZE = 256;
		private const int HASH_SIZE = 256;
		private const int HASH_ITERATIONS = 1000;
		
		public (string HashBase64, string SaltBase64) Hash(string password)
		{
			var salt = new byte[SALT_SIZE];
			new RNGCryptoServiceProvider().GetBytes(salt);
			var hash = new Rfc2898DeriveBytes(password, salt, HASH_ITERATIONS).GetBytes(HASH_SIZE);

			return (Convert.ToBase64String((byte[])hash.Clone()), Convert.ToBase64String((byte[])salt.Clone()));
		}

		public bool Verify(string password, string hashBase64, string saltBase64)
		{
			var salt = new byte[SALT_SIZE];
			var hash = new byte[SALT_SIZE];
			
			Array.Copy(Convert.FromBase64String(saltBase64), 0, salt, 0, SALT_SIZE);
			Array.Copy(Convert.FromBase64String(hashBase64), 0, hash, 0, HASH_SIZE);

			var test = new Rfc2898DeriveBytes(password, salt, HASH_ITERATIONS).GetBytes(HASH_SIZE);
			for (var index = 0; index < HASH_SIZE; index++)
			{
				if (test[index] != hash[index])
				{
					return false;
				}
			}

			return true;
		}
	}
}