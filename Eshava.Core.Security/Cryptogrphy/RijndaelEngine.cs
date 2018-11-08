using System.IO;
using System.Security.Cryptography;
using System.Text;
using Eshava.Core.Security.Cryptogrphy.Interfaces;
using Eshava.Core.Security.Cryptogrphy.Models;

namespace Eshava.Core.Security.Cryptogrphy
{
	public class RijndaelEngine : ISymmetricCryptographyEngine
	{
		private readonly SymmetricCryptographySettings _settings;
		
		public RijndaelEngine(SymmetricCryptographySettings settings)
		{
			_settings = settings;
		}

		public string Type => "Rijndael";

		public (byte[] IV, byte[] Key) GenerateKeys(bool overwriteInstanceKeys)
		{
			using (var rijndaelManaged = new RijndaelManaged())
			{
				Configurate(rijndaelManaged);

				rijndaelManaged.GenerateIV();
				rijndaelManaged.GenerateKey();

				if (overwriteInstanceKeys)
				{
					_settings.IV = rijndaelManaged.IV;
					_settings.Key = rijndaelManaged.Key;
				}

				return (rijndaelManaged.IV, rijndaelManaged.Key);
			}
		}

		public byte[] Encrypt(string input)
		{
			byte[] encryptedData;

			using (var rijndaelManaged = new RijndaelManaged())
			{
				Configurate(rijndaelManaged);

				using (var inputStream = new MemoryStream(Encoding.Unicode.GetBytes(input)))
				using (var outputStream = new MemoryStream())
				{
					using (var cryptStream = new CryptoStream(outputStream, rijndaelManaged.CreateEncryptor(_settings.Key, _settings.IV), CryptoStreamMode.Write))
					{
						var buffer = new byte[1024];
						var read = inputStream.Read(buffer, 0, buffer.Length);
						while (read > 0)
						{
							cryptStream.Write(buffer, 0, read);
							read = inputStream.Read(buffer, 0, buffer.Length);
						}

						cryptStream.FlushFinalBlock();
						encryptedData = outputStream.ToArray();
					}
				}
			}

			return encryptedData;
		}

		public string Decrypt(byte[] encryptedData)
		{
			string output;

			using (var rijndaelManaged = new RijndaelManaged())
			{
				Configurate(rijndaelManaged);

				using (var inputStream = new MemoryStream(encryptedData))
				using (var outputStream = new MemoryStream())
				{
					using (var cryptStream = new CryptoStream(inputStream, rijndaelManaged.CreateDecryptor(_settings.Key, _settings.IV), CryptoStreamMode.Read))
					{
						var buffer = new byte[1024];
						var read = cryptStream.Read(buffer, 0, buffer.Length);
						while (read > 0)
						{
							outputStream.Write(buffer, 0, read);
							read = cryptStream.Read(buffer, 0, buffer.Length);
						}
						cryptStream.Flush();
						output = Encoding.Unicode.GetString(outputStream.ToArray());
					}
				}
			}

			return output;
		}

		private void Configurate(RijndaelManaged rijndaelManaged)
		{
			rijndaelManaged.Padding = _settings.PaddingMode;
			rijndaelManaged.KeySize = _settings.KeySize;
			rijndaelManaged.Mode = _settings.CipherMode;
		}
	}
}