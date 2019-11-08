using System.Collections.Generic;

namespace Eshava.Core.Validation.Models
{
	public class ValidationCheckResult
	{
		public ValidationCheckResult()
		{
			ValidationErrors = new List<ValidationCheckResultEntry>();
		}

		public IList<ValidationCheckResultEntry> ValidationErrors { get; set; }
		public bool IsValid { get { return ValidationErrors.Count == 0; } }
	}
}