using System;
using System.IO;
using Eshava.Core.IO.Interfaces;

namespace Eshava.Core.IO
{
	public class FileSystemEngine : IFileSystemEngine
	{
		public IDirectory GetDirectory(string path) => new Directory(path, this);

		public IDirectory GetDirectory(DirectoryInfo directoryInfo) => new Directory(directoryInfo, this);

		public (bool Successful, Exception Exception) HasWritePermisson(string path, string fileNamePart = null)
		{
			var lockFileName = Path.Combine(path, Environment.MachineName + Guid.NewGuid() + (fileNamePart ?? "") + ".permissionTest");

			try
			{
				using (System.IO.File.Create(lockFileName, 1024, FileOptions.DeleteOnClose))
				{

				}
			}
			catch (Exception e)
			{
				return (false, e);
			}

			return (true, null);
		}

		public IFile GetFile(string path) => new File(path, this);

		public IFile GetFile(FileInfo info) => new File(info, this);

		public IFileSystemEngine SetCurrentDirectory(string baseDirectory)
		{
			System.IO.Directory.SetCurrentDirectory(baseDirectory);

			return this;
		}

		public IFileSystemEngine Delete(string fullName)
		{
			var fileInfo = new FileInfo(fullName);
			var directoryInfo = new DirectoryInfo(fullName);

			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}
			else if (directoryInfo.Exists)
			{
				directoryInfo.Delete(true);
			}

			return this;
		}
	}
}