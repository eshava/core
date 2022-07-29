namespace Eshava.Core.Validation.Enums
{
	public enum ValidationErrorType
	{
		DataTypeNotSupported = 0,
		DataTypesNotEqual = 1,
		DataTypeInteger = 2,
		DataTypeLong = 3,
		DataTypeDecimal = 4,
		DataTypeDouble = 5,
		DataTypeFloat = 6,
		DataTypeFloatOrDouble = 7,
		DataTypeDateTime = 8,
		Equals = 9,
		NotEquals = 10,
		EqualsAndNotEqualToDefault = 11,
		EqualsString = 12,
		NotEqualsString = 13,
		EqualsAndNotEqualToDefaultString = 14,
		PropertyNotFoundFrom = 15,
		PropertyNotFoundTo = 16,
		IsEmptyIEnumerable = 17,
		IsNull = 18,
		IsEmpty = 19,
		NoWellFormedUri = 20,
		NoWellFormedMailAddress = 21,
		GreaterMaxLength = 22,
		LowerMinLength = 23,
		RegularExpression = 24,
		Invalid = 25
	}
}