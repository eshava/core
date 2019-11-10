using System;
using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation;
using Eshava.Test.Core.Dynamic.Fields.Validation.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Dynamic.Fields.Validation
{
	[TestClass, TestCategory("Core.Dynamic.Fields.Validation")]
	public class FieldAnalyzerTest
	{
		private FieldAnalyzer<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int> _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new FieldAnalyzer<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>();
		}

		[DataTestMethod]
		[DataRow(FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInteger), typeof(int), 1)]
		[DataRow(FieldType.ComboBoxInteger, nameof(DynamicFieldValue.ValueInteger), typeof(int), 2)]
		[DataRow(FieldType.Text, nameof(DynamicFieldValue.ValueString), typeof(string), "Launchpad McQuack")]
		[DataRow(FieldType.TextMultiline, nameof(DynamicFieldValue.ValueString), typeof(string), "Let's get dangerous")]
		[DataRow(FieldType.AutoComplete, nameof(DynamicFieldValue.ValueString), typeof(string), "Morgana Macawber")]
		[DataRow(FieldType.Checkbox, nameof(DynamicFieldValue.ValueBoolean), typeof(bool), true)]
		public void AnalyseDataTypesDataRowTest(FieldType fieldType, string valuePropertyName, Type type, object value)
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
					ConfigurationHelper.CreateDefinition(fieldType)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(fieldType)
				},
				Values = alpha.FieldValues
			};

			// Act
			var result = _classUnderTest.Analyse(alpha, fieldInformation);

			// Assert
			result.Result.Should().HaveCount(3);

			result.Result[nameof(Alpha.Beta)].Id.Should().Be(nameof(Alpha.Beta));
			result.Result[nameof(Alpha.Beta)].Value.Should().Be(alpha.Beta);
			result.Result[nameof(Alpha.Beta)].Type.Should().Be(typeof(string));

			result.Result[nameof(Alpha.Gamma)].Id.Should().Be(nameof(Alpha.Gamma));
			result.Result[nameof(Alpha.Gamma)].Value.Should().Be(alpha.Gamma);
			result.Result[nameof(Alpha.Gamma)].Type.Should().Be(typeof(string));

			result.Result[fieldType.ToString() + "Assignment"].Id.Should().Be(fieldType.ToString() + "Value");
			result.Result[fieldType.ToString() + "Assignment"].Value.Should().Be(value);
			result.Result[fieldType.ToString() + "Assignment"].Type.Should().Be(type);
		}

		[TestMethod]
		public void AnalyseDataTypesTest()
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
					ConfigurationHelper.CreateDefinition(FieldType.NumberDecimal),
					ConfigurationHelper.CreateDefinition(FieldType.DateTime),
					ConfigurationHelper.CreateDefinition(FieldType.Guid),
					ConfigurationHelper.CreateDefinition(FieldType.ComboxBoxGuid)
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

			// Act
			var result = _classUnderTest.Analyse(alpha, fieldInformation);

			// Assert
			result.Result.Should().HaveCount(6);

			result.Result[nameof(Alpha.Beta)].Id.Should().Be(nameof(Alpha.Beta));
			result.Result[nameof(Alpha.Beta)].Value.Should().Be(alpha.Beta);
			result.Result[nameof(Alpha.Beta)].Type.Should().Be(typeof(string));

			result.Result[nameof(Alpha.Gamma)].Id.Should().Be(nameof(Alpha.Gamma));
			result.Result[nameof(Alpha.Gamma)].Value.Should().Be(alpha.Gamma);
			result.Result[nameof(Alpha.Gamma)].Type.Should().Be(typeof(string));

			result.Result[nameof(FieldType.NumberDecimal) + "Assignment"].Id.Should().Be(nameof(FieldType.NumberDecimal) + "Value");
			result.Result[nameof(FieldType.NumberDecimal) + "Assignment"].Value.Should().Be(3.5m);
			result.Result[nameof(FieldType.NumberDecimal) + "Assignment"].Type.Should().Be(typeof(decimal));

			result.Result[nameof(FieldType.DateTime) + "Assignment"].Id.Should().Be(nameof(FieldType.DateTime) + "Value");
			result.Result[nameof(FieldType.DateTime) + "Assignment"].Value.Should().Be(DateTime.Today);
			result.Result[nameof(FieldType.DateTime) + "Assignment"].Type.Should().Be(typeof(DateTime));

			result.Result[nameof(FieldType.Guid) + "Assignment"].Id.Should().Be(nameof(FieldType.Guid) + "Value");
			result.Result[nameof(FieldType.Guid) + "Assignment"].Value.Should().Be(guidOne);
			result.Result[nameof(FieldType.Guid) + "Assignment"].Type.Should().Be(typeof(Guid));

			result.Result[nameof(FieldType.ComboxBoxGuid) + "Assignment"].Id.Should().Be(nameof(FieldType.ComboxBoxGuid) + "Value");
			result.Result[nameof(FieldType.ComboxBoxGuid) + "Assignment"].Value.Should().Be(guidTwo);
			result.Result[nameof(FieldType.ComboxBoxGuid) + "Assignment"].Type.Should().Be(typeof(Guid));
		}

		[TestMethod]
		public void AnalyseDuplicateAssignmentTest()
		{
			// Arrange
			var alpha = new Alpha
			{
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), "Let's get dangerous")
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.Text)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text),
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = alpha.FieldValues
			};

			// Act
			var result = _classUnderTest.Analyse(alpha, fieldInformation);

			// Assert
			result.Result[nameof(FieldType.Text) + "Assignment"].Id.Should().Be(nameof(FieldType.Text) + "Value");
			result.Result[nameof(FieldType.Text) + "Assignment"].Value.Should().Be("Let's get dangerous");
			result.Result[nameof(FieldType.Text) + "Assignment"].Type.Should().Be(typeof(string));
		}

		[TestMethod]
		public void AnalyseMissingDefinitionTest()
		{
			// Arrange
			var alpha = new Alpha
			{
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), "Let's get dangerous")
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.TextMultiline)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = alpha.FieldValues
			};

			// Act
			var result = _classUnderTest.Analyse(alpha, fieldInformation);

			// Assert
			result.Result.ContainsKey(nameof(FieldType.Text) + "Assignment").Should().BeFalse();
		}

		[TestMethod]
		public void AnalyseMissingFieldValueTest()
		{
			// Arrange
			var alpha = new Alpha
			{
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.TextMultiline, nameof(DynamicFieldValue.ValueString), "Let's get dangerous")
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.Text)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = alpha.FieldValues
			};

			// Act
			var result = _classUnderTest.Analyse(alpha, fieldInformation);

			// Assert
			result.Result.ContainsKey(nameof(FieldType.Text) + "Assignment").Should().BeFalse();
		}

		[TestMethod]
		public void AnalyseInvalidFieldInformationTest()
		{
			// Arrange
			var alpha = new Alpha
			{
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), "Let's get dangerous")
				}
			};

			var fieldInformationNoDefinitions = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>(),
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = alpha.FieldValues
			};
			var fieldInformationNoAssignments = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.Text)
				},
				Assignments = new List<DynamicFieldAssignment>(),
				Values = alpha.FieldValues
			};
			var fieldInformationNoFieldValues = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.Text)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = new List<DynamicFieldValue>()
			};


			// Act
			var resultNoDefinitions = _classUnderTest.Analyse(alpha, fieldInformationNoDefinitions);
			var resultNoAssignments = _classUnderTest.Analyse(alpha, fieldInformationNoAssignments);
			var resultNoFieldValues = _classUnderTest.Analyse(alpha, fieldInformationNoFieldValues);

			// Assert
			resultNoDefinitions.Result.Should().HaveCount(2);
			resultNoAssignments.Result.Should().HaveCount(2);
			resultNoFieldValues.Result.Should().HaveCount(2);
		}

		[TestMethod]
		public void AnalyseModelIsNullTest()
		{
			// Arrange
			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.Text)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), "Let's get dangerous")
				}
			};

			// Act
			var result = _classUnderTest.Analyse(null, fieldInformation);

			// Assert
			result.Result.Should().HaveCount(1);
			result.Result.ContainsKey(nameof(FieldType.Text) + "Assignment").Should().BeTrue();
		}

		[TestMethod]
		public void AnalyseSubClassesTest()
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				Delta = new Omega
				{
					Stigma = 666
				},
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), "Let's get dangerous")
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.Text)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = alpha.FieldValues
			};

			// Act
			var result = _classUnderTest.Analyse(alpha, fieldInformation);

			// Assert
			result.Result.Should().HaveCount(4);
			result.Result.ContainsKey(nameof(FieldType.Text) + "Assignment").Should().BeTrue();

			result.Result[nameof(Omega.Stigma)].Id.Should().Be(nameof(Omega.Stigma));
			result.Result[nameof(Omega.Stigma)].Value.Should().Be(alpha.Delta.Stigma);
			result.Result[nameof(Omega.Stigma)].Type.Should().Be(typeof(int));
		}

		[TestMethod]
		public void AnalyseValidationIgnoreTest()
		{
			// Arrange
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				Epsilon = new Alpha
				{
					Beta = "beta",
					Gamma = "gamme",
					Delta = new Omega
					{
						Stigma = 666
					}
				},
				FieldValues = new List<DynamicFieldValue>
				{
					ConfigurationHelper.CreateField(FieldType.Text, nameof(DynamicFieldValue.ValueString), "Let's get dangerous")
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					ConfigurationHelper.CreateDefinition(FieldType.Text)
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					ConfigurationHelper.CreateAssignment(FieldType.Text)
				},
				Values = alpha.FieldValues
			};

			// Act
			var result = _classUnderTest.Analyse(alpha, fieldInformation);

			// Assert
			result.Result.Should().HaveCount(3);
			result.Result.ContainsKey(nameof(FieldType.Text) + "Assignment").Should().BeTrue();

			result.Result[nameof(Alpha.Beta)].Id.Should().Be(nameof(Alpha.Beta));
			result.Result[nameof(Alpha.Beta)].Value.Should().Be(alpha.Beta);
			result.Result[nameof(Alpha.Beta)].Type.Should().Be(typeof(string));

			result.Result[nameof(Alpha.Gamma)].Id.Should().Be(nameof(Alpha.Gamma));
			result.Result[nameof(Alpha.Gamma)].Value.Should().Be(alpha.Gamma);
			result.Result[nameof(Alpha.Gamma)].Type.Should().Be(typeof(string));
		}
	}
}