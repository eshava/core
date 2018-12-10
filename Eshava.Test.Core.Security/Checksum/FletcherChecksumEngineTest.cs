using System;
using System.Collections.Generic;
using System.Text;
using Eshava.Core.Security.Checksum;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Security.Checksum
{
	[TestClass, TestCategory("Core.Security")]
	public class FletcherChecksumEngineTest
	{
		[TestInitialize]
		public void Setup()
		{
			
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetChecksumMode64NullInputTest()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher64);
			
			// Act
			classUnderTest.GetChecksum(null);
		}

		[TestMethod]
		public void GetChecksumMode64EmptyInputTest()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher64);

			// Act
			var result = classUnderTest.GetChecksum("");

			// Assert
			result.Should().Be(0UL);
		}

		[TestMethod]
		public void GetChecksumMode32EmptyInputTest()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher32);

			// Act
			var result = classUnderTest.GetChecksum("");

			// Assert
			result.Should().Be(0UL);
		}

		[TestMethod]
		public void GetChecksumMode16EmptyInputTest()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher16);

			// Act
			var result = classUnderTest.GetChecksum("");

			// Assert
			result.Should().Be(0UL);
		}

		[TestMethod]
		public void GetChecksumMode64Test()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher64);
			var input = "Darkwing Duck knows QuackFu";

			// Act
			var result = classUnderTest.GetChecksum(input);

			// Assert
			result.Should().Be(16913007408455343261UL);
		}

		[TestMethod]
		public void GetChecksumMode32Test()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher32);
			var input = "Darkwing Duck knows QuackFu";

			// Act
			var result = classUnderTest.GetChecksum(input);

			// Assert
			result.Should().Be(2915699200UL);
		}

		[TestMethod]
		public void GetChecksumMode16Test()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher16);
			var input = "Darkwing Duck knows QuackFu";

			// Act
			var result = classUnderTest.GetChecksum(input);

			// Assert
			result.Should().Be(23562UL);
		}

		[TestMethod]
		public void VerifyChecksumMode64Test()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher64);
			var input = "Darkwing Duck knows QuackFu";
			var checksum = 16913007408455343261UL;

			// Act
			var result = classUnderTest.VerifyChecksum(input, checksum);

			// Assert
			result.Should().BeTrue();
		}

		[TestMethod]
		public void VerifyChecksumMode32Test()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher32);
			var input = "Darkwing Duck knows QuackFu";
			var checksum = 2915699200UL;

			// Act
			var result = classUnderTest.VerifyChecksum(input, checksum);

			// Assert
			result.Should().BeTrue();
		}

		[TestMethod]
		public void VerifyChecksumMode16Test()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher16);
			var input = "Darkwing Duck knows QuackFu";
			var checksum = 23562UL;

			// Act
			var result = classUnderTest.VerifyChecksum(input, checksum);

			// Assert
			result.Should().BeTrue();
		}

		[TestMethod]
		public void VerifyChecksumMode64WrongChecksumTest()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher64);
			var input = "Darkwing Duck knows QuackFu";
			var checksum = 11913007408455343261UL;

			// Act
			var result = classUnderTest.VerifyChecksum(input, checksum);

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void VerifyChecksumMode32WrongChecksumTest()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher32);
			var input = "Darkwing Duck knows QuackFu";
			var checksum = 1915699200UL;

			// Act
			var result = classUnderTest.VerifyChecksum(input, checksum);

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void VerifyChecksumMode16WrongChecksumTest()
		{
			// Arrange
			var classUnderTest = new FletcherChecksumEngine(FletcherChecksumMode.Fletcher16);
			var input = "Darkwing Duck knows QuackFu";
			var checksum = 13562UL;

			// Act
			var result = classUnderTest.VerifyChecksum(input, checksum);

			// Assert
			result.Should().BeFalse();
		}
	}
}