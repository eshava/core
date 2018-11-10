using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Eshava.Core.Communication.Http;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Communication.Http
{
	[TestClass, TestCategory("Core.Communication")]
	public class HttpRequestTest
	{
		[TestInitialize]
		public void Setup()
		{
		
		}

		[TestMethod]
		public void AddSegementParameterTest()
		{
			// Arrange
			var classUnderTest = new HttpRequest("quackfu/{Darkwing}/{Number}", HttpMethod.Post);

			// Act
			classUnderTest.AddSegmentParameter("Darkwing", "Duck");
			classUnderTest.AddSegmentParameter("Number", 7);
			
			// Assert
			classUnderTest.GetUrl(new UrlBuilder(), new Uri("https://eshava.de/")).Should().Be("https://eshava.de/quackfu/Duck/7/");
		}

		[TestMethod]
		public void AddSegementParameterMultipleTest()
		{
			// Arrange
			var classUnderTest = new HttpRequest("quackfu/{Darkwing}/{Number}", HttpMethod.Post);

			// Act
			classUnderTest.AddSegmentParameter("Darkwing", "Duck");
			classUnderTest.AddSegmentParameter("Darkwing", "DuckDuck");
			classUnderTest.AddSegmentParameter("Number", 7);

			// Assert
			classUnderTest.GetUrl(new UrlBuilder(), new Uri("https://eshava.de/")).Should().Be("https://eshava.de/quackfu/DuckDuck/7/");
		}

		[TestMethod]
		public void AddQueryParameterTest()
		{
			// Arrange
			var classUnderTest = new HttpRequest("quackfu", HttpMethod.Post);

			// Act
			classUnderTest.AddQueryParameter("Darkwing", "Duck");
			classUnderTest.AddQueryParameter("Number", 7);

			// Assert
			classUnderTest.GetUrl(new UrlBuilder(), new Uri("https://eshava.de/")).Should().Be("https://eshava.de/quackfu/?Darkwing=Duck&Number=7");
		}

		[TestMethod]
		public void AddQueryParameterMultipleTest()
		{
			// Arrange
			var classUnderTest = new HttpRequest("quackfu", HttpMethod.Post);

			// Act
			classUnderTest.AddQueryParameter("Darkwing", "Duck");
			classUnderTest.AddQueryParameter("Darkwing", "DuckDuck");
			classUnderTest.AddQueryParameter("Number", 7);

			// Assert
			classUnderTest.GetUrl(new UrlBuilder(), new Uri("https://eshava.de/")).Should().Be("https://eshava.de/quackfu/?Darkwing=Duck&Darkwing=DuckDuck&Number=7");
		}

		[TestMethod]
		public void GetUrlTest()
		{
			// Arrange
			var classUnderTest = new HttpRequest("quackfu/{Number}", HttpMethod.Post);
			classUnderTest.AddQueryParameter("Darkwing", "Duck");
			classUnderTest.AddSegmentParameter("Number", 7);

			// Act
			var result = classUnderTest.GetUrl(new UrlBuilder(), new Uri("https://eshava.de/"));

			// Assert
			result.Should().Be("https://eshava.de/quackfu/7/?Darkwing=Duck");
		}
	}
}