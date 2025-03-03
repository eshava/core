using System.Linq;
using Eshava.Core.Validation;
using Eshava.Test.Core.Validation.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Validation
{
	[TestClass, TestCategory("Core.Validation")]
	public class ValidationRuleEngineTest
	{
		private ValidationRuleEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new ValidationRuleEngine();
		}

		[TestMethod]
		public void CalculateValidationRulesBasicTest()
		{
			// Act
			var rules = _classUnderTest.CalculateValidationRules<BasicRules>();

			// Assert
			rules.Should().HaveCount(2);

			var one = rules.Single(r => r.PropertyName == nameof(BasicRules.CamelCaseName));
			var two = rules.Single(r => r.PropertyName == nameof(BasicRules.JustAnotherProperty));

			one.JsonName.Should().Be("camelCaseName");
			one.IsDynamicField.Should().BeFalse();
			one.Rules.Should().HaveCount(1);
			one.Rules.Single().Rule.Should().Be("Required");

			two.JsonName.Should().Be("itsNotJustAnotherProperty");
			two.IsDynamicField.Should().BeFalse();
			two.Rules.Should().HaveCount(1);
			two.Rules.Single().Rule.Should().Be("Custom");
		}

		[TestMethod]
		public void CalculateValidationRulesStringRulesTest()
		{
			// Act
			var rules = _classUnderTest.CalculateValidationRules<OnlyStringProperties>();

			// Assert
			rules.Should().HaveCount(9);
			var alpha = rules.Single(r => r.PropertyName == nameof(OnlyStringProperties.Alpha));
			alpha.DataType.Should().Be("string");
			alpha.Rules.Should().HaveCount(0);

			var beta = rules.Single(r => r.PropertyName == nameof(OnlyStringProperties.Beta));
			beta.DataType.Should().Be("string");
			beta.Rules.Should().HaveCount(1);
			beta.Rules.Single().Rule.Should().Be("MinLength");
			beta.Rules.Single().Value.Should().Be(2);

			var gamma = rules.Single(r => r.PropertyName == nameof(OnlyStringProperties.Gamma));
			gamma.DataType.Should().Be("string");
			gamma.Rules.Should().HaveCount(1);
			gamma.Rules.Single().Rule.Should().Be("MaxLength");
			gamma.Rules.Single().Value.Should().Be(10);

			var delta = rules.Single(r => r.PropertyName == nameof(OnlyStringProperties.Delta));
			delta.DataType.Should().Be("string");
			delta.Rules.Should().HaveCount(2);
			delta.Rules.Last().Rule.Should().Be("MinLength");
			delta.Rules.Last().Value.Should().Be(2);
			delta.Rules.First().Rule.Should().Be("MaxLength");
			delta.Rules.First().Value.Should().Be(10);

			var epsilon = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Epsilon));
			epsilon.DataType.Should().Be("string");
			epsilon.Rules.Should().HaveCount(2);
			epsilon.Rules.First().Rule.Should().Be("EqualsTo");
			epsilon.Rules.First().PropertyName.Should().Be(nameof(DataTypeAttributeData.Zeta));
			epsilon.Rules.Last().Rule.Should().Be("EqualsTo");
			epsilon.Rules.Last().PropertyName.Should().Be(nameof(DataTypeAttributeData.Eta));


			var zeta = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Zeta));
			zeta.DataType.Should().Be("string");
			zeta.Rules.Should().HaveCount(1);
			zeta.Rules.Single().Rule.Should().Be("EqualsTo");
			zeta.Rules.Single().PropertyName.Should().Be(nameof(DataTypeAttributeData.Eta));

			var eta = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Eta));
			eta.DataType.Should().Be("string");
			eta.Rules.Should().HaveCount(3);
			eta.Rules.First().Rule.Should().Be("EqualsTo");
			eta.Rules.First().PropertyName.Should().Be(nameof(DataTypeAttributeData.Zeta));
			eta.Rules.First(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be(nameof(DataTypeAttributeData.Theta));
			eta.Rules.First(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be("Omega");
			eta.Rules.Last(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be(nameof(DataTypeAttributeData.Iota));
			eta.Rules.Last(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be("Omega");

			var theta = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Theta));
			theta.DataType.Should().Be("string");
			theta.Rules.Should().HaveCount(1);
			theta.Rules.Single().Rule.Should().Be("NotEqualsTo");
			theta.Rules.Single().PropertyName.Should().Be(nameof(DataTypeAttributeData.Iota));
			theta.Rules.Single().DefaultValue.Should().Be("Omega");

			var iota = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Iota));
			iota.DataType.Should().Be("string");
			iota.Rules.Should().HaveCount(1);
			iota.Rules.Single().Rule.Should().Be("NotEqualsTo");
			iota.Rules.Single().PropertyName.Should().Be(nameof(DataTypeAttributeData.Theta));
			iota.Rules.Single().DefaultValue.Should().Be("Omega");
		}

		[TestMethod]
		public void CalculateValidationRulesDataTypeAttributeTest()
		{
			// Act
			var rules = _classUnderTest.CalculateValidationRules<DataTypeAttributeData>();

			// Assert
			rules.Should().HaveCount(9);
			var alpha = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Alpha));
			alpha.DataType.Should().Be("string");
			alpha.Rules.Should().HaveCount(1);
			alpha.Rules.Single().Rule.Should().Be("Password");

			var beta = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Beta));
			beta.DataType.Should().Be("dateTime");
			beta.Rules.Should().HaveCount(0);

			var gamma = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Gamma));
			gamma.DataType.Should().Be("date");
			gamma.Rules.Should().HaveCount(0);

			var delta = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Delta));
			delta.DataType.Should().Be("time");
			delta.Rules.Should().HaveCount(0);

			var epsilon = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Epsilon));
			epsilon.DataType.Should().Be("multiline");
			epsilon.Rules.Should().HaveCount(0);

			var zeta = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Zeta));
			zeta.DataType.Should().Be("string");
			zeta.Rules.Should().HaveCount(1);
			zeta.Rules.Single().Rule.Should().Be("Email");

			var eta = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Eta));
			eta.DataType.Should().Be("string");
			eta.Rules.Should().HaveCount(1);
			eta.Rules.Single().Rule.Should().Be("Url");

			var theta = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Theta));
			theta.DataType.Should().Be("string");
			theta.Rules.Should().HaveCount(1);
			theta.Rules.Single().Rule.Should().Be("Guid");

			var iota = rules.Single(r => r.PropertyName == nameof(DataTypeAttributeData.Iota));
			iota.DataType.Should().Be("dateTime");
			iota.Rules.Should().HaveCount(0);
		}

		[TestMethod]
		public void CalculateValidationRulesDataTypeTest()
		{
			// Act
			var rules = _classUnderTest.CalculateValidationRules<DataTypeProperties>();

			// Assert
			rules.Should().HaveCount(14);
			var alpha = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Alpha));
			alpha.DataType.Should().Be("number");
			alpha.Rules.Should().HaveCount(3);
			alpha.Rules.Single(r => r.Rule == "DecimalPlaces").Value.Should().Be(0);
			alpha.Rules.Single(r => r.Rule == "Range").Minimum.Should().Be(-5);
			alpha.Rules.Single(r => r.Rule == "Range").Maximum.Should().Be(7);
			alpha.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var beta = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Beta));
			beta.DataType.Should().Be("number");
			beta.Rules.Should().HaveCount(3);
			beta.Rules.Single(r => r.Rule == "DecimalPlaces").Value.Should().Be(0);
			beta.Rules.Single(r => r.Rule == "RangeTo").PropertyNameTo.Should().Be(nameof(DataTypeProperties.Gamma));
			beta.Rules.Single(r => r.Rule == "RangeTo").PropertyNameToAllowNull.Should().BeTrue();
			beta.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var gamma = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Gamma));
			gamma.DataType.Should().Be("number");
			gamma.Rules.Should().HaveCount(3);
			gamma.Rules.Single(r => r.Rule == "DecimalPlaces").Value.Should().Be(0);
			gamma.Rules.Single(r => r.Rule == "RangeFrom").PropertyNameFrom.Should().Be(nameof(DataTypeProperties.Beta));
			gamma.Rules.Single(r => r.Rule == "RangeFrom").PropertyNameFromAllowNull.Should().BeFalse();
			gamma.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var delta = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Delta));
			delta.DataType.Should().Be("number");
			delta.Rules.Should().HaveCount(3);
			delta.Rules.Single(r => r.Rule == "DecimalPlaces").Value.Should().Be(0);
			delta.Rules.Single(r => r.Rule == "RangeBetween").PropertyNameFrom.Should().Be(nameof(DataTypeProperties.Beta));
			delta.Rules.Single(r => r.Rule == "RangeBetween").PropertyNameTo.Should().Be(nameof(DataTypeProperties.Gamma));
			delta.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var epsilon = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Epsilon));
			epsilon.DataType.Should().Be("number");
			epsilon.Rules.Should().HaveCount(2);
			epsilon.Rules.Single(r => r.Rule == "DecimalPlaces").Value.Should().Be(3);
			epsilon.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var zeta = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Zeta));
			zeta.DataType.Should().Be("dateTime");
			zeta.Rules.Should().HaveCount(0);

			var eta = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Eta));
			eta.DataType.Should().Be("select");
			eta.Rules.Should().HaveCount(1);
			eta.Rules.Single().Rule.Should().Be("Guid");

			var theta = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Theta));
			theta.DataType.Should().Be("boolean");
			theta.Rules.Should().HaveCount(0);

			var iota = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Iota));
			iota.DataType.Should().Be("select");
			iota.Rules.Should().HaveCount(1);
			iota.Rules.Single().Rule.Should().Be("Number");

			var kappa = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Kappa));
			kappa.DataType.Should().Be("tag");
			kappa.Rules.Should().HaveCount(0);

			var lambda = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Lambda));
			lambda.DataType.Should().Be("typeahead");
			lambda.Rules.Should().HaveCount(0);

			var my = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.My));
			my.DataType.Should().Be("select");
			my.Rules.Should().HaveCount(0);

			var ny = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Ny));
			ny.DataType.Should().Be("select");
			ny.Rules.Should().HaveCount(2);
			ny.Rules.Single(r => r.Rule == "DecimalPlaces").Value.Should().Be(0);
			ny.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var xi = rules.Single(r => r.PropertyName == nameof(DataTypeProperties.Xi));
			xi.DataType.Should().Be("number");
			xi.Rules.Should().HaveCount(3);
			xi.Rules.Single(r => r.Rule == "DecimalPlaces").Value.Should().Be(3);
			xi.Rules.Single(r => r.Rule == "Range").Minimum.Should().Be(0.001m);
			xi.Rules.Single(r => r.Rule == "Range").Maximum.Should().Be(1.001m);
			xi.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();
		}

		[TestMethod]
		public void CalculateValidationRulesComplexDataTest()
		{
			// Act
			var rules = _classUnderTest.CalculateValidationRules<ComplexData>();

			// Assert
			rules.Should().HaveCount(10);

			var alpha = rules.Single(r => r.PropertyName == nameof(ComplexData.Alpha));
			alpha.DataType.Should().Be("string");
			alpha.Rules.Should().HaveCount(0);

			var beta = rules.Single(r => r.PropertyName == nameof(ComplexData.Beta));
			beta.DataType.Should().Be("number");
			beta.Rules.Should().HaveCount(1);
			beta.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var camelCaseNameProperties = rules.Where(r => r.PropertyName == nameof(BasicRules.CamelCaseName));
			var justAnotherPropertyProperties = rules.Where(r => r.PropertyName == nameof(BasicRules.JustAnotherProperty));

			camelCaseNameProperties.Should().HaveCount(1);
			camelCaseNameProperties.Single().DataType.Should().Be("string");
			camelCaseNameProperties.Single().Rules.Should().HaveCount(1);
			camelCaseNameProperties.Single().Rules.Single().Rule.Should().Be("Required");

			justAnotherPropertyProperties.Should().HaveCount(1);
			justAnotherPropertyProperties.Single().DataType.Should().Be("string");
			justAnotherPropertyProperties.Single().Rules.Should().HaveCount(1);
			justAnotherPropertyProperties.Single().Rules.Single().Rule.Should().Be("Custom");

			var epsilon = rules.Single(r => r.PropertyName == nameof(ComplexData.Epsilon));
			epsilon.DataType.Should().Be("string");
			epsilon.Rules.Should().HaveCount(1);
			epsilon.Rules.SingleOrDefault(r => r.Rule == "RegularExpression").Should().NotBeNull();
			epsilon.Rules.SingleOrDefault(r => r.RegEx == ComplexData.EPSILONFORMAT).Should().NotBeNull();

			var zeta = rules.Single(r => r.PropertyName == nameof(ComplexData.Zeta));
			zeta.DataType.Should().Be("string");
			zeta.Rules.Should().HaveCount(1);
			zeta.Rules.SingleOrDefault(r => r.Rule == "ReadOnly").Should().NotBeNull();

			var eta = rules.Single(r => r.PropertyName == nameof(ComplexData.Eta));
			eta.DataType.Should().Be("string");
			eta.Rules.Should().HaveCount(0);

			var primaryColor = rules.Single(r => r.PropertyName == nameof(ComplexData.PrimaryColor));
			primaryColor.DataType.Should().Be("select");
			primaryColor.Rules.Should().HaveCount(1);
			primaryColor.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var secondaryColor = rules.Single(r => r.PropertyName == nameof(ComplexData.SecondaryColor));
			secondaryColor.DataType.Should().Be("select");
			secondaryColor.Rules.Should().HaveCount(1);
			secondaryColor.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var baseColor = rules.Single(r => r.PropertyName == nameof(ComplexData.BaseColor));
			baseColor.DataType.Should().Be("select");
			baseColor.Rules.Should().HaveCount(2);
			baseColor.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();
			baseColor.Rules.SingleOrDefault(r => r.Rule == "Required").Should().NotBeNull();
		}

		[TestMethod]
		public void CalculateValidationRulesComplexDataAsTreeStructureTest()
		{
			// Act
			var rules = _classUnderTest.CalculateValidationRules<ComplexData>(true);

			// Assert
			rules.Should().HaveCount(10);

			var alpha = rules.Single(r => r.PropertyName == nameof(ComplexData.Alpha));
			alpha.DataType.Should().Be("string");
			alpha.Rules.Should().HaveCount(0);

			var beta = rules.Single(r => r.PropertyName == nameof(ComplexData.Beta));
			beta.DataType.Should().Be("number");
			beta.Rules.Should().HaveCount(1);
			beta.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var gammeRules = rules.First(r => r.IsClassContainer && r.PropertyName == nameof(ComplexData.Gamma));
			var gammaCamelCaseNameProperties = gammeRules.Properties.Where(r => r.PropertyName == nameof(BasicRules.CamelCaseName));
			var gammaJustAnotherPropertyProperties = gammeRules.Properties.Where(r => r.PropertyName == nameof(BasicRules.JustAnotherProperty));
			gammaCamelCaseNameProperties.Should().HaveCount(1);
			gammaCamelCaseNameProperties.Single().DataType.Should().Be("string");
			gammaCamelCaseNameProperties.Single().Rules.Should().HaveCount(1);
			gammaCamelCaseNameProperties.Single().Rules.Single().Rule.Should().Be("Required");

			gammaJustAnotherPropertyProperties.Should().HaveCount(1);
			gammaJustAnotherPropertyProperties.Single().DataType.Should().Be("string");
			gammaJustAnotherPropertyProperties.Single().Rules.Should().HaveCount(1);
			gammaJustAnotherPropertyProperties.Single().Rules.Single().Rule.Should().Be("Custom");

			var deltaRules = rules.First(r => r.IsClassContainer && r.PropertyName == nameof(ComplexData.Delta));
			var deltaCamelCaseNameProperties = deltaRules.Properties.Where(r => r.PropertyName == nameof(BasicRules.CamelCaseName));
			var deltaJustAnotherPropertyProperties = deltaRules.Properties.Where(r => r.PropertyName == nameof(BasicRules.JustAnotherProperty));
			deltaCamelCaseNameProperties.Should().HaveCount(1);
			deltaCamelCaseNameProperties.Single().DataType.Should().Be("string");
			deltaCamelCaseNameProperties.Single().Rules.Should().HaveCount(1);
			deltaCamelCaseNameProperties.Single().Rules.Single().Rule.Should().Be("Required");

			deltaJustAnotherPropertyProperties.Should().HaveCount(1);
			deltaJustAnotherPropertyProperties.Single().DataType.Should().Be("string");
			deltaJustAnotherPropertyProperties.Single().Rules.Should().HaveCount(1);
			deltaJustAnotherPropertyProperties.Single().Rules.Single().Rule.Should().Be("Custom");

			var epsilon = rules.Single(r => r.PropertyName == nameof(ComplexData.Epsilon));
			epsilon.DataType.Should().Be("string");
			epsilon.Rules.Should().HaveCount(1);
			epsilon.Rules.SingleOrDefault(r => r.Rule == "RegularExpression").Should().NotBeNull();
			epsilon.Rules.SingleOrDefault(r => r.RegEx == ComplexData.EPSILONFORMAT).Should().NotBeNull();

			var zeta = rules.Single(r => r.PropertyName == nameof(ComplexData.Zeta));
			zeta.DataType.Should().Be("string");
			zeta.Rules.Should().HaveCount(1);
			zeta.Rules.SingleOrDefault(r => r.Rule == "ReadOnly").Should().NotBeNull();

			var eta = rules.Single(r => r.PropertyName == nameof(ComplexData.Eta));
			eta.DataType.Should().Be("string");
			eta.Rules.Should().HaveCount(0);

			var primaryColor = rules.Single(r => r.PropertyName == nameof(ComplexData.PrimaryColor));
			primaryColor.DataType.Should().Be("select");
			primaryColor.Rules.Should().HaveCount(1);
			primaryColor.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var secondaryColor = rules.Single(r => r.PropertyName == nameof(ComplexData.SecondaryColor));
			secondaryColor.DataType.Should().Be("select");
			secondaryColor.Rules.Should().HaveCount(1);
			secondaryColor.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var baseColor = rules.Single(r => r.PropertyName == nameof(ComplexData.BaseColor));
			baseColor.DataType.Should().Be("select");
			baseColor.Rules.Should().HaveCount(2);
			baseColor.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();
			baseColor.Rules.SingleOrDefault(r => r.Rule == "Required").Should().NotBeNull();
		}
	}
}