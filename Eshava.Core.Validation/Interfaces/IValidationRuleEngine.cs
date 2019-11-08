using System.Collections.Generic;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.Interfaces
{
	public interface IValidationRuleEngine
	{
		IEnumerable<ValidationPropertyInfo> CalculateValidationRules<T>() where T : class;
	}
}