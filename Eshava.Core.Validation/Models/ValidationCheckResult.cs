using System.Collections.Generic;

namespace Eshava.Core.Validation.Models
{
	public class ValidationCheckResult
	{
		public ValidationCheckResult()
		{
			ValidationErrors = new List<ValidationCheckResultEntry>();
		}

		public bool IsValid { get; set; }
		public IList<ValidationCheckResultEntry> ValidationErrors { get; set; }
	}
}