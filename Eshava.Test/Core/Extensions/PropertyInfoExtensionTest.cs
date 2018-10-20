using System;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Test.Core.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Extensions
{
	[TestClass, TestCategory("Core.Extensions")]
	public class PropertyInfoExtensionTest
	{
		[TestInitialize]
		public void Setup()
		{

		}
		
		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetDataTypeWithNullInputTest()
		{
			// Arrange
			PropertyInfo source = null;

			// Act
			source.GetDataType();
		}

		[TestMethod]
		public void GetDataTypeTest()
		{
			// Arrange
			var source = typeof(Alpha).GetProperty(nameof(Alpha.Beta));

			// Act
			var result = source.GetDataType();

			// Assert
			result.Should().Be(typeof(int));
		}
	}
}