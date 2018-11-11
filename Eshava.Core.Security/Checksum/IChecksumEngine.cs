namespace Eshava.Core.Security.Checksum
{
	public interface IChecksumEngine
	{
		string Type { get; }
		ulong GetChecksum(string input);
		bool VerifyChecksum(string input, ulong checksum);
	}
}