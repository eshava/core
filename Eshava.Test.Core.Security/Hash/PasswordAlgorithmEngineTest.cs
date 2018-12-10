using Eshava.Core.Security.Hash;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Security.Hash
{
	[TestClass, TestCategory("Core.Security")]
	public class PasswordAlgorithmEngineTest
	{
		private PasswordAlgorithmEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new PasswordAlgorithmEngine();
		}

		[TestMethod]
		public void HashAndVerifySuccessTest()
		{
			// Arrange
			var password = "Darkwing Duck";

			// Act
			var resultHash = _classUnderTest.Hash(password);
			var resultVerify = _classUnderTest.Verify(password, resultHash.HashBase64, resultHash.SaltBase64);

			// Assert
			resultVerify.Should().BeTrue();
			resultHash.HashBase64.Should().NotBeNull();
			resultHash.SaltBase64.Should().NotBeNull();
			resultHash.HashBase64.Length.Should().Be(344);
			resultHash.SaltBase64.Length.Should().Be(344);
		}

		[TestMethod]
		public void HashAndVerifyWringVerifyPasswordTest()
		{
			// Arrange
			var password = "Darkwing Duck";

			// Act
			var resultHash = _classUnderTest.Hash(password);
			var resultVerify = _classUnderTest.Verify("QuackFu", resultHash.HashBase64, resultHash.SaltBase64);

			// Assert
			resultVerify.Should().BeFalse();
		}

		[TestMethod]
		public void HashAndVerifyWrongVerifySaltTest()
		{
			// Arrange
			var password = "Darkwing Duck";
			var wrongSalt = "Llcs6hqS+Y9+qr7u292/NnXYnaT5A8zLaInNX2oHVHyEqyvvi1i7vqsorr7WnFoDBe5BMq2GP6N2G0x+6KqgJ2d81HPxdiHQcIAhk/l7udDrja8lWK9aYux0G/UjWzc/DNI4MkeGnHdLl+XBqJ4VOiZAABsuxPrB9DJJA59luSWpYjXJcT18E1lBrhp78Go3/FUwVloYweJVMbj4Hrfk9qGqVU1DpsviE4965/mAaGsvz8gvtICgHes0TRJKwpg1fzT+1tiA+susfhO+nHmMRFpkrqQ6N/KGimwA+p8E1a+Q4KSzOcE/dnsNFLViLtscZhc/wZnvrcaaD1uDg9Shtg==";

			// Act
			var resultHash = _classUnderTest.Hash(password);
			var resultVerify = _classUnderTest.Verify(password, resultHash.HashBase64, wrongSalt);

			// Assert
			resultVerify.Should().BeFalse();
		}

		[TestMethod]
		public void HashAndVerifyWrongVerifyPasswordHashTest()
		{
			// Arrange
			var password = "Darkwing Duck";
			var wrongSalt = "Llcs6hqS+Y9+qr7u292/NnXYnaT5A8zLaInNX2oHVHyEqyvvi1i7vqsorr7WnFoDBe5BMq2GP6N2G0x+6KqgJ2d81HPxdiHQcIAhk/l7udDrja8lWK9aYux0G/UjWzc/DNI4MkeGnHdLl+XBqJ4VOiZAABsuxPrB9DJJA59luSWpYjXJcT18E1lBrhp78Go3/FUwVloYweJVMbj4Hrfk9qGqVU1DpsviE4965/mAaGsvz8gvtICgHes0TRJKwpg1fzT+1tiA+susfhO+nHmMRFpkrqQ6N/KGimwA+p8E1a+Q4KSzOcE/dnsNFLViLtscZhc/wZnvrcaaD1uDg9Shtg==";

			// Act
			var resultHash = _classUnderTest.Hash(password);
			var resultVerify = _classUnderTest.Verify(password, wrongSalt, resultHash.SaltBase64);

			// Assert
			resultVerify.Should().BeFalse();
		}
	}
}