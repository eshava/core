using System;
using System.Collections.Generic;
using System.Text;
using Eshava.Core.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Linq
{
	[TestClass, TestCategory("Core.Linq")]
	public class SortingQueryEngineTest
	{
		private SortingQueryEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new SortingQueryEngine();
		}

		//[TestMethod]
		//public void ValidateWithNullInputTest()
		//{
		//	// Act
		//	var result = _classUnderTest.Validate(null);

		//	// Assert
		//	result.IsValid.Should().BeFalse();
		//	result.ValidationError.Should().Be("model should not be null.");
		//}
	}
}