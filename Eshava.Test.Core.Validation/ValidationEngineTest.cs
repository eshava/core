using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Validation;
using Eshava.Core.Validation.Enums;
using Eshava.Test.Core.Validation.Models;
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
			result.ValidationErrors.First().MethodType.Should().Be(ValidationMethodType.Input.ToString());
			result.ValidationErrors.First().ErrorType.Should().Be(ValidationErrorType.IsNull.ToString());
		}

		[TestMethod]
		public void ValidateWithEmptyClassTest()
		{
			// Arrange
			var source = new Alpha
			{
				PsiRequiredNullable = DateTime.MinValue,
				ChiNullable = Guid.Empty,
				EpsilonThree = ""
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(21);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.Gamma))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsEmpty.ToString()
				&& error.PropertyName == nameof(Alpha.ChiNotNull))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsEmpty.ToString()
				&& error.PropertyName == nameof(Alpha.ChiNullable))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsEmpty.ToString()
				&& error.PropertyName == nameof(Alpha.PsiRequired))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsEmpty.ToString()
				&& error.PropertyName == nameof(Alpha.PsiRequiredNullable))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.LambdaNullable))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.LambdaLongNullable))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.Sigma))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsString.ToString()
				&& error.PropertyName == nameof(Alpha.Gamma)
				&& error.PropertyNameTo == nameof(Alpha.Epsilon))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsString.ToString()
				&& error.PropertyName == nameof(Alpha.Delta)
				&& error.PropertyNameTo == nameof(Alpha.Epsilon))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsString.ToString()
				&& error.PropertyName == nameof(Alpha.Delta)
				&& error.PropertyNameTo == nameof(Alpha.EpsilonTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsAndNotEqualToDefaultString.ToString()
				&& error.PropertyName == nameof(Alpha.DeltaTwo)
				&& error.PropertyNameTo == nameof(Alpha.EpsilonTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeFloat.ToString()
				&& error.PropertyName == nameof(Alpha.Ny))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeInteger.ToString()
				&& error.PropertyName == nameof(Alpha.Omikron))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Enumeration.ToString()
				&& error.ErrorType == ValidationErrorType.Invalid.ToString()
				&& error.PropertyName == nameof(Alpha.Omikron)
				&& error.Value == "0")
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsAndNotEqualToDefault.ToString()
				&& error.PropertyName == nameof(Alpha.Pi)
				&& error.PropertyNameTo == nameof(Alpha.Rho))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsAndNotEqualToDefault.ToString()
				&& error.PropertyName == nameof(Alpha.Rho)
				&& error.PropertyNameTo == nameof(Alpha.Pi))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.Equals.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaIntegerNotEqual)
				&& error.PropertyNameTo == nameof(Alpha.OmegaIntegerEqualTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.Equals.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaLongNotEqual)
				&& error.PropertyNameTo == nameof(Alpha.OmegaLongEqualTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.EpsilonFour))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsEmpty.ToString()
				&& error.PropertyName == nameof(Alpha.EpsilonThree))
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				MyNullableOne = 0m,
				MyNullableTwo = 2m,
				MyNullableSix = 1m,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Tau = new Omega(),
				TauIEnumerable = new List<Omega> { new Omega() },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(10);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsEmpty.ToString()
				&& error.PropertyName == nameof(Alpha.Gamma))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsEmpty.ToString()
				&& error.PropertyName == nameof(Alpha.PsiRequired))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.PsiRequiredNullable))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.LambdaNullable))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.LambdaLongNullable))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.Sigma))
			.Should().BeTrue();

			result.ValidationErrors.Count(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.Psi))
			.Should().Be(2);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsEmpty.ToString()
				&& error.PropertyName == nameof(Alpha.ChiNotNull))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Required.ToString()
				&& error.ErrorType == ValidationErrorType.IsNull.ToString()
				&& error.PropertyName == nameof(Alpha.ChiNullable))
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerEqualOne = 1,
				OmegaIntegerEqualTwo = 2,
				OmegaIntegerNotEqual = 1,
				OmegaLongEqualOne = 1L,
				OmegaLongEqualTwo = 2L,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(6);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Equals.ToString()
				&& error.ErrorType == ValidationErrorType.NotEqualsString.ToString()
				&& error.PropertyName == nameof(Alpha.Gamma)
				&& error.PropertyNameTo == nameof(Alpha.Delta))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Equals.ToString()
				&& error.ErrorType == ValidationErrorType.NotEqualsString.ToString()
				&& error.PropertyName == nameof(Alpha.Delta)
				&& error.PropertyNameTo == nameof(Alpha.Gamma))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Equals.ToString()
				&& error.ErrorType == ValidationErrorType.NotEquals.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaIntegerEqualOne)
				&& error.PropertyNameTo == nameof(Alpha.OmegaIntegerEqualTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Equals.ToString()
				&& error.ErrorType == ValidationErrorType.NotEquals.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaIntegerEqualTwo)
				&& error.PropertyNameTo == nameof(Alpha.OmegaIntegerEqualOne))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Equals.ToString()
				&& error.ErrorType == ValidationErrorType.NotEquals.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaLongEqualOne)
				&& error.PropertyNameTo == nameof(Alpha.OmegaLongEqualTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Equals.ToString()
				&& error.ErrorType == ValidationErrorType.NotEquals.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaLongEqualTwo)
				&& error.PropertyNameTo == nameof(Alpha.OmegaLongEqualOne))
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				Ny = 1,
				Omikron = Alphabet.A,
				Sigma = new List<int> { 1 },
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(8);


			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsString.ToString()
				&& error.PropertyName == nameof(Alpha.Gamma)
				&& error.PropertyNameTo == nameof(Alpha.Epsilon))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsString.ToString()
				&& error.PropertyName == nameof(Alpha.Delta)
				&& error.PropertyNameTo == nameof(Alpha.Epsilon))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsString.ToString()
				&& error.PropertyName == nameof(Alpha.Delta)
				&& error.PropertyNameTo == nameof(Alpha.EpsilonTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsAndNotEqualToDefaultString.ToString()
				&& error.PropertyName == nameof(Alpha.DeltaTwo)
				&& error.PropertyNameTo == nameof(Alpha.EpsilonTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsAndNotEqualToDefault.ToString().ToString()
				&& error.PropertyName == nameof(Alpha.Pi)
				&& error.PropertyNameTo == nameof(Alpha.Rho))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.EqualsAndNotEqualToDefault.ToString().ToString()
				&& error.PropertyName == nameof(Alpha.Rho)
				&& error.PropertyNameTo == nameof(Alpha.Pi))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.Equals.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaIntegerNotEqual)
				&& error.PropertyNameTo == nameof(Alpha.OmegaIntegerEqualTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.NotEquals.ToString()
				&& error.ErrorType == ValidationErrorType.Equals.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaLongNotEqual)
				&& error.PropertyNameTo == nameof(Alpha.OmegaLongEqualTwo))
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 7,
				Rho = 7,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				Tau = new Omega { Psi = "Darkwing Duck" },
				TauIEnumerable = new List<Omega> { new Omega { Psi = "Darkwing Duck" } },
				Ypsilon = new List<string> { "Darkwing", "Duck" },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(3);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.String.ToString()
				&& error.ErrorType == ValidationErrorType.LowerMinLength.ToString()
				&& error.PropertyName == nameof(Alpha.Gamma))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.String.ToString()
				&& error.ErrorType == ValidationErrorType.LowerMinLength.ToString()
				&& error.PropertyName == nameof(Alpha.Delta))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.String.ToString()
				&& error.ErrorType == ValidationErrorType.LowerMinLength.ToString()
				&& error.PropertyName == nameof(Alpha.Ypsilon))
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				Tau = new Omega { Chi = "Launchpad McQuack in action", Psi = "Darkwing Duck" },
				TauIEnumerable = new List<Omega> { new Omega { Chi = "Launchpad McQuack in action", Psi = "Darkwing Duck" } },
				Ypsilon = new List<string> { "Darkwing", "Launchpad McQuack in action" },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(6);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.String.ToString()
				&& error.ErrorType == ValidationErrorType.GreaterMaxLength.ToString()
				&& error.PropertyName == nameof(Alpha.Gamma))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.String.ToString()
				&& error.ErrorType == ValidationErrorType.GreaterMaxLength.ToString()
				&& error.PropertyName == nameof(Alpha.Delta))
			.Should().BeTrue();

			result.ValidationErrors.Count(error =>
				error.MethodType == ValidationMethodType.String.ToString()
				&& error.ErrorType == ValidationErrorType.GreaterMaxLength.ToString()
				&& error.PropertyName == nameof(Alpha.Chi))
			.Should().Be(2);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.String.ToString()
				&& error.ErrorType == ValidationErrorType.GreaterMaxLength.ToString()
				&& error.PropertyName == nameof(Alpha.Ypsilon))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.String.ToString()
				&& error.ErrorType == ValidationErrorType.GreaterMaxLength.ToString()
				&& error.PropertyName == nameof(Alpha.Phi))
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				Lambda = -1,
				LambdaNullable = 1,
				LambdaLong = -1L,
				LambdaLongNullable = 1L,
				My = -40.6m,
				Ny = 0.24f,
				Xi = -41,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(7);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeInteger.ToString()
				&& error.PropertyName == nameof(Alpha.Lambda))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeLong.ToString()
				&& error.PropertyName == nameof(Alpha.LambdaLong))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDecimal.ToString()
				&& error.PropertyName == nameof(Alpha.My))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeFloat.ToString()
				&& error.PropertyName == nameof(Alpha.Ny))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDouble.ToString()
				&& error.PropertyName == nameof(Alpha.Xi))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeInteger.ToString()
				&& error.PropertyName == nameof(Alpha.Omikron))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Enumeration.ToString()
				&& error.ErrorType == ValidationErrorType.Invalid.ToString()
				&& error.PropertyName == nameof(Alpha.Omikron)
				&& error.Value == "0")
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				Lambda = -1,
				LambdaNullable = 1,
				LambdaLong = -1L,
				LambdaLongNullable = 1L,
				My = 70.6m,
				Ny = 15.76f,
				Xi = 71,
				Omikron = (Alphabet)4,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(7);

			result.ValidationErrors.Any(error =>
					error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
					&& error.ErrorType == ValidationErrorType.DataTypeInteger.ToString()
					&& error.PropertyName == nameof(Alpha.Lambda))
				.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeLong.ToString()
				&& error.PropertyName == nameof(Alpha.LambdaLong))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDecimal.ToString()
				&& error.PropertyName == nameof(Alpha.My))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeFloat.ToString()
				&& error.PropertyName == nameof(Alpha.Ny))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDouble.ToString()
				&& error.PropertyName == nameof(Alpha.Xi))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeHardCoded.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeInteger.ToString()
				&& error.PropertyName == nameof(Alpha.Omikron))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Enumeration.ToString()
				&& error.ErrorType == ValidationErrorType.Invalid.ToString()
				&& error.PropertyName == nameof(Alpha.Omikron)
				&& error.Value == "4")
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				My = 1.00001m,
				Ny = 1,
				Xi = 1.1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(2);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.DecimalPlaces.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDecimal.ToString()
				&& error.PropertyName == nameof(Alpha.My))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.DecimalPlaces.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeFloatOrDouble.ToString()
				&& error.PropertyName == nameof(Alpha.Xi))
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				MyNullableOne = 1,
				MyNullableTwo = 0,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(2);

			result.ValidationErrors.Count(error =>
				error.MethodType == ValidationMethodType.Range.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDecimal.ToString()
				&& error.PropertyNameFrom == nameof(Alpha.MyNullableOne)
				&& error.PropertyNameTo == nameof(Alpha.MyNullableTwo))
			.Should().Be(2);
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				MyNullableThree = 1,
				MyNullableFour = 0,
				MyNullableFive = null,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeTrue();
			result.ValidationErrors.Should().HaveCount(0);
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				MyNullableThree = 1,
				MyNullableFour = null,
				MyNullableFive = 0,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(2);

			result.ValidationErrors.Count(error =>
				error.MethodType == ValidationMethodType.Range.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDecimal.ToString()
				&& error.PropertyNameFrom == nameof(Alpha.MyNullableThree)
				&& error.PropertyNameTo == nameof(Alpha.MyNullableFive))
			.Should().Be(2);
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
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
				OmegaLongFrom = 0L,
				OmegaLong = -1L,
				OmegaLongTo = 1L,
				OmegaDoubleFrom = 0,
				OmegaDouble = -1,
				OmegaDoubleTo = 1,
				OmegaDateTimeFrom = DateTime.Today,
				OmegaDateTime = DateTime.Today.AddDays(-1),
				OmegaDateTimeTo = DateTime.Today.AddDays(1),
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(6);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDecimal.ToString()
				&& error.PropertyName == nameof(Alpha.MyNullableSix)
				&& error.PropertyNameFrom == nameof(Alpha.MyNullableOne)
				&& error.PropertyNameTo == nameof(Alpha.MyNullableTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeFloat.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaFloat)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaFloatFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaFloatTo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeInteger.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaInteger)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaIntegerFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaIntegerTo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeLong.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaLong)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaLongFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaLongTo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDouble.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaDouble)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaDoubleFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaDoubleTo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDateTime.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaDateTime)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaDateTimeFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaDateTimeTo))
			.Should().BeTrue();
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
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
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
				OmegaLongFrom = 0L,
				OmegaLong = 2L,
				OmegaLongTo = 1L,
				OmegaDoubleFrom = 0,
				OmegaDouble = 2,
				OmegaDoubleTo = 1,
				OmegaDateTimeFrom = DateTime.Today,
				OmegaDateTime = DateTime.Today.AddDays(2),
				OmegaDateTimeTo = DateTime.Today.AddDays(1),
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(6);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDecimal.ToString()
				&& error.PropertyName == nameof(Alpha.MyNullableSix)
				&& error.PropertyNameFrom == nameof(Alpha.MyNullableOne)
				&& error.PropertyNameTo == nameof(Alpha.MyNullableTwo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeFloat.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaFloat)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaFloatFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaFloatTo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeInteger.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaInteger)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaIntegerFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaIntegerTo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeLong.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaLong)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaLongFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaLongTo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDouble.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaDouble)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaDoubleFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaDoubleTo))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.RangeBetween.ToString()
				&& error.ErrorType == ValidationErrorType.DataTypeDateTime.ToString()
				&& error.PropertyName == nameof(Alpha.OmegaDateTime)
				&& error.PropertyNameFrom == nameof(Alpha.OmegaDateTimeFrom)
				&& error.PropertyNameTo == nameof(Alpha.OmegaDateTimeTo))
			.Should().BeTrue();
		}

		[TestMethod]
		public void ValidateRangeBetweenAllowNullLeftTest()
		{
			// Arrange
			var data = new RangeBetweenData
			{
				AlphaFrom = null,
				AlphaValue = 5m,
				AlphaTo = 7m,
				BetaFrom = null,
				BetaValue = 5.0,
				BetaTo = 7.0,
				GammaFrom = null,
				GammaValue = 5f,
				GammaTo = 7f,
				DeltaFrom = null,
				DeltaValue = 5,
				DeltaTo = 7,
				EpsilonFrom = null,
				EpsilonValue = 5L,
				EpsilonTo = 7L,
				ZetaFrom = null,
				ZetaValue = DateTime.Today,
				ZetaTo = DateTime.Today.AddDays(2.0)
			};

			// Act
			var result = _classUnderTest.Validate(data);

			// Assert
			result.IsValid.Should().BeTrue();
		}

		[TestMethod]
		public void ValidateRangeBetweenAllowNullRightTest()
		{
			// Arrange
			var data = new RangeBetweenData
			{
				AlphaFrom = 3m,
				AlphaValue = 5m,
				AlphaTo = null,
				BetaFrom = 3.0,
				BetaValue = 5.0,
				BetaTo = null,
				GammaFrom = 3f,
				GammaValue = 5f,
				GammaTo = null,
				DeltaFrom = 3,
				DeltaValue = 5,
				DeltaTo = null,
				EpsilonFrom = 3L,
				EpsilonValue = 5L,
				EpsilonTo = null,
				ZetaFrom = DateTime.Today.AddDays(-2.0),
				ZetaValue = DateTime.Today,
				ZetaTo = null
			};

			// Act
			var result = _classUnderTest.Validate(data);

			// Assert
			result.IsValid.Should().BeTrue();
		}

		[TestMethod]
		public void ValidateRangeBetweenAllowNullLeftAndGreaterValueTest()
		{
			// Arrange
			var data = new RangeBetweenData
			{
				AlphaFrom = null,
				AlphaValue = 10m,
				AlphaTo = 7m,
				BetaFrom = null,
				BetaValue = 10.0,
				BetaTo = 7.0,
				GammaFrom = null,
				GammaValue = 10f,
				GammaTo = 7f,
				DeltaFrom = null,
				DeltaValue = 10,
				DeltaTo = 7,
				EpsilonFrom = null,
				EpsilonValue = 10L,
				EpsilonTo = 7L,
				ZetaFrom = null,
				ZetaValue = DateTime.Today.AddDays(4.0),
				ZetaTo = DateTime.Today.AddDays(2.0)
			};

			// Act
			var result = _classUnderTest.Validate(data);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(6);
			result.ValidationErrors.All(error =>
					error.MethodType == ValidationMethodType.RangeBetween.ToString()
				 && error.ErrorType != ValidationErrorType.DataTypesNotEqual.ToString()
				 && error.ErrorType != ValidationErrorType.DataTypeNotSupported.ToString())
				.Should()
				.BeTrue();
		}

		[TestMethod]
		public void ValidateRangeBetweenAllowNullRightAndLowerValueTest()
		{
			// Arrange
			var data = new RangeBetweenData
			{
				AlphaFrom = 10m,
				AlphaValue = 5m,
				AlphaTo = null,
				BetaFrom = 10.0,
				BetaValue = 5.0,
				BetaTo = null,
				GammaFrom = 10f,
				GammaValue = 5f,
				GammaTo = null,
				DeltaFrom = 10,
				DeltaValue = 5,
				DeltaTo = null,
				EpsilonFrom = 10L,
				EpsilonValue = 5L,
				EpsilonTo = null,
				ZetaFrom = DateTime.Today.AddDays(-2.0),
				ZetaValue = DateTime.Today.AddDays(-4.0),
				ZetaTo = null
			};

			// Act
			var result = _classUnderTest.Validate(data);

			// Assert
			result.IsValid.Should().BeFalse();
			result.ValidationErrors.Should().HaveCount(6);
			result.ValidationErrors.All(error =>
					error.MethodType == ValidationMethodType.RangeBetween.ToString()
				 && error.ErrorType != ValidationErrorType.DataTypesNotEqual.ToString()
				 && error.ErrorType != ValidationErrorType.DataTypeNotSupported.ToString())
				.Should()
				.BeTrue();
		}


		[DataTestMethod]
		[DataRow("Darkwing Duck", false, DisplayName = "Invalid url (1)")]
		[DataRow("http://www.eshava", false, DisplayName = "Invalid url (2)")]
		[DataRow("http://www.esh@ava.de", false, DisplayName = "Invalid url (2)")]
		[DataRow("http://www.eshava.de/", true, DisplayName = "Valid url with schema (http) and www")]
		[DataRow("https://www.eshava.de/", true, DisplayName = "Valid url with schema (https) and www")]
		[DataRow("http://eshava.de/", true, DisplayName = "Valid url with schema (http)")]
		[DataRow("https://eshava.de/", true, DisplayName = "Valid url with schema (https)")]
		[DataRow("https://www.develop.eshava.de/", true, DisplayName = "Valid url with schema (https) and sub domain")]
		[DataRow("www.eshava.de/", true, DisplayName = "Valid url without schema and with www")]
		[DataRow("eshava.de/", true, DisplayName = "Valid url without schema")]
		[DataRow("http://localhost:5000", true, DisplayName = "Valid url localhost")]
		[DataRow("http://192.168.1.1:5000", true, DisplayName = "Valid url IPv4")]
		[DataRow("[2001:0db8:85a3:08d3::0370:7344]", true, DisplayName = "Valid url IPv6 without schema")]
		[DataRow("http://[2001:0db8:85a3:08d3::0370:7344]:8080/", true, DisplayName = "Valid url IPv6 with schema and port")]
		public void ValidateUrlTest(string url, bool isValid)
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				DeltaUrl = url,
				EpsilonTwo = "Alpha",
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().Be(isValid);

			if (!isValid)
			{
				result.ValidationErrors.Should().HaveCount(1);

				result.ValidationErrors.Any(error =>
					error.MethodType == ValidationMethodType.String.ToString()
					&& error.ErrorType == ValidationErrorType.NoWellFormedUri.ToString()
					&& error.PropertyName == nameof(Alpha.DeltaUrl))
				.Should().BeTrue();
			}
		}

		[DataTestMethod]
		[DataRow("Darkwing Duck", false, DisplayName = "Invalid mail address (1)")]
		[DataRow("Darkwing.Duck", false, DisplayName = "Invalid mail address (2)")]
		[DataRow("Darkwing.Duck@", false, DisplayName = "Invalid mail address (3)")]
		[DataRow("Darkwing.Duck@eshava", false, DisplayName = "Invalid mail address (4)")]
		[DataRow("Darkwing.Duck@eshava.", false, DisplayName = "Invalid mail address (5)")]
		[DataRow("Darkwing.Duck@eshava.de", true, DisplayName = "Valid mail address")]
		[DataRow("@eshava.de", false, DisplayName = "Invalid mail address (6)")]
		public void ValidateMailAddressTest(string mailAddress, bool isValid)
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				DeltaMail = mailAddress,
				EpsilonTwo = "Alpha",
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow

			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().Be(isValid);

			if (!isValid)
			{
				result.ValidationErrors.Should().HaveCount(1);

				result.ValidationErrors.Any(error =>
					error.MethodType == ValidationMethodType.String.ToString()
					&& error.ErrorType == ValidationErrorType.NoWellFormedMailAddress.ToString()
					&& error.PropertyName == nameof(Alpha.DeltaMail))
				.Should().BeTrue();
			}
		}

		[DataTestMethod]
		[DataRow("a-b1-c2", true, DisplayName = "Valid value for regular expression (1)")]
		[DataRow("a-B-2", true, DisplayName = "Valid value for regular expression (2)")]
		[DataRow("a--c", false, DisplayName = "Invalid value for regular expression")]
		public void ValidateRegularExpressionTest(string omegaRegEx, bool isValid)
		{
			// Arrange
			var source = new Alpha
			{
				Gamma = "QuackFu",
				Delta = "QuackFu",
				DeltaTwo = "Alpha",
				DeltaMail = null,
				EpsilonTwo = "Alpha",
				EpsilonThree = "Alpha",
				EpsilonFour = "",
				LambdaNullable = 1,
				LambdaLongNullable = 1L,
				Ny = 1,
				Omikron = Alphabet.A,
				Pi = 2,
				Rho = 3,
				Sigma = new List<int> { 1 },
				OmegaIntegerNotEqual = 1,
				OmegaLongNotEqual = 1L,
				ChiNotNull = Guid.NewGuid(),
				ChiNullable = Guid.NewGuid(),
				PsiRequired = DateTime.UtcNow,
				PsiRequiredNullable = DateTime.UtcNow,
				OmegaRegEx = omegaRegEx
			};

			// Act
			var result = _classUnderTest.Validate(source);

			// Assert
			result.IsValid.Should().Be(isValid);

			if (!isValid)
			{
				result.ValidationErrors.Should().HaveCount(1);

				result.ValidationErrors.Any(error =>
					error.MethodType == ValidationMethodType.String.ToString()
					&& error.ErrorType == ValidationErrorType.RegularExpression.ToString()
					&& error.PropertyName == nameof(Alpha.OmegaRegEx))
				.Should().BeTrue();
			}
		}

		[TestMethod]
		public void ValidateEnumValues()
		{
			// Arrange
			var complextData = new ComplexData
			{
				/* invalidated zero */
				PrimaryColor = Color.Transparent,
				/* disabled validation */
				SecondaryColor = (Color)999,
				/* default configuration */
				BaseColor = (Color)999
			};

			// Act
			var result = _classUnderTest.Validate(complextData);

			// Assert
			result.IsValid.Should().BeFalse();

			result.ValidationErrors.Should().HaveCount(2);

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Enumeration.ToString()
				&& error.ErrorType == ValidationErrorType.Invalid.ToString()
				&& error.PropertyName == nameof(ComplexData.PrimaryColor)
				&& error.Value == nameof(Color.Transparent))
			.Should().BeTrue();

			result.ValidationErrors.Any(error =>
				error.MethodType == ValidationMethodType.Enumeration.ToString()
				&& error.ErrorType == ValidationErrorType.Invalid.ToString()
				&& error.PropertyName == nameof(ComplexData.BaseColor)
				&& error.Value == "999")
			.Should().BeTrue();
		}
	}
}