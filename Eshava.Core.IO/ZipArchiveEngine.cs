using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Eshava.Core.Extensions;
using Eshava.Core.IO.Exceptions;
using Eshava.Core.IO.Interfaces;

namespace Eshava.Core.IO
{
	public class ZipArchiveEngine : IArchiveEngine
	{
		private const string FILEEXTENSION = ".zip";

		public string Type => "zip";

		public IArchiveEngine CreateArchive(string sourceDirectoryName, string targetArchiveFullFileName, CompressionLevel compressionLevel = CompressionLevel.Fastest, bool includeBaseDirectory = false)
		{
			if (!targetArchiveFullFileName?.EndsWith(FILEEXTENSION) ?? false)
			{
				throw new NotSupportedException("Only archive of the type '.zip' are supported");
			}

			try
			{
				PrepareTargetPath(targetArchiveFullFileName, ZipArchiveMode.Create);
				ZipFile.CreateFromDirectory(sourceDirectoryName, targetArchiveFullFileName, compressionLevel, includeBaseDirectory);
			}
			catch (Exception ex)
			{
				throw new ArchiveException("Could not create archive: " + targetArchiveFullFileName, FILEEXTENSION, ex);
			}

			return this;
		}

		public string CreateArchive(string archivePath, string archiveNameWithoutExtension, IEnumerable<(string source, string target)> archiveFullFileNames, Encoding entryNameEncoding = null)
		{
			var fullArchiveName = archivePath.IsNullOrEmpty() || archiveNameWithoutExtension.IsNullOrEmpty() ? null : Path.Combine(archivePath, archiveNameWithoutExtension + FILEEXTENSION);
			CreateArchive(fullArchiveName, archiveFullFileNames, entryNameEncoding);

			return fullArchiveName;
		}

		public IArchiveEngine CreateArchive(string fullArchiveName, IEnumerable<(string source, string target)> archiveFullFileNames, Encoding entryNameEncoding = null)
		{
			return CreateOrUpdateArchive(fullArchiveName, archiveFullFileNames, ZipArchiveMode.Create, entryNameEncoding);
		}

		public IArchiveEngine UpdateArchive(string fullArchiveName, IEnumerable<(string source, string target)> archiveFullFileNames, Encoding entryNameEncoding = null)
		{
			return CreateOrUpdateArchive(fullArchiveName, archiveFullFileNames, ZipArchiveMode.Update, entryNameEncoding);
		}

		public IArchiveEngine ExtractArchive(string fullArchiveName, string extractionTargetPath, Encoding entryNameEncoding = null)
		{
			if (!fullArchiveName.EndsWith(FILEEXTENSION))
			{
				throw new NotSupportedException("Only archive of the type '.zip' are supported");
			}

			entryNameEncoding = CheckEncodingOrSetDefault(entryNameEncoding);

			try
			{
				ZipFile.ExtractToDirectory(fullArchiveName, extractionTargetPath, entryNameEncoding);
			}
			catch (Exception ex)
			{
				throw new ArchiveException("Could not extract archive: " + fullArchiveName, FILEEXTENSION, ex);
			}

			return this;
		}

		private Encoding CheckEncodingOrSetDefault(Encoding entryNameEncoding)
		{
			return entryNameEncoding ?? Encoding.UTF8;
		}

		private IArchiveEngine CreateOrUpdateArchive(string fullArchiveName, IEnumerable<(string source, string target)> archiveFullFileNames, ZipArchiveMode zipArchiveMode, Encoding entryNameEncoding)
		{
			if (!fullArchiveName.EndsWith(FILEEXTENSION))
			{
				throw new NotSupportedException("Only archive of the type '.zip' are supported");
			}

			if (archiveFullFileNames == null)
			{
				archiveFullFileNames = new List<(string source, string target)>();
			}

			try
			{
				PrepareTargetPath(fullArchiveName, zipArchiveMode);

				using (var archive = ZipFile.Open(fullArchiveName, zipArchiveMode, CheckEncodingOrSetDefault(entryNameEncoding)))
				{
					foreach (var fullFileName in archiveFullFileNames)
					{
						var fileName = fullFileName.target.IsNullOrEmpty() ? new FileInfo(fullFileName.source).Name : fullFileName.target;
						archive.CreateEntryFromFile(fullFileName.source, fileName);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ArchiveException($"Could not {(zipArchiveMode == ZipArchiveMode.Create ? "create" : "update")} archive: " + fullArchiveName, FILEEXTENSION, ex);
			}

			return this;
		}

		public IEnumerable<string> ReadFullFileNames(string fullArchiveName)
		{
			if (!fullArchiveName.EndsWith(FILEEXTENSION))
			{
				throw new NotSupportedException("Only archive of the type '.zip' are supported");
			}

			try
			{
				using (var archive = ZipFile.OpenRead(fullArchiveName))
				{
					return archive.Entries.Select(entry => entry.FullName.Replace("/", "\\")).ToList();
				}
			}
			catch (Exception ex)
			{
				throw new ArchiveException("Could not read archive: " + fullArchiveName, FILEEXTENSION, ex);
			}
		}

		private static void PrepareTargetPath(string fullArchiveName, ZipArchiveMode zipArchiveMode)
		{
			var archiveFile = new FileInfo(fullArchiveName);
			if (zipArchiveMode == ZipArchiveMode.Create)
			{
				if (archiveFile.Exists)
				{
					System.IO.File.Delete(fullArchiveName);
				}
				else if (archiveFile.Directory != null && !archiveFile.Directory.Exists)
				{
					System.IO.Directory.CreateDirectory(archiveFile.Directory.FullName);
				}
			}
			else if (zipArchiveMode == ZipArchiveMode.Update)
			{
				if (!archiveFile.Exists)
				{
					throw new ArgumentNullException(nameof(fullArchiveName));
				}
			}
		}
	}
}