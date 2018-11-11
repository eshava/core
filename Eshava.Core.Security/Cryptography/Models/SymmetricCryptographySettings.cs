using System.Security.Cryptography;

namespace Eshava.Core.Security.Cryptography.Models
{
	public class SymmetricCryptographySettings
	{
		public SymmetricCryptographySettings()
		{
			KeySize = 256;
			PaddingMode = PaddingMode.ISO10126;
			CipherMode = CipherMode.CBC;
		}

		/// <summary>
		/// Max key size: 256
		/// </summary>
		public int KeySize { get; set; }
		public PaddingMode PaddingMode { get; set; }
		public CipherMode CipherMode { get; set; }
		public byte[] IV { get; set; }
		public byte[] Key { get; set; }
	}
}