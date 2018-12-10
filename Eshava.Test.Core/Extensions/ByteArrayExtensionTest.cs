using System;
using Eshava.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Extensions
{
	[TestClass, TestCategory("Core.Extensions")]
	public class ByteArrayExtensionTest
	{
		[TestInitialize]
		public void Setup()
		{

		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void DecompressStringWithEmptyArrayTest()
		{
			// Arrange
			var source = Array.Empty<byte>();

			// Act
			source.DecompressString();
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void DecompressStringWithNullArrayTest()
		{
			// Arrange
			byte[] source = null;

			// Act
			source.DecompressString();
		}
	}
}