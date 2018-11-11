namespace Eshava.Core.Security.Cryptogrphy.Interfaces
{
	public interface ISymmetricCryptographyEngine
	{
		string Type { get; }
		(byte[] IV, byte[] Key) GenerateKeys(bool overwriteInstanceKeys);
		byte[] Encrypt(string input);
		string Decrypt(byte[] encryptedData);
	}
}