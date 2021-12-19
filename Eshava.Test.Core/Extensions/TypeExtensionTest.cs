using System;
using System.Collections.Generic;
using Eshava.Core.Extensions;
using Eshava.Test.Core.Models;
using Eshava.Test.Core.Models.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Extensions
{
	[TestClass, TestCategory("Core.Extensions")]
	public class TypeExtensionTest
	{
		[TestInitialize]
		public void Setup()
		{

		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetDataTypeWithNullInputTest()
		{
			// Arrange
			Type source = null;
			
			// Act
			source.GetDataType();
		}

		[TestMethod]
		public void GetDataTypeTest()
		{
			// Arrange
			var source = typeof(int);

			// Act
			var result = source.GetDataType();

			// Assert
			result.Should().Be(typeof(int));
		}

		[TestMethod]
		public void GetDataTypeWithNullableTypeTest()
		{
			// Arrange
			var source = typeof(int?);

			// Act
			var result = source.GetDataType();

			// Assert
			result.Should().Be(typeof(int));
		}
		
		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetDataTypeFromIEnumerableWithNullInputTest()
		{
			// Arrange
			Type source = null;

			// Act
			source.GetDataTypeFromIEnumerable();
		}

		[TestMethod]
		public void GetDataTypeFromIEnumerableTest()
		{
			// Arrange
			var source = typeof(IEnumerable<string>);

			// Act
			var result = source.GetDataTypeFromIEnumerable();

			// Assert
			result.Should().Be(typeof(string));
		}

		[TestMethod]
		public void GetDataTypeFromArrayTest()
		{
			// Arrange
			var source = typeof(string[]);

			// Act
			var result = source.GetDataTypeFromIEnumerable();

			// Assert
			result.Should().Be(typeof(string));
		}

		[TestMethod]
		public void GetDataTypeFromIEnumerableNoEnumerableInputTest()
		{
			// Arrange
			var source = typeof(string);

			// Act
			var result = source.GetDataTypeFromIEnumerable();

			// Assert
			result.Should().Be(typeof(string));
		}
		
		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void IsDataTypeNullableWithNullInputTest()
		{
			// Arrange
			Type source = null;

			// Act
			source.IsDataTypeNullable();
		}

		[TestMethod]
		public void IsDataTypeNullableIEnumerableStringTest()
		{
			// Arrange
			var source = typeof(IEnumerable<string>);

			// Act
			var result = source.IsDataTypeNullable();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsDataTypeNullableStringTest()
		{
			// Arrange
			var source = typeof(string);

			// Act
			var result = source.IsDataTypeNullable();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsDataTypeNullableDateTimeTest()
		{
			// Arrange
			var source = typeof(DateTime);

			// Act
			var result = source.IsDataTypeNullable();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsDataTypeNullableIntegerTest()
		{
			// Arrange
			var source = typeof(int);

			// Act
			var result = source.IsDataTypeNullable();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsDataTypeNullableIntegerNullableTest()
		{
			// Arrange
			var source = typeof(int?);

			// Act
			var result = source.IsDataTypeNullable();

			// Assert
			result.Should().BeTrue();
		}

		[TestMethod]
		public void IsDataTypeNullableObjectTest()
		{
			// Arrange
			var source = typeof(object);

			// Act
			var result = source.IsDataTypeNullable();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsDataTypeNullableClassTest()
		{
			// Arrange
			var source = typeof(Alpha);

			// Act
			var result = source.IsDataTypeNullable();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ImplementsIEnumerableWithNullInputTest()
		{
			// Arrange
			Type source = null;

			// Act
			source.ImplementsIEnumerable();
		}

		[TestMethod]
		public void ImplementsIEnumerableTest()
		{
			// Arrange
			var source = typeof(List<string>);

			// Act
			var result = source.ImplementsIEnumerable();

			// Assert
			result.Should().BeTrue();
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ImplementsInterfaceWithNullInputTest()
		{
			// Arrange
			Type source = null;
			var interfaceType = typeof(IAlpha);

			// Act
			source.ImplementsInterface(interfaceType);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ImplementsInterfaceWithNullInterfaceTest()
		{
			// Arrange
			var source = typeof(Alpha);
			
			// Act
			source.ImplementsInterface(null);
		}

		[TestMethod]
		public void ImplementsInterfaceTest()
		{
			// Arrange
			var source = typeof(Alpha);
			var interfaceType = typeof(IAlpha);

			// Act
			var result = source.ImplementsInterface(interfaceType);

			// Assert
			result.Should().BeTrue();
		}

		[TestMethod]
		public void ImplementsInterfaceFalseTest()
		{
			// Arrange
			var source = typeof(Alpha);
			var interfaceType = typeof(IComparable);

			// Act
			var result = source.ImplementsInterface(interfaceType);

			// Assert
			result.Should().BeFalse();
		}
		
		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void CreateInstanceWithNullInputTest()
		{
			// Arrange
			Type source = null;

			// Act
			source.CreateInstance();
		}

		[TestMethod]
		public void CreateInstanceTest()
		{
			// Arrange
			var source = typeof(Alpha);

			// Act
			var result = source.CreateInstance();

			// Assert
			result.Should().NotBeNull();
			result.GetType().Should().Be(source);
		}

		[TestMethod]
		public void CreateInstanceWithParameterTest()
		{
			// Arrange
			var source = typeof(Alpha);

			// Act
			var result = source.CreateInstance(7, "Darkwing Duck");

			// Assert
			result.Should().NotBeNull();
			result.GetType().Should().Be(source);

			var alphaResult = result as Alpha;
			alphaResult.Beta.Should().Be(7);
			alphaResult.Gamma.Should().Be("Darkwing Duck");
		}
	}
}