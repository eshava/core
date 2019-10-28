using System.Security.Cryptography;
using System.Text;
using Eshava.Core.Security.Hash;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Security.Hash
{
	[TestClass, TestCategory("Core.Security")]
	public class HashAlgorithmEngineTest
	{
		private HashAlgorithmEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new HashAlgorithmEngine(new SHA256Managed());
		}

		[TestMethod]
		public void ComputeAndVerifyTest()
		{
			// Arrange
			var hashOne = Encoding.UTF8.GetBytes("Darkwing Duck");
			var hashTwo = Encoding.UTF8.GetBytes("MegaVolt");

			// Act
			var resultHash = _classUnderTest.Compute(hashOne, hashTwo);
			var resultVerify = _classUnderTest.Verify(hashOne, hashTwo, resultHash);

			// Assert
			resultVerify.Should().BeTrue();
		}

		[TestMethod]
		public void VerifyHashDifferentLenghtTest()
		{
			// Arrange
			var hashOne = Encoding.UTF8.GetBytes("Darkwing Duck");
			var hashTwo = Encoding.UTF8.GetBytes("MegaVolt");
			var resultHash = Encoding.UTF8.GetBytes("LaunchpadMcQuack");

			// Act
			var resultVerify = _classUnderTest.Verify(hashOne, hashTwo, resultHash);

			// Assert
			resultVerify.Should().BeFalse();
		}

		[TestMethod]
		public void VerifyHashNotEqualTest()
		{
			// Arrange
			var hashOne = Encoding.UTF8.GetBytes("Darkwing Duck");
			var hashTwo = Encoding.UTF8.GetBytes("MegaVolt");

			// Act
			var resultHash = _classUnderTest.Compute(hashOne, hashTwo);
			resultHash[0] = 255;

			var resultVerify = _classUnderTest.Verify(hashOne, hashTwo, resultHash);

			// Assert
			resultVerify.Should().BeFalse();
		}
	}
}