using System.Collections.Generic;

namespace Eshava.Core.Validation.Models
{
	public class ValidationPropertyInfo
	{
		public string PropertyName { get; set; }
		public string JsonName { get; set; }
		public string DataType { get; set; }
		public bool IsDynamicField { get; set; }
		public IList<ValidationRule> Rules { get; set; }
	}
}