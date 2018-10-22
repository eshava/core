using System;
using System.Linq;
using System.Linq.Expressions;
using Eshava.Core.Linq;
using Eshava.Core.Linq.Models;
using Eshava.Test.Core.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Linq
{
	[TestClass, TestCategory("Core.Linq")]
	public class WhereQueryEngineTest
	{
		private WhereQueryEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new WhereQueryEngine();
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void BuildQueryExpressionsWithNullInputTest()
		{
			// Act
			_classUnderTest.BuildQueryExpressions<Alpha>(null);
		}

		[TestMethod]
		public void BuildQueryExpressionsGlobalSearchTermTest()
		{
			// Arrange
			var queryParameter = new QueryParameters
			{
				SearchTerm = "Darkwing Duck"
			};

			Expression<Func<Alpha, bool>> expectedResult = p => 
				p.Gamma.Contains("Darkwing Duck") ||
				p.Delta.Contains("Darkwing Duck") ||
				p.DeltaTwo.Contains("Darkwing Duck") ||
				p.DeltaMail.Contains("Darkwing Duck") ||
				p.DeltaUrl.Contains("Darkwing Duck") ||
				p.Epsilon.Contains("Darkwing Duck") ||
				p.EpsilonTwo.Contains("Darkwing Duck") ||
				p.Phi.Contains("Darkwing Duck");

			// Act
			var result = _classUnderTest.BuildQueryExpressions<Alpha>(queryParameter);

			// Assert
			result.Should().HaveCount(1);
			result.First().Should().BeEquivalentTo(expectedResult);
		}
	}
}