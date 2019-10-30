using System.IO;
using System.Text;

namespace Eshava.Core.IO.Interfaces
{
	public interface IFile : IFileSystemInfo
	{
		string Extension { get; }

		bool IsLocked { get; }

		IFile CopyTo(IFile targetFile, bool overwrite);

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
		IFile WriteAllText(string text, Encoding encoding);

		byte[] ReadAllBytes();
	}
}