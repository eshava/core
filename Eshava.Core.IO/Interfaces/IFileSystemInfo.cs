using System;

namespace Eshava.Core.IO.Interfaces
{
	public interface IFileSystemInfo
	{
		DateTime LastAccessTimeUtc { get; }
		DateTime CreationTimeUtc { get; }
		DateTime LastWriteTimeUtc { get; }
		DateTime LastAccessTime { get; }
		DateTime CreationTime { get; }
		DateTime LastWriteTime { get; }
		string Name { get; }
		string FullName { get; }
		bool Exists { get; }
		bool IsDirectory { get; }
		bool IsFile { get; }
	}
}