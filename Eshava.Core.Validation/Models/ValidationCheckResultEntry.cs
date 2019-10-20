using Eshava.Core.Validation.Enums;

namespace Eshava.Core.Validation.Models
{
	public class ValidationCheckResultEntry
	{
		public ValidationMethodType MethodType { get; set; }
		public ValidationErrorType ErrorType { get; set; }
		public string PropertyName { get; set; }
		public string PropertyNameFrom { get; set; }
		public string PropertyNameTo { get; set; }
	}
}