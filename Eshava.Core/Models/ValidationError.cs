namespace Eshava.Core.Models
{
	public class ValidationError
	{
		public string MethodType { get; set; }
		public string ErrorType { get; set; }
		public string Value { get; set; }
		public string PropertyName { get; set; }
		public string PropertyNameFrom { get; set; }
		public string PropertyNameTo { get; set; }
	}
}