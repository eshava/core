using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Eshava.Core.Validation;
using Eshava.Core.Validation.Attributes;
using Eshava.Test.Core.Validation.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
			rules.Should().HaveCount(13);
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
			alpha.Rules.Single(r => r.Rule == "Range").Minimum.Should().Be(-5);
			alpha.Rules.Single(r => r.Rule == "Range").Maximum.Should().Be(7);
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
		}

		[TestMethod]
		public void CalculateValidationRulesComplexDataTest()
		{
			// Act
			var rules = _classUnderTest.CalculateValidationRules<ComplexData>();

			// Assert
			rules.Should().HaveCount(6);

			var alpha = rules.Single(r => r.PropertyName == nameof(ComplexData.Alpha));
			alpha.DataType.Should().Be("string");
			alpha.Rules.Should().HaveCount(0);
			
			var beta = rules.Single(r => r.PropertyName == nameof(ComplexData.Beta));
			beta.DataType.Should().Be("number");
			beta.Rules.Should().HaveCount(1);
			beta.Rules.SingleOrDefault(r => r.Rule == "Number").Should().NotBeNull();

			var camelCaseNameProperties = rules.Where(r => r.PropertyName == nameof(BasicRules.CamelCaseName));
			var justAnotherPropertyProperties = rules.Where(r => r.PropertyName == nameof(BasicRules.JustAnotherProperty));

			camelCaseNameProperties.Should().HaveCount(2);
			camelCaseNameProperties.First().DataType.Should().Be("string");
			camelCaseNameProperties.First().Rules.Should().HaveCount(1);
			camelCaseNameProperties.First().Rules.Single().Rule.Should().Be("Required");
			camelCaseNameProperties.Last().DataType.Should().Be("string");
			camelCaseNameProperties.Last().Rules.Should().HaveCount(1);
			camelCaseNameProperties.Last().Rules.Single().Rule.Should().Be("Required");

			justAnotherPropertyProperties.Should().HaveCount(2);
			justAnotherPropertyProperties.First().DataType.Should().Be("string");
			justAnotherPropertyProperties.First().Rules.Should().HaveCount(1);
			justAnotherPropertyProperties.First().Rules.Single().Rule.Should().Be("Custom");
			justAnotherPropertyProperties.Last().DataType.Should().Be("string");
			justAnotherPropertyProperties.Last().Rules.Should().HaveCount(1);
			justAnotherPropertyProperties.Last().Rules.Single().Rule.Should().Be("Custom");
		}

		private class BasicRules
		{
			[Required]
			public string CamelCaseName { get; set; }

			[SpecialValidation]
			[JsonProperty("itsNotJustAnotherProperty")]
			public string JustAnotherProperty { get; set; }

			[ValidationIgnore]
			public string IgnoreMe { get; set; }
		}

		private class OnlyStringProperties
		{
			public string Alpha { get; set; }

			[MinLength(2)]
			public string Beta { get; set; }

			[MaxLength(10)]
			public string Gamma { get; set; }

			[MinLength(2)]
			[MaxLength(10)]
			public string Delta { get; set; }

			[EqualsTo("Zeta, Eta")]
			public string Epsilon { get; set; }

			[EqualsTo("Eta")]
			public string Zeta { get; set; }

			[EqualsTo("Zeta")]
			[NotEqualsTo("Theta,Iota", "Omega")]
			public string Eta { get; set; }

			[NotEqualsTo("Iota", "Omega")]
			public string Theta { get; set; }

			[NotEqualsTo("Theta", "Omega")]
			public string Iota { get; set; }
		}

		private class DataTypeAttributeData
		{
			[DataType(DataType.Password)]
			public string Alpha { get; set; }

			[DataType(DataType.DateTime)]
			public DateTime Beta { get; set; }

			[DataType(DataType.Date)]
			public DateTime Gamma { get; set; }

			[DataType(DataType.Time)]
			public DateTime Delta { get; set; }

			[DataType(DataType.MultilineText)]
			public string Epsilon { get; set; }

			[DataType(DataType.EmailAddress)]
			public string Zeta { get; set; }

			[DataType(DataType.Url)]
			public string Eta { get; set; }

			[DataType(DataType.Custom)]
			public Guid Theta { get; set; }

			[DataType(DataType.Custom)]
			public DateTime Iota { get; set; }
		}

		private class DataTypeProperties
		{
			[Range(-5, 7)]
			public int? Alpha { get; set; }

			[RangeTo(nameof(Gamma), true)]
			public long? Beta { get; set; }

			[RangeFrom(nameof(Beta), false)]
			public double? Gamma { get; set; }

			[RangeBetween(nameof(Beta), nameof(Gamma))]
			public float? Delta { get; set; }

			[DecimalPlaces(3)]
			public decimal? Epsilon { get; set; }

			public DateTime? Zeta { get; set; }

			public Guid? Eta { get; set; }

			public bool? Theta { get; set; }

			public Alphabet Iota { get; set; }

			[Tags]
			public string Kappa { get; set; }

			[Typeahead]
			public string Lambda { get; set; }

			[DropDownList]
			public string My { get; set; }

			[DropDownList]
			public int? Ny { get; set; }
		}

		private class ComplexData
		{
			public IEnumerable<string> Alpha { get; set; }
			public IList<int> Beta { get; set; }
			public List<BasicRules> Gamma { get; set; }
			public BasicRules Delta { get; set; }
		}
	}
}