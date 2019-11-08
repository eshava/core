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
		[DataRow(FieldType.NumberInteger, nameof(DynamicFieldValue.ValueInt), typeof(int), 1)]
		[DataRow(FieldType.ComboBoxInt, nameof(DynamicFieldValue.ValueInt), typeof(int), 2)]
		[DataRow(FieldType.Text, nameof(DynamicFieldValue.ValueString), typeof(string), "Launchpad McQuack")]
		[DataRow(FieldType.TextMultiline, nameof(DynamicFieldValue.ValueString), typeof(string), "Let's get dangerous")]
		[DataRow(FieldType.AutoComplete, nameof(DynamicFieldValue.ValueString), typeof(string), "Morgana Macawber")]
		[DataRow(FieldType.Checkbox, nameof(DynamicFieldValue.ValueBool), typeof(bool), true)]
		public void AnalyseDataTypesDataRowTest(FieldType fieldType, string valuePropertyName, Type type, object value)
		{
			// Arrange
			var field = new DynamicFieldValue { Id = fieldType.ToString() + "Value", AssignmentId = fieldType.ToString() + "Assignment" };
			typeof(DynamicFieldValue).GetProperty(valuePropertyName).SetValue(field, value);
			var alpha = new Alpha
			{
				Beta = "Darkwing Duck",
				Gamma = "MegaVolt",
				FieldValues = new List<DynamicFieldValue> { field }
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = fieldType, Id = fieldType.ToString() },
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = fieldType.ToString() + "Assignment", DefinitionId = fieldType.ToString() },
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
					new DynamicFieldValue { Id = nameof(FieldType.NumberDecimal) + "Value", AssignmentId = nameof(FieldType.NumberDecimal) + "Assignment", ValueDecimals = 3.5m },
					new DynamicFieldValue { Id = nameof(FieldType.DateTime) + "Value", AssignmentId = nameof(FieldType.DateTime) + "Assignment", ValueDateTime = DateTime.Today },
					new DynamicFieldValue { Id = nameof(FieldType.Guid) + "Value", AssignmentId = nameof(FieldType.Guid) + "Assignment", ValueGuid = guidOne },
					new DynamicFieldValue { Id = nameof(FieldType.ComboxBoxGuid) + "Value", AssignmentId = nameof(FieldType.ComboxBoxGuid) + "Assignment", ValueGuid = guidTwo }
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = FieldType.NumberDecimal, Id = nameof(FieldType.NumberDecimal) },
					new DynamicFieldDefinition { FieldType = FieldType.DateTime, Id = nameof(FieldType.DateTime) },
					new DynamicFieldDefinition { FieldType = FieldType.Guid, Id = nameof(FieldType.Guid) },
					new DynamicFieldDefinition { FieldType = FieldType.ComboxBoxGuid, Id = nameof(FieldType.ComboxBoxGuid) }
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.NumberDecimal) + "Assignment", DefinitionId = nameof(FieldType.NumberDecimal) },
					new DynamicFieldAssignment { Id = nameof(FieldType.DateTime) + "Assignment", DefinitionId = nameof(FieldType.DateTime) },
					new DynamicFieldAssignment { Id = nameof(FieldType.Guid) + "Assignment", DefinitionId = nameof(FieldType.Guid) },
					new DynamicFieldAssignment { Id = nameof(FieldType.ComboxBoxGuid) + "Assignment", DefinitionId = nameof(FieldType.ComboxBoxGuid) }
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
					new DynamicFieldValue { Id = nameof(FieldType.Text) + "Value", AssignmentId = nameof(FieldType.Text) + "Assignment", ValueString = "Let's get dangerous" }
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = FieldType.Text, Id = nameof(FieldType.Text) }
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) },
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) }
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
					new DynamicFieldValue { Id = nameof(FieldType.Text) + "Value", AssignmentId = nameof(FieldType.Text) + "Assignment", ValueString = "Let's get dangerous" }
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = FieldType.TextMultiline, Id = nameof(FieldType.TextMultiline) }
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) }
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
					new DynamicFieldValue { Id = nameof(FieldType.TextMultiline) + "Value", AssignmentId = nameof(FieldType.TextMultiline) + "Assignment", ValueString = "Let's get dangerous" }
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = FieldType.Text, Id = nameof(FieldType.Text) }
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) }
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
					new DynamicFieldValue { Id = nameof(FieldType.Text) + "Value", AssignmentId = nameof(FieldType.Text) + "Assignment", ValueString = "Let's get dangerous" }
				}
			};

			var fieldInformationNoDefinitions = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>(),
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) }
				},
				Values = alpha.FieldValues
			};
			var fieldInformationNoAssignments = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = FieldType.Text, Id = nameof(FieldType.Text) }
				},
				Assignments = new List<DynamicFieldAssignment>(),
				Values = alpha.FieldValues
			};
			var fieldInformationNoFieldValues = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = FieldType.Text, Id = nameof(FieldType.Text) }
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) }
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
					new DynamicFieldDefinition { FieldType = FieldType.Text, Id = nameof(FieldType.Text) }
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) }
				},
				Values = new List<DynamicFieldValue>
				{
					new DynamicFieldValue { Id = nameof(FieldType.Text) + "Value", AssignmentId = nameof(FieldType.Text) + "Assignment", ValueString = "Let's get dangerous" }
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
					Stigma = "Darkwing Duck"
				},
				FieldValues = new List<DynamicFieldValue>
				{
					new DynamicFieldValue { Id = nameof(FieldType.Text) + "Value", AssignmentId = nameof(FieldType.Text) + "Assignment", ValueString = "Let's get dangerous" }
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = FieldType.Text, Id = nameof(FieldType.Text) }
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) }
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
			result.Result[nameof(Omega.Stigma)].Type.Should().Be(typeof(string));
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
						Stigma = "stigma"
					}
				},
				FieldValues = new List<DynamicFieldValue>
				{
					new DynamicFieldValue { Id = nameof(FieldType.Text) + "Value", AssignmentId = nameof(FieldType.Text) + "Assignment", ValueString = "Let's get dangerous" }
				}
			};

			var fieldInformation = new FieldInformation<DynamicFieldDefinition, DynamicFieldAssignment, DynamicFieldValue, string, int>
			{
				Definitions = new List<DynamicFieldDefinition>
				{
					new DynamicFieldDefinition { FieldType = FieldType.Text, Id = nameof(FieldType.Text) }
				},
				Assignments = new List<DynamicFieldAssignment>
				{
					new DynamicFieldAssignment { Id = nameof(FieldType.Text) + "Assignment", DefinitionId = nameof(FieldType.Text) }
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