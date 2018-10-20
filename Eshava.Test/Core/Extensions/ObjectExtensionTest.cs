using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eshava.Core.Extensions;
using Eshava.Test.Core.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Extensions
{
	[TestClass, TestCategory("Core.Extensions")]
	public class ObjectExtensionTest
	{
		[TestInitialize]
		public void Setup()
		{

		}

		[TestMethod]
		public void ReplaceEmptyStringsToNullWithNullObjectTest()
		{
			// Arrange
			object model = null;

			// Act
			var result = model.ReplaceEmptyStringsToNull();

			// Assert
			result.Should().BeNull();
		}

		[TestMethod]
		public void ReplaceEmptyStringsToNullTest()
		{
			// Arrange
			var model = new Alpha
			{
				Beta = 1,
				Gamma = null,
				Delta = "",
				Epsilon = "DarkwingDuck",
				Zeta = null,
				Eta = new List<string>
				{
					"",
					"QuackFu"
				},
				Theta = new List<int>
				{
					1,
					2,
					3
				},
				Iota = new List<Omega>
				{
					new Omega
					{
						Chi = "Let’s Get Dangerous",
						Psi = ""
					},
					new Omega
					{
						Chi = "",
						Psi = "Darkwing Duck is Coming Back to Cartoons!"
					}
				},
				Kappa = new Omega
				{
					Chi = "Launchpad McQuack",
					Psi = ""
				}
			};

			// Act
			var result = model.ReplaceEmptyStringsToNull();

			// Assert
			var alphaResult = result as Alpha;
			alphaResult.Beta.Should().Be(1);
			alphaResult.Gamma.Should().BeNull();
			alphaResult.Delta.Should().BeNull();
			alphaResult.Epsilon.Should().Be("DarkwingDuck");
			alphaResult.Zeta.Should().BeNull();
			alphaResult.Eta.Should().HaveCount(1);
			alphaResult.Eta.First().Should().Be("QuackFu");
			alphaResult.Theta.Should().HaveCount(3);
			alphaResult.Theta.Single(e => e == 1).Should().Be(1);
			alphaResult.Theta.Single(e => e == 2).Should().Be(2);
			alphaResult.Theta.Single(e => e == 3).Should().Be(3);
			alphaResult.Iota.Should().HaveCount(2);
			alphaResult.Iota.First().Chi.Should().Be("Let’s Get Dangerous");
			alphaResult.Iota.First().Psi.Should().BeNull();
			alphaResult.Iota.Last().Chi.Should().BeNull();
			alphaResult.Iota.Last().Psi.Should().Be("Darkwing Duck is Coming Back to Cartoons!");
			alphaResult.Kappa.Chi.Should().Be("Launchpad McQuack");
			alphaResult.Kappa.Psi.Should().BeNull();
		}
	}
}