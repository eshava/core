using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation;
using Eshava.Core.Validation.Enums;
using Eshava.Test.Core.Dynamic.Fields.Validation.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Dynamic.Fields.Validation
{
	[TestClass, TestCategory("Core.Dynamic.Fields.Validation")]
	public class FieldValidationEngineTest
	{
		private FieldAnalyzer<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int> _fieldAnalyzer;
		private FieldValidationEngine<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int> _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_fieldAnalyzer = new FieldAnalyzer<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>();
			_classUnderTest = new FieldValidationEngine<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>();
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueInteger), 1, true)]
		[DataRow(FieldType.NumberInteger, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueInteger), null, false)]
		[DataRow(FieldType.ComboBoxInteger, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueInteger), 1, true)]
		[DataRow(FieldType.ComboBoxInteger, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueInteger), null, false)]
		[DataRow(FieldType.NumberLong, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueLong), 1L, true)]
		[DataRow(FieldType.NumberLong, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueLong), null, false)]
		[DataRow(FieldType.NumberFloat, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueFloat), 1f, true)]
		[DataRow(FieldType.NumberFloat, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueFloat), null, false)]
		[DataRow(FieldType.NumberDouble, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueDouble), 1.0, true)]
		[DataRow(FieldType.NumberDouble, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueDouble), null, false)]
		[DataRow(FieldType.Text, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), "Darkwing Duck", true)]
		[DataRow(FieldType.Text, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), null, false)]
		[DataRow(FieldType.Text, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), "", false)]
		[DataRow(FieldType.TextMultiline, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), "Darkwing Duck", true)]
		[DataRow(FieldType.TextMultiline, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), null, false)]
		[DataRow(FieldType.TextMultiline, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), "", false)]
		[DataRow(FieldType.AutoComplete, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), "Darkwing Duck", true)]
		[DataRow(FieldType.AutoComplete, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), null, false)]
		[DataRow(FieldType.AutoComplete, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueString), "", false)]
		[DataRow(FieldType.Checkbox, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueBoolean), true, true)]
		[DataRow(FieldType.Checkbox, FieldConfigurationType.Required, nameof(DynamicFieldValue.ValueBoolean), null, false)]
		public void ValidateRequiredDataRowTest(FieldType fieldType, FieldConfigurationType fieldConfigurationType, string valuePropertyName, object value, bool expectedResult)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType, valuePropertyName, value)
				}
			};

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
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().Be(expectedResult);
		}

		[TestMethod]
		public void ValidateRequiredTest()
		{
			// Arrange
			var guidOne = Guid.NewGuid();
			var guidTwo = Guid.NewGuid();

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 3.5m),
					ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today),
					ConfigurationHelper.CreateField(FieldType.Guid, nameof(DynamicFieldValue.ValueGuid), guidOne),
					ConfigurationHelper.CreateField(FieldType.ComboxBoxGuid, nameof(DynamicFieldValue.ValueGuid), guidTwo)
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, FieldConfigurationType.Required),
					ConfigurationHelper.CreateDefinition(FieldType.DateTime, FieldConfigurationType.Required),
					ConfigurationHelper.CreateDefinition(FieldType.Guid, FieldConfigurationType.Required),
					ConfigurationHelper.CreateDefinition(FieldType.ComboxBoxGuid, FieldConfigurationType.Required)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.Guid),
					ConfigurationHelper.CreateAssignment(FieldType.ComboxBoxGuid)
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeTrue();
		}

		[TestMethod]
		public void ValidateRequiredErrorTest()
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.NumberDecimal),
					ConfigurationHelper.CreateField(FieldType.DateTime),
					ConfigurationHelper.CreateField(FieldType.Guid),
					ConfigurationHelper.CreateField(FieldType.ComboxBoxGuid)
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, FieldConfigurationType.Required),
					ConfigurationHelper.CreateDefinition(FieldType.DateTime, FieldConfigurationType.Required),
					ConfigurationHelper.CreateDefinition(FieldType.Guid, FieldConfigurationType.Required),
					ConfigurationHelper.CreateDefinition(FieldType.ComboxBoxGuid, FieldConfigurationType.Required)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.Guid),
					ConfigurationHelper.CreateAssignment(FieldType.ComboxBoxGuid)
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(4);
			result.ValidationErrors.Any(error => error.PropertyName == nameof(FieldType.NumberDecimal) + "Value").Should().BeTrue();
			result.ValidationErrors.Any(error => error.PropertyName == nameof(FieldType.DateTime) + "Value").Should().BeTrue();
			result.ValidationErrors.Any(error => error.PropertyName == nameof(FieldType.Guid) + "Value").Should().BeTrue();
			result.ValidationErrors.Any(error => error.PropertyName == nameof(FieldType.ComboxBoxGuid) + "Value").Should().BeTrue();
		}

		[DataTestMethod]
		[DataRow(FieldType.Text, FieldConfigurationType.Email, 0, "darkwing.duck@hero.io", true)]
		[DataRow(FieldType.Text, FieldConfigurationType.Email, 0, "Darkwing Duck", false)]
		[DataRow(FieldType.Text, FieldConfigurationType.Url, 0, "darkwing.duck.io", true)]
		[DataRow(FieldType.Text, FieldConfigurationType.Url, 0, "Darkwing Duck", false)]
		[DataRow(FieldType.Text, FieldConfigurationType.MinLength, 13, "Darkwing Duck Hero", true)]
		[DataRow(FieldType.Text, FieldConfigurationType.MinLength, 13, "Darkwing Duck", true)]
		[DataRow(FieldType.Text, FieldConfigurationType.MinLength, 13, "Darkwing", false)]
		[DataRow(FieldType.Text, FieldConfigurationType.MaxLength, 8, "Duck", true)]
		[DataRow(FieldType.Text, FieldConfigurationType.MaxLength, 8, "Darkwing", true)]
		[DataRow(FieldType.Text, FieldConfigurationType.MaxLength, 8, "Darkwing Duck", false)]
		public void ValidateStringDataRowTest(FieldType fieldType, FieldConfigurationType fieldConfigurationType, int configurationValue, string value, bool expectedResult)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType, nameof(DynamicFieldValue.ValueString), value)
				}
			};

			var fieldDefinition = ConfigurationHelper.CreateDefinition(fieldType, fieldConfigurationType);
			fieldDefinition.Configurations.First().ValueInteger = configurationValue;

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
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().Be(expectedResult);
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueInteger), 1, true)]
		[DataRow(FieldType.NumberInteger, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueInteger), null, true)]
		[DataRow(FieldType.ComboBoxInteger, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueInteger), 1, true)]
		[DataRow(FieldType.ComboBoxInteger, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueInteger), null, true)]
		[DataRow(FieldType.NumberLong, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueLong), 1L, true)]
		[DataRow(FieldType.NumberLong, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueLong), null, true)]
		[DataRow(FieldType.NumberFloat, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueFloat), 1f, true)]
		[DataRow(FieldType.NumberFloat, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueFloat), 1.25f, true)]
		[DataRow(FieldType.NumberFloat, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueFloat), 1.258f, false)]
		[DataRow(FieldType.NumberFloat, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueFloat), null, true)]
		[DataRow(FieldType.NumberDouble, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueDouble), 1.0, true)]
		[DataRow(FieldType.NumberDouble, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueDouble), 1.25, true)]
		[DataRow(FieldType.NumberDouble, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueDouble), 1.258, false)]
		[DataRow(FieldType.NumberDouble, FieldConfigurationType.DecimalPlaces, nameof(DynamicFieldValue.ValueDouble), null, true)]
		public void ValidateDecimalPlacesDataRowTest(FieldType fieldType, FieldConfigurationType fieldConfigurationType, string valuePropertyName, object value, bool expectedResult)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType, valuePropertyName, value)
				}
			};

			var fieldDefinition = ConfigurationHelper.CreateDefinition(fieldType, fieldConfigurationType);
			fieldDefinition.Configurations.First().ValueInteger = 2;
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
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().Be(expectedResult);
		}

		[TestMethod]
		public void ValidateDecimalPlacesTest()
		{
			// Arrange
			var fieldOne = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 1.25m, "One");
			var fieldTwo = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 1.248m, "Two");
			var fieldThree = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 1.1m, "Three");
			var fieldFour = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), null, "Four");

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { fieldOne, fieldTwo, fieldThree, fieldFour }
			};

			var fieldDefinitionOne = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, FieldConfigurationType.DecimalPlaces, "One");
			fieldDefinitionOne.Configurations.First().ValueInteger = 2;
			var fieldDefinitionTwo = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, FieldConfigurationType.DecimalPlaces, "Two");
			fieldDefinitionTwo.Configurations.First().ValueInteger = 2;
			var fieldDefinitionThree = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, FieldConfigurationType.DecimalPlaces, "Three");
			fieldDefinitionThree.Configurations.First().ValueInteger = 2;
			var fieldDefinitionFour = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, FieldConfigurationType.DecimalPlaces, "Four");
			fieldDefinitionFour.Configurations.First().ValueInteger = 2;

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionOne,
					fieldDefinitionTwo,
					fieldDefinitionThree,
					fieldDefinitionFour
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "One"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "Two"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "Three"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "Four")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.Any(error => error.PropertyName == fieldTwo.Id).Should().BeTrue();
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldConfiguration.ValueInteger), 3, 7, 1, false)]
		[DataRow(FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldConfiguration.ValueInteger), 3, 7, 5, true)]
		[DataRow(FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldConfiguration.ValueInteger), 3, 7, 10, false)]
		[DataRow(FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldConfiguration.ValueInteger), 3, 7, null, true)]
		[DataRow(FieldType.ComboBoxInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldConfiguration.ValueInteger), 3, 7, 1, false)]
		[DataRow(FieldType.ComboBoxInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldConfiguration.ValueInteger), 3, 7, 5, true)]
		[DataRow(FieldType.ComboBoxInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldConfiguration.ValueInteger), 3, 7, 10, false)]
		[DataRow(FieldType.ComboBoxInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldConfiguration.ValueInteger), 3, 7, null, true)]
		[DataRow(FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldConfiguration.ValueLong), 3L, 7L, 1L, false)]
		[DataRow(FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldConfiguration.ValueLong), 3L, 7L, 5L, true)]
		[DataRow(FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldConfiguration.ValueLong), 3L, 7L, 10L, false)]
		[DataRow(FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldConfiguration.ValueLong), 3L, 7L, null, true)]
		[DataRow(FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldConfiguration.ValueFloat), 3f, 7f, 1f, false)]
		[DataRow(FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldConfiguration.ValueFloat), 3f, 7f, 5f, true)]
		[DataRow(FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldConfiguration.ValueFloat), 3f, 7f, 10f, false)]
		[DataRow(FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldConfiguration.ValueFloat), 3f, 7f, null, true)]
		[DataRow(FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldConfiguration.ValueDouble), 3.0, 7.0, 1.0, false)]
		[DataRow(FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldConfiguration.ValueDouble), 3.0, 7.0, 5.0, true)]
		[DataRow(FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldConfiguration.ValueDouble), 3.0, 7.0, 10.0, false)]
		[DataRow(FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldConfiguration.ValueDouble), 3.0, 7.0, null, true)]
		public void ValidateRangeDataRowTest(FieldType fieldType, string valuePropertyName, string configurationValueName, object rangeFrom, object rangeTo, object value, bool expectedResult)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldType, valuePropertyName, value)
				}
			};

			var fieldDefinition = ConfigurationHelper.CreateDefinition(fieldType);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum },
			};

			typeof(DynamicFieldConfiguration).GetProperty(configurationValueName).SetValue(fieldDefinition.Configurations.First(), rangeFrom);
			typeof(DynamicFieldConfiguration).GetProperty(configurationValueName).SetValue(fieldDefinition.Configurations.Last(), rangeTo);


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
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().Be(expectedResult);
		}

		[TestMethod]
		public void ValidateRangeDecimalTest()
		{
			// Arrange
			var fieldOne = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 1m, "One");
			var fieldTwo = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 5m, "Two");
			var fieldThree = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 10m, "Three");
			var fieldFour = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), null, "Four");

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { fieldOne, fieldTwo, fieldThree, fieldFour }
			};

			var fieldDefinitionOne = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "One");
			fieldDefinitionOne.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum, ValueDecimal = 3m },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum, ValueDecimal = 7m }
			};
			
			var fieldDefinitionTwo = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "Two");
			fieldDefinitionTwo.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum, ValueDecimal = 3m },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum, ValueDecimal = 7m }
			};
			
			var fieldDefinitionThree = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "Three");
			fieldDefinitionThree.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum, ValueDecimal = 3m },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum, ValueDecimal = 7m }
			};
			
			var fieldDefinitionFour = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "Four");
			fieldDefinitionFour.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum, ValueDecimal = 3m },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum, ValueDecimal = 7m }
			};
			
			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionOne,
					fieldDefinitionTwo,
					fieldDefinitionThree,
					fieldDefinitionFour
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "One"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "Two"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "Three"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "Four")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(2);
			result.ValidationErrors.Any(error => error.PropertyName == fieldOne.Id).Should().BeTrue();
			result.ValidationErrors.Any(error => error.PropertyName == fieldThree.Id).Should().BeTrue();
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, FieldType.ComboBoxInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 2, 2, true)]
		[DataRow(FieldType.NumberInteger, FieldType.ComboBoxInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 1, 5, true)]
		[DataRow(FieldType.NumberInteger, FieldType.ComboBoxInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 5, 1, false)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberLong, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueLong), 1, 5L, false)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberLong, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueLong), 1, 5L, false)]

		public void ValidateRangeDataRowTest(FieldType fieldTypeFrom, FieldType fieldTypeTo, string valuePropertyNameFrom, string valuePropertyNameTo, object valueFrom, object valueTo, bool expectedResult)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				Delta = new Omega
				{
					Stigma = 10
				},
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldTypeFrom, valuePropertyNameFrom, valueFrom),
					ConfigurationHelper.CreateField(fieldTypeTo, valuePropertyNameTo, valueTo)
				}
			};

			var fieldDefinitionOne = ConfigurationHelper.CreateDefinition(fieldTypeFrom, FieldConfigurationType.RangeTo);
			fieldDefinitionOne.Configurations.First().ValueString = alpha.FieldValues.Last().Id;

			var fieldDefinitionTwo = ConfigurationHelper.CreateDefinition(fieldTypeTo, FieldConfigurationType.RangeFrom);
			fieldDefinitionTwo.Configurations.First().ValueString = alpha.FieldValues.First().Id;


			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionOne,
					fieldDefinitionTwo
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldTypeFrom),
					ConfigurationHelper.CreateAssignment(fieldTypeTo)
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().Be(expectedResult);
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, nameof(Omega.Stigma), nameof(DynamicFieldValue.ValueInteger), 2, 2, true)]
		[DataRow(FieldType.NumberInteger, nameof(Omega.Stigma), nameof(DynamicFieldValue.ValueInteger), 2, 5, true)]
		[DataRow(FieldType.NumberInteger, nameof(Omega.Stigma), nameof(DynamicFieldValue.ValueInteger), 5, 1, false)]
		[DataRow(FieldType.NumberInteger, nameof(Alpha.Delta), nameof(DynamicFieldValue.ValueInteger), 2, 5, false)]
		[DataRow(FieldType.NumberLong, nameof(Omega.Stigma), nameof(DynamicFieldValue.ValueLong), 2L, 5, false)]
		public void ValidateRangeWithHardCodedFieldDataRowTest(FieldType fieldTypeFrom, string fieldNameTo, string valuePropertyNameFrom, object valueFrom, int valueTo, bool expectedResult)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				Delta = new Omega
				{
					Stigma = valueTo
				},
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldTypeFrom, valuePropertyNameFrom, valueFrom)
				}
			};

			var fieldDefinitionOne = ConfigurationHelper.CreateDefinition(fieldTypeFrom, FieldConfigurationType.RangeTo);
			fieldDefinitionOne.Configurations.First().ValueString = fieldNameTo;


			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionOne
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldTypeFrom)
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().Be(expectedResult);
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 2, 2, 2, false, true)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 3, 1, 5, false, true)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 1, 3, 5, false, false)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), null, 3, 5, false, true)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 3, null, 5, false, false)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 3, 1, null, false, false)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), null, 3, 5, true, true)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 3, null, 5, true, true)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 3, 1, null, true, true)]
		[DataRow(FieldType.NumberInteger, FieldType.NumberInteger, FieldType.NumberLong, nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueLong), 3, 1, 5L, false, false)]
		[DataRow(FieldType.NumberLong, FieldType.NumberInteger, FieldType.NumberInteger, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueInteger), nameof(DynamicFieldValue.ValueInteger), 3L, 1, 5, false, false)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), 2L, 2L, 2L, false, true)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), 3L, 1L, 5L, false, true)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), 1L, 3L, 5L, false, false)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), null, 3L, 5L, false, true)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), 3L, null, 5L, false, false)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), 3L, 1L, null, false, false)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), null, 3L, 5L, true, true)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), 3L, null, 5L, true, true)]
		[DataRow(FieldType.NumberLong, FieldType.NumberLong, FieldType.NumberLong, nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), nameof(DynamicFieldValue.ValueLong), 3L, 1L, null, true, true)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), 2f, 2f, 2f, false, true)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), 3f, 1f, 5f, false, true)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), 1f, 3f, 5f, false, false)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), null, 3f, 5f, false, true)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), 3f, null, 5f, false, false)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), 3f, 1f, null, false, false)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), null, 3f, 5f, true, true)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), 3f, null, 5f, true, true)]
		[DataRow(FieldType.NumberFloat, FieldType.NumberFloat, FieldType.NumberFloat, nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), nameof(DynamicFieldValue.ValueFloat), 3f, 1f, null, true, true)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), 2.0, 2.0, 2.0, false, true)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), 3.0, 1.0, 5.0, false, true)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), 1.0, 3.0, 5.0, false, false)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), null, 3.0, 5.0, false, true)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), 3.0, null, 5.0, false, false)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), 3.0, 1.0, null, false, false)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), null, 3.0, 5.0, true, true)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), 3.0, null, 5.0, true, true)]
		[DataRow(FieldType.NumberDouble, FieldType.NumberDouble, FieldType.NumberDouble, nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), nameof(DynamicFieldValue.ValueDouble), 3.0, 1.0, null, true, true)]
		public void ValidateRangeBetweenDataRowTest(FieldType field, FieldType fieldTypeFrom, FieldType fieldTypeTo, string valuePropertyName, string valuePropertyNameFrom, string valuePropertyNameTo, object value, object valueFrom, object valueTo, bool allowNull, bool expectedResult)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(fieldTypeFrom, valuePropertyNameFrom, valueFrom, "One"),
					ConfigurationHelper.CreateField(fieldTypeTo, valuePropertyNameTo, valueTo, "Two"),
					ConfigurationHelper.CreateField(field, valuePropertyName, value, "Three")
				}
			};

			var fieldDefinitionThree = ConfigurationHelper.CreateDefinition(field, null, "Three");
			var configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = fieldTypeFrom.ToString() + "ValueOne" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = fieldTypeTo.ToString() + "ValueTwo" },
			};

			if (allowNull)
			{
				configurations.Add(new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull });
			}

			fieldDefinitionThree.Configurations = configurations;

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(fieldTypeFrom, null, "One"),
					ConfigurationHelper.CreateDefinition(fieldTypeTo, null, "Two"),
					fieldDefinitionThree
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldTypeFrom, "One"),
					ConfigurationHelper.CreateAssignment(fieldTypeTo, "Two"),
					ConfigurationHelper.CreateAssignment(field, "Three")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().Be(expectedResult);
		}

		[TestMethod]
		public void ValidateRangeBetweenDecimalLowerValueTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 3m, "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 7m, "To");
			var field = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 1m);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo}
			};
			
			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "To");
			

			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.NumberDecimal) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.NumberDecimal) + "ValueTo" }
			};
						
			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.Single().ErrorType.Should().Be(ValidationErrorType.DataTypeDecimal.ToString());
			result.ValidationErrors.Single().MethodType.Should().Be(ValidationMethodType.RangeBetween.ToString());
		}

		[TestMethod]
		public void ValidateRangeBetweenDecimalGreaterValueTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 3m, "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 7m, "To");
			var field = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 10m);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.NumberDecimal) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.NumberDecimal) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.Single().ErrorType.Should().Be(ValidationErrorType.DataTypeDecimal.ToString());
			result.ValidationErrors.Single().MethodType.Should().Be(ValidationMethodType.RangeBetween.ToString());
		}

		[TestMethod]
		public void ValidateRangeBetweenDecimalTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 3m, "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 7m, "To");
			var field = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 5m);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.NumberDecimal) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.NumberDecimal) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
		}

		[TestMethod]
		public void ValidateRangeBetweenDecimalAllowNullLeftTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), null, "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 7m, "To");
			var field = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 5m);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.NumberDecimal) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.NumberDecimal) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
		}

		[TestMethod]
		public void ValidateRangeBetweenDecimalAllowNullRightTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 3m, "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), null, "To");
			var field = ConfigurationHelper.CreateField(FieldType.NumberDecimal, nameof(DynamicFieldValue.ValueDecimal), 5m);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.NumberDecimal) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.NumberDecimal) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.NumberDecimal, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
		}

		[TestMethod]
		public void ValidateRangeBetweenDateTimeLowerValueTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today, "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(2.0), "To");
			var field = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(-2.0));

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.DateTime) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.DateTime) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.Single().ErrorType.Should().Be(ValidationErrorType.DataTypeDateTime.ToString());
			result.ValidationErrors.Single().MethodType.Should().Be(ValidationMethodType.RangeBetween.ToString());
		}

		[TestMethod]
		public void ValidateRangeBetweenDateTimeGreaterValueTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(-2.0), "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today, "To");
			var field = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(2.0));

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.DateTime) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.DateTime) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.Single().ErrorType.Should().Be(ValidationErrorType.DataTypeDateTime.ToString());
			result.ValidationErrors.Single().MethodType.Should().Be(ValidationMethodType.RangeBetween.ToString());
		}

		[TestMethod]
		public void ValidateRangeBetweenDateTimeTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(-2.0), "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(2.0), "To");
			var field = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.DateTime) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.DateTime) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
		}

		[TestMethod]
		public void ValidateRangeBetweenDateTimeAllowNullLeftTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), null, "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(2.0), "To");
			var field = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.DateTime) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.DateTime) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
		}

		[TestMethod]
		public void ValidateRangeBetweenDateTimeAllowNullRightTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today, "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), null, "To");
			var field = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(2.0));

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.DateTime) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.DateTime) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
		}

		[TestMethod]
		public void ValidateRangeBetweenDateTimePropertyNotFoundLeftTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(-2.0), "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(2.0), "To");
			var field = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.DateTime) + "LaunchpadMcQuack" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.DateTime) + "ValueTo" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.Single().MethodType.Should().Be(ValidationMethodType.RangeBetween.ToString());
			result.ValidationErrors.Single().ErrorType.Should().Be(ValidationErrorType.PropertyNotFoundFrom.ToString());
			result.ValidationErrors.Single().PropertyNameFrom.Should().Be(nameof(FieldType.DateTime) + "LaunchpadMcQuack");
			result.ValidationErrors.Single().PropertyNameTo.Should().Be(nameof(FieldType.DateTime) + "ValueTo");
			result.ValidationErrors.Single().PropertyName.Should().Be(nameof(FieldType.DateTime) + "Value");
		}

		[TestMethod]
		public void ValidateRangeBetweenDateTimePropertyNotFoundRightTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(-2.0), "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(2.0), "To");
			var field = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.DateTime) + "ValueFrom" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.DateTime) + "LaunchpadMcQuack" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(1);
			result.ValidationErrors.Single().MethodType.Should().Be(ValidationMethodType.RangeBetween.ToString());
			result.ValidationErrors.Single().ErrorType.Should().Be(ValidationErrorType.PropertyNotFoundTo.ToString());
			result.ValidationErrors.Single().PropertyNameFrom.Should().Be(nameof(FieldType.DateTime) + "ValueFrom");
			result.ValidationErrors.Single().PropertyNameTo.Should().Be(nameof(FieldType.DateTime) + "LaunchpadMcQuack");
			result.ValidationErrors.Single().PropertyName.Should().Be(nameof(FieldType.DateTime) + "Value");
		}

		[TestMethod]
		public void ValidateRangeBetweenDateTimePropertyNotFoundBothTest()
		{
			// Arrange
			var fieldFrom = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(-2.0), "From");
			var fieldTo = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today.AddDays(2.0), "To");
			var field = ConfigurationHelper.CreateField(FieldType.DateTime, nameof(DynamicFieldValue.ValueDateTime), DateTime.Today);

			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field, fieldFrom, fieldTo }
			};

			var fieldDefinitionFrom = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "From");
			var fieldDefinitionTo = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null, "To");


			var fieldDefinition = ConfigurationHelper.CreateDefinition(FieldType.DateTime, null);
			fieldDefinition.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetween },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.AllowNull },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenFrom, ValueString = nameof(FieldType.DateTime) + "MorganaMacawber" },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.RangeBetweenTo, ValueString = nameof(FieldType.DateTime) + "LaunchpadMcQuack" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionFrom,
					fieldDefinitionTo,
					fieldDefinition,
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.DateTime),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "From"),
					ConfigurationHelper.CreateAssignment(FieldType.DateTime, "To")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(2);
			result.ValidationErrors.First().MethodType.Should().Be(ValidationMethodType.RangeBetween.ToString());
			result.ValidationErrors.First().ErrorType.Should().Be(ValidationErrorType.PropertyNotFoundFrom.ToString());
			result.ValidationErrors.First().PropertyNameFrom.Should().Be(nameof(FieldType.DateTime) + "MorganaMacawber");
			result.ValidationErrors.First().PropertyNameTo.Should().Be(nameof(FieldType.DateTime) + "LaunchpadMcQuack");
			result.ValidationErrors.First().PropertyName.Should().Be(nameof(FieldType.DateTime) + "Value");
			result.ValidationErrors.Last().MethodType.Should().Be(ValidationMethodType.RangeBetween.ToString());
			result.ValidationErrors.Last().ErrorType.Should().Be(ValidationErrorType.PropertyNotFoundTo.ToString());
			result.ValidationErrors.Last().PropertyNameFrom.Should().Be(nameof(FieldType.DateTime) + "MorganaMacawber");
			result.ValidationErrors.Last().PropertyNameTo.Should().Be(nameof(FieldType.DateTime) + "LaunchpadMcQuack");
			result.ValidationErrors.Last().PropertyName.Should().Be(nameof(FieldType.DateTime) + "Value");
		}
		
		[DataTestMethod]
		[DataRow("Darkwing Duck", "Darkwing Duck", "Darkwing Duck", "", true, true)]
		[DataRow("Launchpad McQuack", "Darkwing Duck", "Darkwing Duck", "", true, false)]
		[DataRow("Darkwing Duck", "Launchpad McQuack", "Darkwing Duck", "", true, false)]
		[DataRow("Darkwing Duck", "Darkwing Duck", "Launchpad McQuack", "", true, false)]
		[DataRow("Darkwing Duck", "Darkwing Duck", "Darkwing Duck", "Darkwing Duck", false, true)]
		[DataRow("Darkwing Duck", "Darkwing Duck", "Darkwing Duck", "Launchpad McQuack", false, false)]
		[DataRow("Launchpad McQuack", "Darkwing Duck", "Darkwing Duck", "Launchpad McQuack", false, false)]
		[DataRow("Darkwing Duck", "Launchpad McQuack", "Darkwing Duck", "Launchpad McQuack", false, false)]
		[DataRow("Darkwing Duck", "Darkwing Duck", "Launchpad McQuack", "Launchpad McQuack", false, false)]
		[DataRow("Launchpad McQuack", "Launchpad McQuack", "Darkwing Duck", "Launchpad McQuack", false, true)]
		[DataRow("Launchpad McQuack", "Darkwing Duck", "Launchpad McQuack",  "Launchpad McQuack", false, true)]
		[DataRow("Darkwing Duck", "Launchpad McQuack", "Launchpad McQuack",  "Launchpad McQuack", false, true)]
		[DataRow("Darkwing Duck Hero", "Darkwing Duck Super Hero", "Darkwing Duck", "Launchpad McQuack", false, true)]
		public void ValidateEqualDataRowTest(string propertyValueOne, string propertyValueTwo, string propertyValueThree, string defaultValue, bool equal, bool expectedResult)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = propertyValueThree,
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), propertyValueOne, "One"),
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), propertyValueTwo, "Two")
				}
			};

			var configurationType = equal ? FieldConfigurationType.EqualsTo : FieldConfigurationType.NotEqualsTo;

			var fieldDefinitionOne = ConfigurationHelper.CreateDefinition(FieldType.Text, null, "One");
			fieldDefinitionOne.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = configurationType, ValueString = nameof(FieldType.Text) + "ValueTwo" },
				new DynamicFieldConfiguration { ConfigurationType = configurationType, ValueString = nameof(Alpha.Beta) },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault, ValueString = defaultValue }
			};

			var fieldDefinitionTwo = ConfigurationHelper.CreateDefinition(FieldType.Text, null, "Two");
			fieldDefinitionTwo.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = configurationType, ValueString = nameof(FieldType.Text) + "ValueOne" },
				new DynamicFieldConfiguration { ConfigurationType = configurationType, ValueString = nameof(Alpha.Beta) },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault, ValueString = defaultValue }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionOne,
					fieldDefinitionTwo
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text, "One"),
					ConfigurationHelper.CreateAssignment(FieldType.Text, "Two")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().Be(expectedResult);
		}

		[DataTestMethod]
		[DataRow("ValueOne", "ValueThree", true, 1)]
		[DataRow("ValueThree", "ValueTwo", true, 1)]
		[DataRow("ValueThree", "ValueFour", true, 2)]
		[DataRow("ValueOne", "ValueThree", false, 1)]
		[DataRow("ValueThree", "ValueTwo", false, 1)]
		[DataRow("ValueThree", "ValueFour", false, 2)]

		public void ValidateEqualDataRowPropertyNotFoundTest(string propertyNameOne, string propertyNameTwo, bool equal, int errorCount)
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Launchpad McQuack",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), "Darkwing Duck", "One"),
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), "Darkwing Duck", "Two")
				}
			};

			var configurationType = equal ? FieldConfigurationType.EqualsTo : FieldConfigurationType.NotEqualsTo;

			var fieldDefinitionOne = ConfigurationHelper.CreateDefinition(FieldType.Text, null, "One");
			fieldDefinitionOne.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = configurationType, ValueString = nameof(FieldType.Text) + propertyNameTwo },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault, ValueString = "Darkwing Duck" }
			};

			var fieldDefinitionTwo = ConfigurationHelper.CreateDefinition(FieldType.Text, null, "Two");
			fieldDefinitionTwo.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = configurationType, ValueString = nameof(FieldType.Text) + propertyNameOne },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.NotEqualsDefault, ValueString = "Darkwing Duck" }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					fieldDefinitionOne,
					fieldDefinitionTwo
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text, "One"),
					ConfigurationHelper.CreateAssignment(FieldType.Text, "Two")
				},
				Values = alpha.FieldValues
			};

			var analysisResult = _fieldAnalyzer.Analyse(alpha, fieldInformation);

			// Act
			var result = _classUnderTest.Validate(fieldInformation, analysisResult);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(errorCount);
			result.ValidationErrors.All(error => error.MethodType == (equal ?ValidationMethodType.Equals : ValidationMethodType.NotEquals).ToString()).Should().BeTrue();
			result.ValidationErrors.All(error => error.ErrorType == ValidationErrorType.PropertyNotFoundTo.ToString()).Should().BeTrue();
		}
	}
}