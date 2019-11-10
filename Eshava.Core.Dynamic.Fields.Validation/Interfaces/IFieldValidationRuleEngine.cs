using System;
using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.Interfaces
{
	public interface IFieldValidationRuleEngine<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		IEnumerable<ValidationPropertyInfo> CalculateValidationRules(FieldInformation<FD, FA, FV, T, D> fieldInformation);
	}
}
