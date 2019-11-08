using System;
using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.Interfaces
{
	public interface IFieldValidationRuleEngine<T, D>
	{
		IEnumerable<ValidationPropertyInfo> CalculateValidationRules(FieldInformation<T, D> fieldInformation);
	}
}
