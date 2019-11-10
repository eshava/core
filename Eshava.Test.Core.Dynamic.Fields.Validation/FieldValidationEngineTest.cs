using System;
using System.Collections.Generic;
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
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum },
			};
			fieldDefinitionOne.Configurations.First().ValueDecimal = 3m;
			fieldDefinitionOne.Configurations.Last().ValueDecimal = 7m;


			var fieldDefinitionTwo = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "Two");
			fieldDefinitionTwo.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum },
			};
			fieldDefinitionTwo.Configurations.First().ValueDecimal = 3m;
			fieldDefinitionTwo.Configurations.Last().ValueDecimal = 7m;



			var fieldDefinitionThree = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "Three");
			fieldDefinitionThree.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum },
			};
			fieldDefinitionThree.Configurations.First().ValueDecimal = 3m;
			fieldDefinitionThree.Configurations.Last().ValueDecimal = 7m;


			var fieldDefinitionFour = ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal, null, "Four");
			fieldDefinitionFour.Configurations = new List<DynamicFieldConfiguration>
			{
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Minimum },
				new DynamicFieldConfiguration { ConfigurationType = FieldConfigurationType.Maximum },
			};
			fieldDefinitionFour.Configurations.First().ValueDecimal = 3m;
			fieldDefinitionFour.Configurations.Last().ValueDecimal = 7m;

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
		public void ValidateRangeDataRowTest(FieldType field, FieldType fieldTypeFrom, FieldType fieldTypeTo, string valuePropertyName, string valuePropertyNameFrom, string valuePropertyNameTo, object value, object valueFrom, object valueTo, bool allowNull, bool expectedResult)
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
	}
}