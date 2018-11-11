namespace Eshava.Core.Security.Hash.Interfaces
{
	public interface IPasswordAlgorithmEngine
	{
		bool Verify(string password, string hashBase64, string saltBase64);
		(string HashBase64, string SaltBase64) Hash(string password);
	}
}