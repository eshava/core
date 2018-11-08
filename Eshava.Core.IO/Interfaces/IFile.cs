using System.IO;

namespace Eshava.Core.IO.Interfaces
{
	public interface IFile : IFileSystemInfo
	{
		string Extension { get; }

		bool IsLocked { get; }

		IFile CopyTo(string fullName, bool overwrite);

		IFile Delete();

		IFile MoveTo(IFile targetFile);

		IDirectory Directory { get; }

		long Length { get; }

		string ReadAllText();

		IFile Create();

		Stream GetStream(FileMode fileMode, FileAccess fileAccess);

		Stream GetStream(FileMode fileMode, FileAccess fileAccess, FileShare fileShare);

		IFile WriteAllBytes(byte[] blob);

		IFile WriteAllText(string text);

		byte[] ReadAllBytes();
	}
}