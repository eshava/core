using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Eshava.Core.Extensions;
using Eshava.Core.IO;
using Eshava.Core.IO.Exceptions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.IO
{
	[TestClass, TestCategory("Core.IO")]
	public class ZipArchiveEngineTest
	{
		private ZipArchiveEngine _classUnderTest;
		private static ConcurrentBag<string> _workspaces;

		[TestInitialize]
		public void Setup()
		{
			_workspaces = new ConcurrentBag<string>();
			_classUnderTest = new ZipArchiveEngine();
		}

		[AssemblyCleanup]
		public static void CleanupAfterAllTests()
		{
			CleanUpTestBed();
		}

		[TestMethod, ExpectedException(typeof(ArchiveException))]
		public void CreateArchiveFromDirectorNoArchivePathTest()
		{
			// Act
			_classUnderTest.CreateArchive(null, null, CompressionLevel.Optimal, false);
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void CreateArchiveFromDirectoryWrongArchiveTypeTest()
		{
			// Act
			_classUnderTest.CreateArchive(null, Path.Combine(Path.GetTempPath(), "Archive.7z"), CompressionLevel.Optimal, false);
		}

		[TestMethod]
		public void CreateArchiveFromDirectoryTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(true, false);
			var archiveFullFileName = Path.Combine(targetPath, "Archive.zip");

			// Act
			_classUnderTest.CreateArchive(sourcePath, archiveFullFileName, CompressionLevel.Optimal, false);

			// Assert
			System.IO.File.Exists(archiveFullFileName).Should().BeTrue();

			var fileList = new List<string>();
			using (var archive = ZipFile.OpenRead(archiveFullFileName))
			{
				fileList.AddRange(archive.Entries.Select(entry => entry.FullName.Replace("/", "\\")));
			}

			fileList.Should().HaveCount(2);
			fileList.First().Should().Be("Darkwing Duck.txt");
			fileList.Last().Should().Be(Path.Combine("QuackFu", "Manifest of QuackFu.txt"));

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}

		[TestMethod]
		public void CreateArchiveFromDirectoryIncludeBaseDirectoryTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(true, false);
			var archiveFullFileName = Path.Combine(targetPath, "Archive.zip");

			// Act
			_classUnderTest.CreateArchive(sourcePath, archiveFullFileName, CompressionLevel.Fastest, true);

			// Assert
			System.IO.File.Exists(archiveFullFileName).Should().BeTrue();

			var fileList = new List<string>();
			using (var archive = ZipFile.OpenRead(archiveFullFileName))
			{
				fileList.AddRange(archive.Entries.Select(entry => entry.FullName.Replace("/", "\\")));
			}

			var sourceDirectory = new DirectoryInfo(sourcePath);
			fileList.Should().HaveCount(2);
			fileList.First().Should().Be(Path.Combine(sourceDirectory.Name, "Darkwing Duck.txt"));
			fileList.Last().Should().Be(Path.Combine(sourceDirectory.Name, "QuackFu", "Manifest of QuackFu.txt"));

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}

		[TestMethod]
		public void CreateArchiveByArchiveNameWithExistingTargetArchiveTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(true, true);
			var archiveFullFileName = Path.Combine(targetPath, "Archive.zip");

			System.IO.Directory.CreateDirectory(targetPath);
			System.IO.File.Move(Path.Combine(sourcePath, "Archive.zip"), Path.Combine(targetPath, "Archive.zip"));

			// Act
			_classUnderTest.CreateArchive(targetPath, "Archive", Array.Empty<(string source, string target)>());

			// Assert
			System.IO.File.Exists(archiveFullFileName).Should().BeTrue();

			using (var archive = ZipFile.OpenRead(archiveFullFileName))
			{
				archive.Entries.Should().HaveCount(0);
			}

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}

		[TestMethod]
		public void CreateArchiveByArchiveNameWithDirectoryStrutureTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(true, false);
			var archiveFullFileName = Path.Combine(targetPath, "Archive.zip");
			var fullFileNames = new List<(string source, string target)>
			{
				(Path.Combine(sourcePath, "Darkwing Duck.txt"), "Darkwing Duck.txt"),
				(Path.Combine(sourcePath, "QuackFu", "Manifest of QuackFu.txt"), Path.Combine("QuackFu", "Manifest of QuackFu.txt"))
			};

			// Act
			_classUnderTest.CreateArchive(targetPath, "Archive", fullFileNames);

			// Assert
			System.IO.File.Exists(archiveFullFileName).Should().BeTrue();

			var fileList = new List<string>();
			using (var archive = ZipFile.OpenRead(archiveFullFileName))
			{
				fileList.AddRange(archive.Entries.Select(entry => entry.FullName.Replace("/", "\\")));
			}

			fileList.Should().HaveCount(2);
			fileList.First().Should().Be("Darkwing Duck.txt");
			fileList.Last().Should().Be(Path.Combine("QuackFu", "Manifest of QuackFu.txt"));

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}

		[TestMethod]
		public void CreateArchiveByArchiveNameWithoutDirectoryStrutureTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(true, false);
			var archiveFullFileName = Path.Combine(targetPath, "Archive.zip");
			var fullFileNames = new List<(string source, string target)>
			{
				(Path.Combine(sourcePath, "Darkwing Duck.txt"), null),
				(Path.Combine(sourcePath, "QuackFu", "Manifest of QuackFu.txt"), null)
			};

			// Act
			_classUnderTest.CreateArchive(targetPath, "Archive", fullFileNames);

			// Assert
			System.IO.File.Exists(archiveFullFileName).Should().BeTrue();

			var fileList = new List<string>();
			using (var archive = ZipFile.OpenRead(archiveFullFileName))
			{
				fileList.AddRange(archive.Entries.Select(entry => entry.FullName.Replace("/", "\\")));
			}

			fileList.Should().HaveCount(2);
			fileList.First().Should().Be("Darkwing Duck.txt");
			fileList.Last().Should().Be("Manifest of QuackFu.txt");

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}
		
		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void CreateArchiveWrongArchiveTypeTest()
		{
			// Act
			_classUnderTest.CreateArchive(Path.Combine(Path.GetTempPath(), "Archive.7z"), Array.Empty<(string source, string target)>());
		}
		
		[TestMethod]
		public void CreateArchiveWithExistingTargetArchiveTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(true, false);
			var archiveFullFileName = Path.Combine(targetPath, "Archive.zip");
			var fullFileNames = new List<(string source, string target)>
			{
				(Path.Combine(sourcePath, "Darkwing Duck.txt"), null),
				(Path.Combine(sourcePath, "QuackFu", "Manifest of QuackFu.txt"), null)
			};

			// Act
			_classUnderTest.CreateArchive(archiveFullFileName, fullFileNames);

			// Assert
			System.IO.File.Exists(archiveFullFileName).Should().BeTrue();

			var fileList = new List<string>();
			using (var archive = ZipFile.OpenRead(archiveFullFileName))
			{
				fileList.AddRange(archive.Entries.Select(entry => entry.FullName.Replace("/", "\\")));
			}

			fileList.Should().HaveCount(2);
			fileList.First().Should().Be("Darkwing Duck.txt");
			fileList.Last().Should().Be("Manifest of QuackFu.txt");

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void UpdateArchiveWrongArchiveTypeTest()
		{
			// Act
			_classUnderTest.UpdateArchive(Path.Combine(Path.GetTempPath(), "Archive.7z"), Array.Empty<(string source, string target)>());
		}

		[TestMethod, ExpectedException(typeof(ArchiveException))]
		public void UpdateArchiveMissingArchiveTest()
		{
			// Act
			_classUnderTest.UpdateArchive(Path.Combine(Path.GetTempPath(), "Archive.zip"), Array.Empty<(string source, string target)>());
		}

		[TestMethod]
		public void UpdateArchiveWithExistingTargetArchiveTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(true, true);
			var archiveFullFileName = Path.Combine(sourcePath, "Archive.zip");
			var fullFileNames = new List<(string source, string target)>
			{
				(Path.Combine(sourcePath, "Darkwing Duck.txt"), "Darkwing Duck Again.txt")
			};

			// Act
			_classUnderTest.UpdateArchive(archiveFullFileName, fullFileNames);

			// Assert
			System.IO.File.Exists(archiveFullFileName).Should().BeTrue();

			var fileList = new List<string>();
			using (var archive = ZipFile.OpenRead(archiveFullFileName))
			{
				fileList.AddRange(archive.Entries.Select(entry => entry.FullName.Replace("/", "\\")));
			}

			fileList.Should().HaveCount(3);
			fileList[0].Should().Be("Darkwing Duck.txt");
			fileList[1].Should().Be(Path.Combine("QuackFu", "Manifest of QuackFu.txt"));
			fileList[2].Should().Be("Darkwing Duck Again.txt");

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void ExtractArchiveWrongArchiveTypeTest()
		{
			// Act
			_classUnderTest.ExtractArchive(Path.Combine(Path.GetTempPath(), "Archive.7z"), null);
		}

		[TestMethod, ExpectedException(typeof(ArchiveException))]
		public void ExtractArchiveMissingArchiveTest()
		{
			// Act
			_classUnderTest.ExtractArchive(Path.Combine(Path.GetTempPath(), "Archive.zip"), null);
		}

		[TestMethod]
		public void ExtractArchiveTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(false, true);
			var archiveFullFileName = Path.Combine(sourcePath, "Archive.zip");

			// Act
			_classUnderTest.ExtractArchive(archiveFullFileName, targetPath);

			// Assert
			System.IO.File.Exists(Path.Combine(targetPath, "Darkwing Duck.txt")).Should().BeTrue();
			System.IO.File.Exists(Path.Combine(targetPath, "QuackFu", "Manifest of QuackFu.txt")).Should().BeTrue();

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}

		private (string SourcePath, string TargetPath) PrepareTestBed(bool copyFiles, bool copyArchive)
		{
			var userTempPath = Path.GetTempPath();

			var sourceDirectory = Path.Combine(userTempPath, Path.GetRandomFileName());
			var targetDirectory = Path.Combine(userTempPath, Path.GetRandomFileName());

			_workspaces.Add(sourceDirectory);
			_workspaces.Add(targetDirectory);

			CleanUpTestBed(sourceDirectory, targetDirectory);

			if (copyFiles || copyArchive)
			{
				System.IO.Directory.CreateDirectory(sourceDirectory);
			}

			if (copyFiles)
			{
				System.IO.Directory.CreateDirectory(Path.Combine(sourceDirectory, "QuackFu"));
				System.IO.File.Copy(Path.Combine(Environment.CurrentDirectory, "Input", "Darkwing Duck.txt"), Path.Combine(sourceDirectory, "Darkwing Duck.txt"));
				System.IO.File.Copy(Path.Combine(Environment.CurrentDirectory, "Input", "QuackFu", "Manifest of QuackFu.txt"), Path.Combine(sourceDirectory, "QuackFu", "Manifest of QuackFu.txt"));
			}

			if (copyArchive)
			{
				System.IO.File.Copy(Path.Combine(Environment.CurrentDirectory, "Input", "Archive.zip"), Path.Combine(sourceDirectory, "Archive.zip"));
			}

			return (sourceDirectory, targetDirectory);
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void ReadFullFileNamesWrongArchiveTypeTest()
		{
			// Act
			_classUnderTest.ReadFullFileNames(Path.Combine(Path.GetTempPath(), "Archive.7z"));
		}

		[TestMethod]
		public void ReadFullFileNamesTest()
		{
			// Arrange
			(var sourcePath, var targetPath) = PrepareTestBed(false, true);
			var archiveFullFileName = Path.Combine(sourcePath, "Archive.zip");

			// Act
			var result = _classUnderTest.ReadFullFileNames(archiveFullFileName);

			// Assert
			result.Should().HaveCount(2);
			result.First().Should().Be("Darkwing Duck.txt");
			result.Last().Should().Be(Path.Combine("QuackFu", "Manifest of QuackFu.txt"));

			// Avoid mess
			CleanUpTestBed(sourcePath, targetPath);
		}

		[TestMethod, ExpectedException(typeof(ArchiveException))]
		public void ReadFullFileNamesMissingArchiveTest()
		{
			// Act
			_classUnderTest.ReadFullFileNames(Path.Combine(Path.GetTempPath(), "Archive.zip"));
		}

		private static void CleanUpTestBed(string sourceDirectoryPath, string targetDirectoryPath)
		{
			if (!sourceDirectoryPath.IsNullOrEmpty() && System.IO.Directory.Exists(sourceDirectoryPath))
			{
				System.IO.Directory.Delete(sourceDirectoryPath, true);
			}

			if (!targetDirectoryPath.IsNullOrEmpty() && System.IO.Directory.Exists(targetDirectoryPath))
			{
				System.IO.Directory.Delete(targetDirectoryPath, true);
			}
		}

		private static void CleanUpTestBed()
		{
			foreach (var workspace in _workspaces.Where(System.IO.Directory.Exists))
			{
				System.IO.Directory.Delete(workspace, true);
			}
		}
	}
}