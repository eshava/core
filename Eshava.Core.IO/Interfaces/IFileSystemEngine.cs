using System;

namespace Eshava.Core.IO.Interfaces
{
	public interface IFileSystemEngine
	{
		IFile GetFile(string path);
		IDirectory GetDirectory(string path);
		IFileSystemEngine Delete(string fullName);
		IFileSystemEngine SetCurrentDirectory(string baseDirectory);
		(bool Successful, Exception Exception) HasWritePermisson(string path, string fileNamePart = null);
	}
}