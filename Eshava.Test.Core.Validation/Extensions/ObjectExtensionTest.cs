using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Validation.Extension;
using Eshava.Test.Core.Validation.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Validation.Extensions
{
	[TestClass, TestCategory("Core.Validation.Extensions")]
	public class ObjectExtensionTest
	{

		[TestInitialize]
		public void Setup()
		{

		}

		[TestMethod]
		public void CleanUpAndSubstringStringPropertiesWithNullInputTest()
		{
			// Arrange
			Alpha source = null;

			// Act
			var result = source.CleanUpAndSubstringStringProperties();

			// Assert
			result.Should().BeNull();
		}

		[TestMethod]
		public void CleanUpAndSubstringStringPropertiesTest()
		{
			// Arrange
			var source = new Alpha(7, "Darkwing Duck", "Darkwing Duck")
			{
				Delta = "",
				DeltaTwo = "Delta",
				Epsilon = null,
				Iota = new List<Omega>
				{
					new Omega
					{
						Chi = "Launchpad McQuack in action",
						Psi = ""
					},
					new Omega
					{
						Chi = "",
						Psi = "QuackFu"
					}
				},
				Kappa = new Omega
				{
					Chi = "Launchpad McQuack in action",
					Psi = ""
				},
				Ypsilon = new List<string> { "Darkwing Duck", "", null, "QuackFu" }
			};

			// Act
			var result = source.CleanUpAndSubstringStringProperties();

			// Assert
			result.Should().NotBeNull();
			var alphaResult = result as Alpha;
			alphaResult.Gamma.Should().Be("Darkwing D");
			alphaResult.Delta.Should().BeNull();
			alphaResult.DeltaTwo.Should().Be("Delta");
			alphaResult.Epsilon.Should().BeNull();
			alphaResult.Iota.Should().HaveCount(2);
			alphaResult.Iota.First().Chi.Should().Be("Launchpad McQuack in");
			alphaResult.Iota.First().Psi.Should().BeNull();
			alphaResult.Iota.Last().Chi.Should().BeNull();
			alphaResult.Iota.Last().Psi.Should().Be("QuackFu");
			alphaResult.Kappa.Chi.Should().Be("Launchpad McQuack in");
			alphaResult.Kappa.Psi.Should().BeNull();
			alphaResult.Ypsilon.Should().HaveCount(2);
			alphaResult.Ypsilon.First().Should().Be("Darkwing D");
			alphaResult.Ypsilon.Last().Should().Be("QuackFu");
		}
	}
}