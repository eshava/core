namespace Eshava.Core.Security.Hash.Interfaces
{
	public interface IHashAlgorithmEngine
	{
		byte[] Compute(byte[] hashOne, byte[] hashTwo);
		bool Verify(byte[] hashOne, byte[] hashTwo, byte[] hashToVerify);
	}
}