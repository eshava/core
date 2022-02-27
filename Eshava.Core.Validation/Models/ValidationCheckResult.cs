using System.Collections.Generic;
using Eshava.Core.Models;

namespace Eshava.Core.Validation.Models
{
	public class ValidationCheckResult
	{
		public ValidationCheckResult()
		{
			ValidationErrors = new List<ValidationError>();
		}

		public IList<ValidationError> ValidationErrors { get; set; }
		public bool IsValid { get { return ValidationErrors.Count == 0; } }
	}
}