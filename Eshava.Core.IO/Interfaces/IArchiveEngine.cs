using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using Eshava.Core.IO.Exceptions;

namespace Eshava.Core.IO.Interfaces
{
	public interface IArchiveEngine
	{
		string Type { get; }

		/// <summary>
		/// Create an archive at the passed target path
		/// </summary>
		/// <param name="sourceDirectoryName">full file name of the source directory</param>
		/// <param name="targetArchiveFullFileName">full file name of the archive inclusive file extension</param>
		/// <param name="compressionLevel"></param>
		/// <param name="includeBaseDirectory"></param>
		/// <exception cref="NotSupportedException">Unsupported archive type</exception>
		/// <returns><see cref="IArchiveEngine"/></returns>
		IArchiveEngine CreateArchive(string sourceDirectoryName, string targetArchiveFullFileName, CompressionLevel compressionLevel = CompressionLevel.Fastest, bool includeBaseDirectory = false);

		/// <summary>
		/// Create an archive at the passed target path
		/// </summary>
		/// <param name="fullArchiveName">full file name of the archive inclusive file extension</param>
		/// <param name="archiveFullFileNames">Full file names to pack</param>
		/// <param name="entryNameEncoding"></param>
		/// <exception cref="NotSupportedException">Unsupported archive type</exception>
		/// <exception cref="ArchiveException">Exception while creating archive</exception>
		/// <returns><see cref="IArchiveEngine"/></returns>
		IArchiveEngine CreateArchive(string fullArchiveName, IEnumerable<(string source, string target)> archiveFullFileNames, Encoding entryNameEncoding = null);

		/// <summary>
		/// Create an archive at the passed target path
		/// </summary>
		/// <param name="archivePath">Target directory path for the archive</param>
		/// <param name="archiveNameWithoutExtension">name of the archive exclusive file extension</param>
		/// <param name="archiveFullFileNames">Full file names to pack</param>
		/// <param name="entryNameEncoding"></param>
		/// <exception cref="ArchiveException">Exception while creating archive</exception>
		/// <returns>full archive name</returns>
		string CreateArchive(string archivePath, string archiveNameWithoutExtension, IEnumerable<(string source, string target)> archiveFullFileNames, Encoding entryNameEncoding = null);

		/// <summary>
		/// Add files to existing archive
		/// </summary>
		/// <param name="fullArchiveName">full file name of the archive inclusive file extension</param>
		/// <param name="archiveFullFileNames">Full file names to pack</param>
		/// <param name="entryNameEncoding"></param>
		/// <exception cref="NotSupportedException">Unsupported archive type</exception>
		/// <exception cref="ArchiveException">Exception while creating archive</exception>
		/// <returns><see cref="IArchiveEngine"/></returns>
		IArchiveEngine UpdateArchive(string fullArchiveName, IEnumerable<(string source, string target)> archiveFullFileNames, Encoding entryNameEncoding = null);

		/// <summary>
		/// Extract the specified archive to the passed target path
		/// </summary>
		/// <param name="fullArchiveName"></param>
		/// <param name="extractionTargetPath">Target directory path for the extraction</param>
		/// <param name="entryNameEncoding">Encoding for archive entry names</param>
		/// <exception cref="NotSupportedException">Unsupported archive type</exception>
		/// <exception cref="ArchiveException">Exception while extracting archive</exception>
		/// <returns><see cref="IArchiveEngine"/></returns>
		IArchiveEngine ExtractArchive(string fullArchiveName, string extractionTargetPath, Encoding entryNameEncoding = null);

		/// <summary>
		/// Open archive and read all containing full file names
		/// </summary>
		/// <param name="fullArchiveName"></param>
		/// <returns>Containing full file names</returns>
		IEnumerable<string> ReadFullFileNames(string fullArchiveName);
	}
}