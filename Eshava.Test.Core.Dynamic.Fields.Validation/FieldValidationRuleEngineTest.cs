using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation;
using Eshava.Core.Extensions;
using Eshava.Test.Core.Dynamic.Fields.Validation.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Dynamic.Fields.Validation
{
	[TestClass, TestCategory("Core.Dynamic.Fields.Validation")]
	public class FieldValidationRuleEngineTest
	{
		private FieldValidationRuleEngine<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int> _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new FieldValidationRuleEngine<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>();
		}

		[DataTestMethod]
		[DataRow(FieldType.None)]
		[DataRow(FieldType.DynamicCode)]
		public void CalculateValidationRuleNoRulesTest(FieldType fieldType)
		{
			// Arrange
			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(fieldType)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldType)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(0);
		}

		[DataTestMethod]
		[DataRow(FieldType.Text)]
		[DataRow(FieldType.TextMultiline)]
		[DataRow(FieldType.DateTime)]
		[DataRow(FieldType.Checkbox)]
		[DataRow(FieldType.AutoComplete)]
		[DataRow(FieldType.BoxedCheckbox)]
		public void CalculateValidationRuleDataTypeTest(FieldType fieldType)
		{
			// Arrange
			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(fieldType)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldType)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(1);

			rules.Single().PropertyName.Should().Be(fieldType.ToString());
			rules.Single().JsonName.Should().Be(fieldType.ToString());
			rules.Single().IsDynamicField.Should().BeTrue();
			rules.Single().Rules.Should().HaveCount(0);
		}

		[DataTestMethod]
		[DataRow(FieldType.Text, "string", FieldConfigurationType.Required, "Required")]
		[DataRow(FieldType.Text, "string", FieldConfigurationType.Email, "Email")]
		[DataRow(FieldType.Text, "string", FieldConfigurationType.Url, "Url")]
		[DataRow(FieldType.ComboBoxInteger, "select", null, "Number")]
		[DataRow(FieldType.ComboxBoxGuid, "select", null, "Guid")]
		[DataRow(FieldType.Guid, "string", null, "Guid")]
		[DataRow(FieldType.DateTime, "date", FieldConfigurationType.Date, null)]
		[DataRow(FieldType.DateTime, "time", FieldConfigurationType.Time, null)]
		public void CalculateValidationRuleSimpleRulesTest(FieldType fieldType, string dataType, FieldConfigurationType? fieldConfigurationType, string ruleName)
		{
			// Arrange
			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(fieldType, fieldConfigurationType)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldType)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(1);
			rules.Single().DataType.Should().Be(dataType);
			rules.Single().PropertyName.Should().Be(fieldType.ToString());
			rules.Single().JsonName.Should().Be(fieldType.ToString());
			rules.Single().IsDynamicField.Should().BeTrue();

			if (ruleName.IsNullOrEmpty())
			{
				rules.Single().Rules.Should().HaveCount(0);
			}
			else
			{
				rules.Single().Rules.Should().HaveCount(1);
				rules.Single().Rules.Single().Rule.Should().Be(ruleName);
			}
		}

		[TestMethod]
		public void CalculateValidationRulesStringLengthTest()
		{
			// Arrange
			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.Text, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.MinLength, ValueInteger = 5 },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.MaxLength, ValueInteger = 10 }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinition
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(1);

			rules.Single().PropertyName.Should().Be("Text");
			rules.Single().JsonName.Should().Be("Text");
			rules.Single().IsDynamicField.Should().BeTrue();
			rules.Single().Rules.Should().HaveCount(2);
			rules.Single().Rules.First().Rule.Should().Be("MinLength");
			rules.Single().Rules.First().Value.Should().Be(5);
			rules.Single().Rules.Last().Rule.Should().Be("MaxLength");
			rules.Single().Rules.Last().Value.Should().Be(10);
		}

		[TestMethod]
		public void CalculateValidationRulesRangeTest()
		{
			// Arrange
			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.NumberInteger, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum, ValueInteger = 5 },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum, ValueInteger = 10 }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinition
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberInteger)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.NumberInteger)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(1);

			rules.Single().PropertyName.Should().Be("NumberInteger");
			rules.Single().JsonName.Should().Be("NumberInteger");
			rules.Single().IsDynamicField.Should().BeTrue();
			rules.Single().Rules.Should().HaveCount(3);
			rules.Single().Rules[0].Rule.Should().Be("Number");
			rules.Single().Rules[1].Rule.Should().Be("DecimalPlaces");
			rules.Single().Rules[1].Value.Should().Be(0);
			rules.Single().Rules[2].Rule.Should().Be("Range");
			rules.Single().Rules[2].Minimum.Should().Be(5);
			rules.Single().Rules[2].Maximum.Should().Be(10);
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, 0)]
		[DataRow(FieldType.NumberDecimal, 1)]
		[DataRow(FieldType.NumberDouble, 2)]
		[DataRow(FieldType.NumberFloat, 3)]
		public void CalculateValidationRuleDecimalPlacesTest(FieldType fieldType, int decimalPlaces)
		{
			// Arrange
			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(fieldType, FieldConfigurationType.DecimalPlaces)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldType)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType)
				}
			};

			fieldInformation.Definitions.Single().Configurations.Single().ValueInteger = decimalPlaces;

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(1);
			rules.Single().DataType.Should().Be("number");
			rules.Single().PropertyName.Should().Be(fieldType.ToString());
			rules.Single().JsonName.Should().Be(fieldType.ToString());
			rules.Single().IsDynamicField.Should().BeTrue();

			rules.Single().Rules.Should().HaveCount(2);
			rules.Single().Rules[0].Rule.Should().Be("Number");
			rules.Single().Rules[1].Rule.Should().Be("DecimalPlaces");
			rules.Single().Rules[1].Value.Should().Be(decimalPlaces);
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger)]
		[DataRow(FieldType.NumberDecimal)]
		[DataRow(FieldType.NumberDouble)]
		[DataRow(FieldType.NumberFloat)]
		public void CalculateValidationRulesRangeFromToTest(FieldType fieldType)
		{
			// Arrange
			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(fieldType, null, "From");
			fieldDefinitionFrom.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeTo, ValueString = fieldType.ToString() + "ValueTo" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull }
			};

			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(fieldType, null, "To");
			fieldDefinitionTo.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeFrom, ValueString = fieldType.ToString() + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldType, "From"),
					ConfigurationHelper.CreateAssignment(fieldType, "To")
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType, "From"),
					ConfigurationHelper.CreateField(fieldType, "To")
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(2);

			var ruleFrom = rules.First();
			ruleFrom.PropertyName.Should().Be(fieldType.ToString() + "From");
			ruleFrom.JsonName.Should().Be(fieldType.ToString() + "From");
			ruleFrom.IsDynamicField.Should().BeTrue();
			ruleFrom.Rules.Should().HaveCount(3);
			ruleFrom.Rules[0].Rule.Should().Be("Number");
			ruleFrom.Rules[1].Rule.Should().Be("DecimalPlaces");
			ruleFrom.Rules[1].Value.Should().Be(0);
			ruleFrom.Rules[2].Rule.Should().Be("RangeTo");
			ruleFrom.Rules[2].PropertyNameTo.Should().Be(fieldType.ToString() + "ValueTo");
			ruleFrom.Rules[2].PropertyNameToAllowNull.Should().BeTrue();

			var ruleTo = rules.Last();
			ruleTo.PropertyName.Should().Be(fieldType.ToString() + "To");
			ruleTo.JsonName.Should().Be(fieldType.ToString() + "To");
			ruleTo.IsDynamicField.Should().BeTrue();
			ruleTo.Rules.Should().HaveCount(3);
			ruleTo.Rules[0].Rule.Should().Be("Number");
			ruleTo.Rules[1].Rule.Should().Be("DecimalPlaces");
			ruleTo.Rules[1].Value.Should().Be(0);
			ruleTo.Rules[2].Rule.Should().Be("RangeFrom");
			ruleTo.Rules[2].PropertyNameFrom.Should().Be(fieldType.ToString() + "ValueFrom");
			ruleTo.Rules[2].PropertyNameFromAllowNull.Should().BeTrue();
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger)]
		[DataRow(FieldType.NumberDecimal)]
		[DataRow(FieldType.NumberDouble)]
		[DataRow(FieldType.NumberFloat)]
		public void CalculateValidationRulesRangeBetweenTest(FieldType fieldType)
		{
			// Arrange
			var fieldDefinition = ConfigurationHelper.CreateDefinition(fieldType, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = fieldType.ToString() + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = fieldType.ToString() + "ValueTo" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(fieldType, null, "From"),
					ConfigurationHelper.CreateDefinition(fieldType, null, "To"),
					fieldDefinition
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldType, "From"),
					ConfigurationHelper.CreateAssignment(fieldType, "To"),
					ConfigurationHelper.CreateAssignment(fieldType)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType, "From"),
					ConfigurationHelper.CreateField(fieldType, "To"),
					ConfigurationHelper.CreateField(fieldType)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(3);

			var ruleFrom = rules.Last();
			ruleFrom.PropertyName.Should().Be(fieldType.ToString());
			ruleFrom.JsonName.Should().Be(fieldType.ToString());
			ruleFrom.IsDynamicField.Should().BeTrue();
			ruleFrom.Rules.Should().HaveCount(3);
			ruleFrom.Rules[0].Rule.Should().Be("Number");
			ruleFrom.Rules[1].Rule.Should().Be("DecimalPlaces");
			ruleFrom.Rules[1].Value.Should().Be(0);
			ruleFrom.Rules[2].Rule.Should().Be("RangeBetween");
			ruleFrom.Rules[2].PropertyNameFrom.Should().Be(fieldType.ToString() + "ValueFrom");
			ruleFrom.Rules[2].PropertyNameTo.Should().Be(fieldType.ToString() + "ValueTo");
			ruleFrom.Rules[2].PropertyNameFromAllowNull.Should().BeTrue();
			ruleFrom.Rules[2].PropertyNameToAllowNull.Should().BeTrue();
		}

		[TestMethod]
		public void CalculateValidationRulesEqualsToTest()
		{
			// Arrange
			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.Text, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.EqualsTo, ValueString = "DarkwingDuck" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.EqualsTo, ValueString = "LaunchpadMcQuack" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinition
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(1);

			var rule = rules.Single();
			rule.PropertyName.Should().Be(nameof(FieldType.Text));
			rule.JsonName.Should().Be(nameof(FieldType.Text));
			rule.IsDynamicField.Should().BeTrue();
			rule.Rules.Should().HaveCount(2);
			rule.Rules[0].Rule.Should().Be("EqualsTo");
			rule.Rules[0].PropertyName.Should().Be("DarkwingDuck");
			rule.Rules[1].Rule.Should().Be("EqualsTo");
			rule.Rules[1].PropertyName.Should().Be("LaunchpadMcQuack");
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, nameof(DynamicFieldConfiguration.ValueInteger), 1)]
		[DataRow(FieldType.ComboBoxInteger, nameof(DynamicFieldConfiguration.ValueInteger), 2)]
		[DataRow(FieldType.NumberLong, nameof(DynamicFieldConfiguration.ValueLong), 3L)]
		[DataRow(FieldType.NumberDouble, nameof(DynamicFieldConfiguration.ValueDouble), 4.0)]
		[DataRow(FieldType.NumberFloat, nameof(DynamicFieldConfiguration.ValueFloat), 5f)]
		[DataRow(FieldType.AutoComplete, nameof(DynamicFieldConfiguration.ValueString), "MegaVolt")]
		[DataRow(FieldType.Text, nameof(DynamicFieldConfiguration.ValueString), "MegaVolt")]
		[DataRow(FieldType.TextMultiline, nameof(DynamicFieldConfiguration.ValueString), "MegaVolt")]
		public void CalculateValidationRulesNotEqualsToDataRowTest(FieldType fieldType, string propertyValueName, object defaultValue)
		{
			// Arrange
			var fieldDefinition = ConfigurationHelper.CreateDefinition(fieldType, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "DarkwingDuck" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "LaunchpadMcQuack" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault }
			};

			typeof(DynamicFieldConfiguration).GetProperty(propertyValueName).SetValue(fieldDefinition.Configurations.Last(), defaultValue);

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinition
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldType)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(1);

			var rule = rules.Single();
			rule.PropertyName.Should().Be(fieldType.ToString());
			rule.JsonName.Should().Be(fieldType.ToString());
			rule.IsDynamicField.Should().BeTrue();

			rule.Rules.First(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("DarkwingDuck");
			rule.Rules.First(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(defaultValue?.ToString());
			rule.Rules.Last(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("LaunchpadMcQuack");
			rule.Rules.Last(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(defaultValue?.ToString());
		}

		[TestMethod]
		public void CalculateValidationRulesNotEqualsToTest()
		{
			// Arrange
			var guidOne = Guid.NewGuid();
			var guidTwo = Guid.NewGuid();
			var dateTime = DateTime.UtcNow;

			var fieldDefinitionDecimal = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null);
			fieldDefinitionDecimal.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "DarkwingDuck" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "LaunchpadMcQuack" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault, ValueDecimal = 5m }
			};

			var fieldDefinitionDateTime = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinitionDateTime.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "DarkwingDuck" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "LaunchpadMcQuack" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault, ValueDateTime = dateTime }
			};

			var fieldDefinitionComboxBoxGuid = ConfigurationHelper.CreateDefinition(FieldType.ComboxBoxGuid, null);
			fieldDefinitionComboxBoxGuid.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "DarkwingDuck" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "LaunchpadMcQuack" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault, ValueGuid = guidOne }
			};

			var fieldDefinitionGuid = ConfigurationHelper.CreateDefinition(FieldType.Guid, null);
			fieldDefinitionGuid.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "DarkwingDuck" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsTo, ValueString = "LaunchpadMcQuack" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault, ValueGuid = guidTwo }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionDecimal,
					fieldDefinitionDateTime,
					fieldDefinitionComboxBoxGuid,
					fieldDefinitionGuid
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.ComboxBoxGuid),
					ConfigurationHelper.CreateAssignment(FieldType.Guid)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.NumberDecimal),
					ConfigurationHelper.CreateField(FieldType.DateTime),
					ConfigurationHelper.CreateField(FieldType.ComboxBoxGuid),
					ConfigurationHelper.CreateField(FieldType.Guid)
				}
			};

			// Act
			var rules = _classUnderTest.CalculateValidationRules(fieldInformation);

			// Assert
			rules.Should().HaveCount(4);

			var ruleDecimal = rules.Single(r => r.PropertyName == nameof(FieldType.NumberDecimal));
			ruleDecimal.JsonName.Should().Be(nameof(FieldType.NumberDecimal));
			ruleDecimal.IsDynamicField.Should().BeTrue();
			ruleDecimal.Rules.First(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("DarkwingDuck");
			ruleDecimal.Rules.First(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(5m.ToString(CultureInfo.InvariantCulture));
			ruleDecimal.Rules.Last(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("LaunchpadMcQuack");
			ruleDecimal.Rules.Last(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(5m.ToString(CultureInfo.InvariantCulture));

			var ruleDateTime = rules.Single(r => r.PropertyName == nameof(FieldType.DateTime));
			ruleDateTime.JsonName.Should().Be(nameof(FieldType.DateTime));
			ruleDateTime.IsDynamicField.Should().BeTrue();
			ruleDateTime.Rules.First(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("DarkwingDuck");
			ruleDateTime.Rules.First(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(dateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
			ruleDateTime.Rules.Last(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("LaunchpadMcQuack");
			ruleDateTime.Rules.Last(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(dateTime.ToString("yyyy-MM-ddTHH:mm:ss"));

			var ruleComboxBoxGuid = rules.Single(r => r.PropertyName == nameof(FieldType.ComboxBoxGuid));
			ruleComboxBoxGuid.JsonName.Should().Be(nameof(FieldType.ComboxBoxGuid));
			ruleComboxBoxGuid.IsDynamicField.Should().BeTrue();
			ruleComboxBoxGuid.Rules.First(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("DarkwingDuck");
			ruleComboxBoxGuid.Rules.First(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(guidOne.ToString());
			ruleComboxBoxGuid.Rules.Last(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("LaunchpadMcQuack");
			ruleComboxBoxGuid.Rules.Last(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(guidOne.ToString());

			var ruleGuid = rules.Single(r => r.PropertyName == nameof(FieldType.Guid));
			ruleGuid.JsonName.Should().Be(nameof(FieldType.Guid));
			ruleGuid.IsDynamicField.Should().BeTrue();
			ruleGuid.Rules.First(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("DarkwingDuck");
			ruleGuid.Rules.First(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(guidTwo.ToString());
			ruleGuid.Rules.Last(r => r.Rule == "NotEqualsTo").PropertyName.Should().Be("LaunchpadMcQuack");
			ruleGuid.Rules.Last(r => r.Rule == "NotEqualsTo").DefaultValue.Should().Be(guidTwo.ToString());
		}
	}
}