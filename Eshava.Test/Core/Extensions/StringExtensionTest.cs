using System;
using System.Text;
using Eshava.Core.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Extensions
{
	[TestClass, TestCategory("Core.Extensions")]
	public class StringExtensionTest
	{
		[TestInitialize]
		public void Setup()
		{

		}

		[TestMethod]
		public void GetCompareResultByLevenshteinAlgorithmBothStringEmptyTest()
		{
			// Arrange
			var source = "";
			var target = "";

			// Act
			var result = source.GetCompareResultByLevenshteinAlgorithm(target);

			// Assert
			result.Should().Be(1);
		}

		[TestMethod]
		public void GetCompareResultByLevenshteinAlgorithmSourceStringEmptyTest()
		{
			// Arrange
			var source = "DarkwingDuck";
			var target = "";

			// Act
			var result = source.GetCompareResultByLevenshteinAlgorithm(target);

			// Assert
			result.Should().Be(0);
		}

		[TestMethod]
		public void GetCompareResultByLevenshteinAlgorithmTargetStringEmptyTest()
		{
			// Arrange
			var source = "";
			var target = "DarkwingDuck";

			// Act
			var result = target.GetCompareResultByLevenshteinAlgorithm(source);

			// Assert
			result.Should().Be(0);
		}

		[TestMethod]
		public void GetCompareResultByLevenshteinAlgorithmSourceTargetAreDiffrentTest()
		{
			// Arrange
			var source = "DurkwingDack";
			var target = "DarkwingDuck";

			// Act
			var result = target.GetCompareResultByLevenshteinAlgorithm(source);

			// Assert
			Math.Round(result, 4).Should().Be(0.8333);
		}

		[TestMethod]
		public void GetCompareResultByLevenshteinAlgorithmSourceTargetAreEqualTest()
		{
			// Arrange
			var source = "DarkwingDuck";
			var target = "DarkwingDuck";

			// Act
			var result = target.GetCompareResultByLevenshteinAlgorithm(source);

			// Assert
			result.Should().Be(1);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CompressExceptionStringEmptyTest()
		{
			// Arrange
			var uncompressed = "";

			// Act
			uncompressed.Compress();

			// Assert
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CompressExceptionStringNullTest()
		{
			// Arrange
			string uncompressed = null;

			// Act
			uncompressed.Compress();

			// Assert
		}

		[TestMethod]
		public void CompressAndDecompressTest()
		{
			// Arrange
			var uncompressed = "Knockout is a JavaScript library that helps you to create rich, responsive display and editor user interfaces with a clean underlying data model. Any time you have sections of UI that update dynamically (e.g., changing depending on the user’s actions or when an external data source changes), KO can help you implement it more simply and maintainably.";

			// Act
			var resultCompressed = uncompressed.Compress();
			var resultDecompressed = resultCompressed.DecompressString();

			// Assert
			resultCompressed.Length.Should().Be(236);
			resultDecompressed.Should().Be(uncompressed);
		}

		[TestMethod]
		public void ReturnNullByEmptyTest()
		{
			// Arrange
			var source = "";

			// Act
			var result = source.ReturnNullByEmpty();

			// Assert
			result.Should().BeNull();
		}

		[TestMethod]
		public void ReturnNullByEmptyWithNullInputTest()
		{
			// Arrange
			string source = null;

			// Act
			var result = source.ReturnNullByEmpty();

			// Assert
			result.Should().BeNull();
		}

		[TestMethod]
		public void ReturnNullByEmptyWithInputTest()
		{
			// Arrange
			var source = "DarkwingDuck";

			// Act
			var result = source.ReturnNullByEmpty();

			// Assert
			result.Should().Be(source);
		}

		[TestMethod]
		public void ToBooleanTest()
		{
			// Act && Assert
			((string)null).ToBoolean().Should().BeFalse();
			"".ToBoolean().Should().BeFalse();
			"DarkwingDuck".ToBoolean().Should().BeFalse();
			"wahr".ToBoolean().Should().BeFalse();
			"falsch".ToBoolean().Should().BeFalse();
			"True".ToBoolean().Should().BeTrue();
			"False".ToBoolean().Should().BeFalse();
			"true".ToBoolean().Should().BeTrue();
			"false".ToBoolean().Should().BeFalse();
			"1".ToBoolean().Should().BeTrue();
			"0".ToBoolean().Should().BeFalse();
			"-1".ToBoolean().Should().BeFalse();
			"56".ToBoolean().Should().BeFalse();
		}

		[TestMethod]
		public void IsNullOrEmptyTest()
		{
			// Act && Assert
			((string)null).IsNullOrEmpty().Should().BeTrue();
			"".IsNullOrEmpty().Should().BeTrue();
			"DarkwingDuck".IsNullOrEmpty().Should().BeFalse();
		}

		[TestMethod]
		public void SetValidDirectoryPathEndTest()
		{
			// Act && Assert
			"".SetValidDirectoryPathEnd().Should().Be("\\");
			@"C:\DarkwingDuck".SetValidDirectoryPathEnd().Should().Be(@"C:\DarkwingDuck\");
			@"C:\DarkwingDuck\".SetValidDirectoryPathEnd().Should().Be(@"C:\DarkwingDuck\");
		}

		[TestMethod, ExpectedException(typeof(NullReferenceException))]
		public void SetValidDirectoryPathEndWithNullStringTest()
		{
			// Act && Assert
			((string)null).SetValidDirectoryPathEnd();
		}

		[TestMethod]
		public void FromBase64Test()
		{
			// Arrange
			var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes("DarkwingDuck"));

			// Act 
			var result = base64String.FromBase64();

			// Assert
			Encoding.UTF8.GetString(result).Should().Be("DarkwingDuck");
		}

		[TestMethod]
		public void FromBase64WithEmptyStringTest()
		{
			// Arrange
			var base64String = "";

			// Act 
			var result = base64String.FromBase64();

			// Assert
			result.Length.Should().Be(0);
		}

		[TestMethod]
		public void FromBase64WithNullStringTest()
		{
			// Arrange
			string base64String = null;

			// Act 
			var result = base64String.FromBase64();

			// Assert
			result.Length.Should().Be(0);
		}
	}
}