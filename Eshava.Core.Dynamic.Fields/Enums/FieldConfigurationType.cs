namespace Eshava.Core.Dynamic.Fields.Enums
{
	public enum FieldConfigurationType
	{
		None = 0,
		Required = 1,
		Minimum = 2, /* Number */
		Maximum = 3, /* Number */
		EqualsTo = 4,
		NotEqualsTo = 5,
		DecimalPlaces = 6, /* Number */
		Date = 7, /* DateTime */
		Email = 8, /* Text */
		Url = 9, /* Text */
		RangeFrom = 10,
		RangeTo = 11,
		RangeBetween = 12,
		RangeBetweenFrom = 13,
		RangeBetweenTo = 14,
		CodeAssemblies = 15,
		CodeScript = 16,
		CodeClassname = 17,
		MinLength = 18, /* Text */
		MaxLength = 19, /* Text */
		RangeRequired = 20,
		ComboBoxItem = 21,
		TextMultilineRows = 22,
		NotEqualsDefault = 23,
		CSSClass = 24 /* BoxedCheckBox */,
		AllowNull = 25,
		Time = 26
	}
}