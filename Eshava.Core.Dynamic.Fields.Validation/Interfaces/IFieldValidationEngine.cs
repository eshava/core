using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.Interfaces
{
	public interface IFieldValidationEngine<T, D>
	{
		ValidationCheckResult Validate(FieldInformation<T, D> fieldInformation, AnalysisResult analysisResult);
	}
}