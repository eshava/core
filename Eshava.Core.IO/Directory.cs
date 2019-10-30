using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eshava.Core.IO.Interfaces;

namespace Eshava.Core.IO
{
	public class Directory : IDirectory
	{
		private readonly FileSystemEngine _fileSystemEngine;
		private readonly DirectoryInfo _directoryInfo;

		public Directory(string path, FileSystemEngine fileSystemEngine)
		{
			_directoryInfo = new DirectoryInfo(path);
			_fileSystemEngine = fileSystemEngine;
		}

		public Directory(DirectoryInfo directoryInfo, FileSystemEngine fileSystemEngine)
		{
			_directoryInfo = directoryInfo;
			_fileSystemEngine = fileSystemEngine;
		}

		public DateTime LastAccessTimeUtc => _directoryInfo.LastWriteTimeUtc;

		public DateTime CreationTimeUtc => _directoryInfo.CreationTimeUtc;

		public DateTime LastWriteTimeUtc => _directoryInfo.LastWriteTimeUtc;

		public DateTime LastAccessTime => _directoryInfo.LastWriteTime;

		public DateTime CreationTime => _directoryInfo.CreationTime;

		public DateTime LastWriteTime => _directoryInfo.LastWriteTime;

		public string Name => _directoryInfo.Name;

		public string FullName => _directoryInfo.FullName;

		public bool Exists => _directoryInfo.Exists;

		public bool IsDirectory => true;

		public bool IsFile => false;

		public IDirectory Parent => new Directory(_directoryInfo.Parent, _fileSystemEngine);

		public IDirectory Create()
		{
			_directoryInfo.Create();

			return this;
		}

		public IDirectory Delete(bool recursive)
		{
			_directoryInfo.Delete(recursive);

			return this;
		}

		public IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption searchOption)
		{
			return _directoryInfo.EnumerateDirectories(searchPattern, searchOption)
								 .Select(directoryInfo => _fileSystemEngine.GetDirectory(directoryInfo))
								 .ToList();
		}

		public IEnumerable<IFile> EnumerateFiles(string searchPattern)
		{
			return _directoryInfo.EnumerateFiles(searchPattern)
								 .Select(fileInfo => _fileSystemEngine.GetFile(fileInfo))
								 .ToList();
		}

		public IEnumerable<IFile> EnumerateFiles(string searchPattern, SearchOption searchOption)
		{
			return _directoryInfo.EnumerateFiles(searchPattern, searchOption)
								 .Select(fileInfo => _fileSystemEngine.GetFile(fileInfo))
								 .ToList();
		}

		public IDirectory MoveTo(IDirectory targetDirectory)
		{
			_directoryInfo.MoveTo(targetDirectory.FullName);

			return targetDirectory;
		}
	}
}