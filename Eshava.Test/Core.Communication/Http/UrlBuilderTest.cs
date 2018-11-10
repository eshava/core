using System;
using System.Collections.Generic;
using Eshava.Core.Communication.Http;
using Eshava.Core.Communication.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Communication.Http
{
	[TestClass, TestCategory("Core.Communication")]
	public class UrlBuilderTest
	{
		private UrlBuilder _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new UrlBuilder();
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetSegementParameterNameWithNullInputTest()
		{
			// Act
			_classUnderTest.GetSegementParameterName(null);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetSegementParameterNameWithEmptyInputTest()
		{
			// Act
			_classUnderTest.GetSegementParameterName("");
		}

		[TestMethod]
		public void TestGetSegementParameterName()
		{
			// Arrange
			var name = "DarkwingDuck";

			// Act
			var result = _classUnderTest.GetSegementParameterName(name);

			// Assert
			result.Should().Be("{" + name + "}");
		}

		[TestMethod, ExpectedException(typeof(NullReferenceException))]
		public void BuildWithOnlyNullInputsTest()
		{
			// Act
			_classUnderTest.Build(null);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void BuildWithBaseUrlNullTest()
		{
			// Act
			_classUnderTest.Build(new UrlBuilderSettings());
		}

		[TestMethod]
		public void BuildWithOnlyBaseUrlTest()
		{
			// Arrange
			var settings = new UrlBuilderSettings
			{
				BaseUrl = new Uri("http://quack.fu/duck")
			};

			// Act
			var result = _classUnderTest.Build(settings);

			// Assert
			result.Should().Be("http://quack.fu/duck/");
		}

		[TestMethod]
		public void BuildWithSegmentParameterTest()
		{
			// Arrange
			var settings = new UrlBuilderSettings
			{
				BaseUrl = new Uri("http://quack.fu"),
				SegmentParameter = new Dictionary<string, object>
				{
					{ "darkwing", "duck" },
					{ "quackfu", Guid.Empty },
					{ "number", 7 }
				}
			};

			// Act
			var result = _classUnderTest.Build(settings);

			// Assert
			result.Should().Be("http://quack.fu/");

		}

		[TestMethod]
		public void BuildWithSegmentParameterAndSegmentTest()
		{
			// Arrange
			var settings = new UrlBuilderSettings
			{
				BaseUrl = new Uri("http://quack.fu"),
				Segment = "config/{darkwing}/{quackfu}/{number}",
				SegmentParameter = new Dictionary<string, object>
				{
					{ "darkwing", "duck" },
					{ "quackfu", Guid.Empty },
					{ "number", 7 }
				}
			};

			// Act
			var result = _classUnderTest.Build(settings);

			// Assert
			result.Should().Be($"http://quack.fu/config/duck/{Guid.Empty}/7/");
		}

		[TestMethod]
		public void BuildWithQueryParameterTest()
		{
			// Arrange
			var settings = new UrlBuilderSettings
			{
				BaseUrl = new Uri("http://quack.fu"),
				QueryParameter = new List<(string Name, object Value)>
				{
					(Name: "darkwing", Value: "duck"),
					(Name: "quackfu", Value: Guid.Empty),
					(Name: "number", Value: 7)
				}
			};

			// Act
			var result = _classUnderTest.Build(settings);

			// Assert
			result.Should().Be($"http://quack.fu/?darkwing=duck&quackfu={Guid.Empty}&number=7");
		}

		[TestMethod]
		public void BuildWithQueryParameterAndSegmentTest()
		{
			// Arrange
			var settings = new UrlBuilderSettings
			{
				BaseUrl = new Uri("http://quack.fu"),
				Segment = "config",
				QueryParameter = new List<(string Name, object Value)>
				{
					(Name: "darkwing", Value: "duck"),
					(Name: "quackfu", Value: Guid.Empty),
					(Name: "number", Value: 7)
				}
			};

			// Act
			var result = _classUnderTest.Build(settings);

			// Assert
			result.Should().Be($"http://quack.fu/config/?darkwing=duck&quackfu={Guid.Empty}&number=7");
		}


		[TestMethod]
		public void BuildWithSegmentAndQueryParameterTest()
		{
			// Arrange
			var settings = new UrlBuilderSettings
			{
				BaseUrl = new Uri("http://quack.fu"),
				Segment = "config/{darkwing}",
				SegmentParameter = new Dictionary<string, object>
				{
					{ "darkwing", "duck" },
				},
				QueryParameter = new List<(string Name, object Value)>
				{
					(Name: "quackfu", Value: Guid.Empty),
					(Name: "number", Value: 7)
				}
			};

			// Act
			var result = _classUnderTest.Build(settings);

			// Assert
			result.Should().Be($"http://quack.fu/config/duck/?quackfu={Guid.Empty}&number=7");
		}
	}
}
