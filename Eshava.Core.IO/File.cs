using System;
using System.IO;
using System.Text;
using Eshava.Core.IO.Interfaces;

namespace Eshava.Core.IO
{
	public class File : IFile
	{
		private readonly FileSystemEngine _fileSystemEngine;
		private readonly FileInfo _fileInfo;
		private IDirectory _directory;

		public File(string fullFileName, FileSystemEngine fileSystemEngine)
		{
			_fileInfo = new FileInfo(fullFileName);
			_fileSystemEngine = fileSystemEngine;
		}

		public File(FileInfo fileInfo, FileSystemEngine fileSystemEngine)
		{
			_fileInfo = fileInfo;
			_fileSystemEngine = fileSystemEngine;
		}

		public string Extension => _fileInfo.Extension;

		public DateTime LastAccessTimeUtc => _fileInfo.LastWriteTimeUtc;

		public DateTime LastWriteTimeUtc => _fileInfo.LastWriteTimeUtc;

		public DateTime CreationTimeUtc => _fileInfo.CreationTimeUtc;

		public DateTime LastAccessTime => _fileInfo.LastWriteTime;

		public DateTime CreationTime => _fileInfo.CreationTime;

		public DateTime LastWriteTime => _fileInfo.LastWriteTime;

		public string Name => _fileInfo.Name;

		public string FullName => _fileInfo.FullName;

		public bool Exists => _fileInfo.Exists;

		public bool IsLocked
		{
			get
			{
				var result = false;
				try
				{
					using (var stream = System.IO.File.Open(FullName, FileMode.Open, FileAccess.Read, FileShare.None))
					{
						stream.Close();
					}
				}
				catch
				{
					result = true;
				}

				return result;
			}
		}

		public IDirectory Directory
		{
			get
			{
				if (_directory == null)
				{
					_directory = _fileSystemEngine.GetDirectory(_fileInfo.Directory.FullName);
				}

				return _directory;
			}
		}

		public long Length => _fileInfo.Length;

		public bool IsDirectory => false;

		public bool IsFile => true;

		public IFile CopyTo(string fullName, bool overwrite) => new File(_fileInfo.CopyTo(fullName, overwrite), _fileSystemEngine);

		public IFile Create()
		{
			using (var file = System.IO.File.Create(FullName))
			{
				file.Close();
			}

			return this;
		}

		public IFile Delete()
		{
			_fileInfo.Delete();

			return this;
		}

		public Stream GetStream(FileMode fileMode, FileAccess fileAccess) => new FileStream(FullName, fileMode, fileAccess);

		public Stream GetStream(FileMode fileMode, FileAccess fileAccess, FileShare fileShare) => new FileStream(FullName, fileMode, fileAccess, fileShare);

		public IFile MoveTo(IFile targetFile)
		{
			_fileInfo.MoveTo(targetFile.FullName);

			return targetFile;
		}

		public byte[] ReadAllBytes() => System.IO.File.ReadAllBytes(_fileInfo.FullName);

		public string ReadAllText() => System.IO.File.ReadAllText(_fileInfo.FullName);

		public IFile WriteAllBytes(byte[] blob)
		{
			System.IO.File.WriteAllBytes(_fileInfo.FullName, blob);

			return this;
		}

		public IFile WriteAllText(string text)
		{
			System.IO.File.WriteAllText(_fileInfo.FullName, text);

			return this;
		}

		public IFile WriteAllText(string text, Encoding encoding)
		{
			System.IO.File.WriteAllText(_fileInfo.FullName, text, encoding);

			return this;
		}
	}
}