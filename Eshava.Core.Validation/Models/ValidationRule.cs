namespace Eshava.Core.Validation.Models
{
	public class ValidationRule
	{
		public string Rule { get; set; }
		public int Value { get; set; }
		public string DefaultValue { get; set; }
		public string RegEx { get; set; }
		public decimal Minimum { get; set; }
		public decimal Maximum { get; set; }

		public string PropertyName { get; set; }
		public string PropertyNameFrom { get; set; }
		public string PropertyNameTo { get; set; }
		public bool PropertyNameFromAllowNull { get; set; }
		public bool PropertyNameToAllowNull { get; set; }
	}
}