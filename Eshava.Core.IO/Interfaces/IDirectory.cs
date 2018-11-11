using System.Collections.Generic;
using System.IO;

namespace Eshava.Core.IO.Interfaces
{
	public interface IDirectory : IFileSystemInfo
	{
		IEnumerable<IFile> EnumerateFiles(string searchPattern);
		IEnumerable<IFile> EnumerateFiles(string searchPattern, SearchOption searchOption);
		IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption searchOption);
		IDirectory Create();
		IDirectory Delete(bool recursive);
	}
}