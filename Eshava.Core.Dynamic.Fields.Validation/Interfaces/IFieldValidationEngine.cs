using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.Interfaces
{
	public interface IFieldValidationEngine<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		ValidationCheckResult Validate(FieldInformation<FD, FA, FV, T, D> fieldInformation, AnalysisResult analysisResult);
	}
}