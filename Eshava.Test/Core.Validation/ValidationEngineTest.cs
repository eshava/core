using System;
using System.Collections.Generic;
using System.Text;
using Eshava.Core.Validation;
using Eshava.Test.Core.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Core.Validation
{
	[TestClass, TestCategory("Core.Validation")]
	public class ValidationEngineTest
	{
		private ValidationEngine _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new ValidationEngine();
		}

		[TestMethod]
		public void ValidateWithNullInputTest()
		{
			// Act
			var result = _classUnderTest.Validate(null);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationError.Should().Be("model should not be null.");
		}

		[TestMethod]
		public void ValidateWithEmptyClassTest()
		{
			// Arrange
			var source = new Alpha();
			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckRequired->Gamma->ValueIsNull");
			expectedError.AppendLine("CheckEqualsToString->NotEquals=True->Gamma->Epsilon->EqualsOrNotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToString->NotEquals=True->Delta->Epsilon->EqualsOrNotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToString->NotEquals=True->Delta->EpsilonTwo->EqualsOrNotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToString->NotEquals=True->DeltaTwo->EpsilonTwo->NotEqualsStringValue");
			expectedError.AppendLine("CheckRequired->LambdaNullable->ValueIsNull");
			expectedError.AppendLine("CheckRange->Ny->FloatValue");
			expectedError.AppendLine("CheckRange->Omikron->IntegerValue");
			expectedError.AppendLine("CheckEqualsToObject->NotEquals=True->Pi->Rho->EqualsAndNotEqualToDefault");
			expectedError.AppendLine("CheckEqualsToObject->NotEquals=True->Rho->Pi->EqualsAndNotEqualToDefault");
			expectedError.AppendLine("CheckRequired->Sigma->ValueIsNull");
			expectedError.Append("CheckEqualsToObject->NotEquals=True->OmegaIntegerNotEqual->OmegaIntegerEqualTwo->EqualsOrNotEquals");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateAllValidationCriteriaMetTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				MyNullableOne = 0m,
				MyNullableTwo = 2m,
				MyNullableSix = 1m,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationError.Should().BeNull();
		}


		[TestMethod]
		public void ValidateCheckRequiredTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "",
				DeltaTwo = "Alpha",
				Epsilon = "Alpha",
				EpsilonTwo = "Alpha",
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Tau = new Omega(),
				TauIEnumerable = new List<Omega> { new Omega() },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckRequired->Gamma->StringValueIsNullOrEmpty");
			expectedError.AppendLine("CheckRequired->LambdaNullable->ValueIsNull");
			expectedError.AppendLine("CheckRequired->Sigma->ValueIsNull");
			expectedError.AppendLine("CheckRequired->Psi->ValueIsNull");
			expectedError.Append("CheckRequired->Psi->ValueIsNull");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateEqualTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "Quack",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerEqualOne = 1,
				OmegaIntegerEqualTwo = 2,
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckEqualsToString->NotEquals=False->Gamma->Delta->EqualsOrNotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToString->NotEquals=False->Delta->Gamma->EqualsOrNotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToObject->NotEquals=False->OmegaIntegerEqualOne->OmegaIntegerEqualTwo->EqualsOrNotEquals");
			expectedError.Append("CheckEqualsToObject->NotEquals=False->OmegaIntegerEqualTwo->OmegaIntegerEqualOne->EqualsOrNotEquals");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateNotEqualTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				Epsilon = "QuackFu",
				EpsilonTwo = "QuackFu",
				DeltaTwo = "QuackFu",
				LambdaNullable = 1,
				Ny = 1,
				Omikron = Alphabet.A,
				Sigma = new List<int> { 1 }
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckEqualsToString->NotEquals=True->Gamma->Epsilon->EqualsOrNotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToString->NotEquals=True->Delta->Epsilon->EqualsOrNotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToString->NotEquals=True->Delta->EpsilonTwo->EqualsOrNotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToString->NotEquals=True->DeltaTwo->EpsilonTwo->NotEqualsStringValue");
			expectedError.AppendLine("CheckEqualsToObject->NotEquals=True->Pi->Rho->EqualsAndNotEqualToDefault");
			expectedError.AppendLine("CheckEqualsToObject->NotEquals=True->Rho->Pi->EqualsAndNotEqualToDefault");
			expectedError.Append("CheckEqualsToObject->NotEquals=True->OmegaIntegerNotEqual->OmegaIntegerEqualTwo->EqualsOrNotEquals");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateNotEqualDefaultValueTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 7,
				Rho = 7,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationError.Should().BeNull();
		}

		[TestMethod]
		public void ValidateStringMinlengthTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "Duck",
				Delta = "Duck",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				Tau = new Omega { Psi = "Darkwing Duck" },
				TauIEnumerable = new List<Omega> { new Omega { Psi = "Darkwing Duck" } },
				Ypsilon = new List<string> { "Darkwing", "Duck" },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckStringLength->Gamma->LowerMinLength");
			expectedError.AppendLine("CheckStringLength->Delta->LowerMinLength");
			expectedError.Append("CheckStringLength->Ypsilon->LowerMinLength");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateStringMaxlengthTest()
		{
			// Arrange
			var source = new Alpha(7, "Darkwing Duck", "Darkwing Duck")
			{
				Delta = "Darkwing Duck",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				Tau = new Omega { Chi = "Launchpad McQuack in action", Psi = "Darkwing Duck" },
				TauIEnumerable = new List<Omega> { new Omega { Chi = "Launchpad McQuack in action", Psi = "Darkwing Duck" } },
				Ypsilon = new List<string> { "Darkwing", "Launchpad McQuack in action" },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckStringLength->Gamma->GreaterMaxLength");
			expectedError.AppendLine("CheckStringLength->Delta->GreaterMaxLength");
			expectedError.AppendLine("CheckStringLength->Chi->GreaterMaxLength");
			expectedError.AppendLine("CheckStringLength->Chi->GreaterMaxLength");
			expectedError.AppendLine("CheckStringLength->Ypsilon->GreaterMaxLength");
			expectedError.Append("CheckStringLength->Phi->GreaterMaxLength");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateNumberRangeMinValueTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				Lambda = -1,
				LambdaNullable = 1,
				My = -40.5001m,
				Ny = 0.24f,
				Xi = -41,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckRange->Lambda->IntegerValue");
			expectedError.AppendLine("CheckRange->My->DecimalValue");
			expectedError.AppendLine("CheckRange->Ny->FloatValue");
			expectedError.AppendLine("CheckRange->Xi->DoubleValue");
			expectedError.Append("CheckRange->Omikron->IntegerValue");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateNumberRangeMaxValueTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				Lambda = -1,
				LambdaNullable = 1,
				My = 70.5001m,
				Ny = 15.76f,
				Xi = 71,
				Omikron = (Alphabet)4,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckRange->Lambda->IntegerValue");
			expectedError.AppendLine("CheckRange->My->DecimalValue");
			expectedError.AppendLine("CheckRange->Ny->FloatValue");
			expectedError.AppendLine("CheckRange->Xi->DoubleValue");
			expectedError.Append("CheckRange->Omikron->IntegerValue");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateDecimalPlacesTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				My = 1.00001m,
				Ny = 1,
				Xi = 1.1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckDecimalPlaces->My->DecimalValue");
			expectedError.Append("CheckDecimalPlaces->Xi->FloatOrDoubleValue");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateRangeFromAndToTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				MyNullableOne = 1,
				MyNullableTwo = 0,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckRangeValue->MyNullableOne->MyNullableTwo->CheckRangeValueDecimalValue");
			expectedError.Append("CheckRangeValue->MyNullableOne->MyNullableTwo->CheckRangeValueDecimalValue");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateRangeFromAllowNullTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				MyNullableThree = 1,
				MyNullableFour = 0,
				MyNullableFive = null,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationError.Should().BeNull();
		}

		[TestMethod]
		public void ValidateRangeToAllowNullTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				MyNullableThree = 1,
				MyNullableFour = null,
				MyNullableFive = 0,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckRangeValue->MyNullableThree->MyNullableFive->CheckRangeValueDecimalValue");
			expectedError.Append("CheckRangeValue->MyNullableThree->MyNullableFive->CheckRangeValueDecimalValue");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateRangeBetweenLowerValueTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				MyNullableOne = 0,
				MyNullableTwo = 2,
				MyNullableSix = -1,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaFloatFrom = 0,
				OmegaFloat = -1,
				OmegaFloatTo = 1,
				OmegaIntegerFrom = 0,
				OmegaInteger = -1,
				OmegaIntegerTo = 1,
				OmegaDoubleFrom = 0,
				OmegaDouble = -1,
				OmegaDoubleTo = 1,
				OmegaDateTimeFrom  = DateTime.Today,
				OmegaDateTime = DateTime.Today.AddDays(-1),
				OmegaDateTimeTo = DateTime.Today.AddDays(1),
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckRangeBetween->MyNullableSix->MyNullableOne-and-MyNullableTwo->CheckRangeValueDecimalValue");
			expectedError.AppendLine("CheckRangeBetween->OmegaFloat->OmegaFloatFrom-and-OmegaFloatTo->CheckRangeValueFloatValue");
			expectedError.AppendLine("CheckRangeBetween->OmegaInteger->OmegaIntegerFrom-and-OmegaIntegerTo->CheckRangeValueIntegerValue");
			expectedError.AppendLine("CheckRangeBetween->OmegaDouble->OmegaDoubleFrom-and-OmegaDoubleTo->CheckRangeValueDoubleValue");
			expectedError.Append("CheckRangeBetween->OmegaDateTime->OmegaDateTimeFrom-and-OmegaDateTimeTo->CheckRangeValueDateTimeValue");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[TestMethod]
		public void ValidateRangeBetweenGreaterValueTest()
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				MyNullableOne = 0,
				MyNullableTwo = 2,
				MyNullableSix = 3,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaFloatFrom = 0,
				OmegaFloat = 2,
				OmegaFloatTo = 1,
				OmegaIntegerFrom = 0,
				OmegaInteger = 2,
				OmegaIntegerTo = 1,
				OmegaDoubleFrom = 0,
				OmegaDouble = 2,
				OmegaDoubleTo = 1,
				OmegaDateTimeFrom = DateTime.Today,
				OmegaDateTime = DateTime.Today.AddDays(2),
				OmegaDateTimeTo = DateTime.Today.AddDays(1),
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();

			var expectedError = new StringBuilder();
			expectedError.AppendLine("CheckRangeBetween->MyNullableSix->MyNullableOne-and-MyNullableTwo->CheckRangeValueDecimalValue");
			expectedError.AppendLine("CheckRangeBetween->OmegaFloat->OmegaFloatFrom-and-OmegaFloatTo->CheckRangeValueFloatValue");
			expectedError.AppendLine("CheckRangeBetween->OmegaInteger->OmegaIntegerFrom-and-OmegaIntegerTo->CheckRangeValueIntegerValue");
			expectedError.AppendLine("CheckRangeBetween->OmegaDouble->OmegaDoubleFrom-and-OmegaDoubleTo->CheckRangeValueDoubleValue");
			expectedError.Append("CheckRangeBetween->OmegaDateTime->OmegaDateTimeFrom-and-OmegaDateTimeTo->CheckRangeValueDateTimeValue");
			result.ValidationError.Should().Be(expectedError.ToString());
		}

		[DataTestMethod]
		[DataRow("Darkwing Duck", false, "CheckUrl->DeltaUrl->NoWellFormedUri", DisplayName = "Invalid url (1)")]
		[DataRow("http://www.eshava", false, "CheckUrl->DeltaUrl->NoWellFormedUri", DisplayName = "Invalid url (2)")]
		[DataRow("http://www.esh@ava.de", false, "CheckUrl->DeltaUrl->NoWellFormedUri", DisplayName = "Invalid url (2)")]
		[DataRow("http://www.eshava.de/", true, null, DisplayName = "Valid url with schema (http)")]
		[DataRow("https://www.eshava.de/", true, null, DisplayName = "Valid url with schema (https)")]
		[DataRow("https://www.develop.eshava.de/", true, null, DisplayName = "Valid url with schema (https) and sub domain")]
		[DataRow("www.eshava.de/", true, null, DisplayName = "Valid url without schema (www)")]
		[DataRow("eshava.de/", true, null, DisplayName = "Valid url without schema")]
		public void ValidateUrlTest(string url, bool isValid, string error)
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				DeltaUrl = url,
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().Be(isValid);
			result.ValidationError.Should().Be(error);
		}

		[DataTestMethod]
		[DataRow("Darkwing Duck", false, "CheckMailAddress->DeltaMail->NotMatch", DisplayName = "Invalid mail address (1)")]
		[DataRow("Darkwing.Duck", false, "CheckMailAddress->DeltaMail->NotMatch", DisplayName = "Invalid mail address (2)")]
		[DataRow("Darkwing.Duck@", false, "CheckMailAddress->DeltaMail->NotMatch", DisplayName = "Invalid mail address (3)")]
		[DataRow("Darkwing.Duck@eshava", false, "CheckMailAddress->DeltaMail->NotMatch", DisplayName = "Invalid mail address (4)")]
		[DataRow("Darkwing.Duck@eshava.", false, "CheckMailAddress->DeltaMail->NotMatch", DisplayName = "Invalid mail address (5)")]
		[DataRow("Darkwing.Duck@eshava.de", true, null, DisplayName = "Valid mail address")]
		[DataRow("@eshava.de", false, "CheckMailAddress->DeltaMail->NotMatch", DisplayName = "Invalid mail address (6)")]
		public void ValidateMailAddressTest(string mailAddress, bool isValid, string error)
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				DeltaMail = mailAddress,
				EpsilonTwo = "Alpha",
				LambdaNullable = 1,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().Be(isValid);
			result.ValidationError.Should().Be(error);
		}
	}
}