using Eshava.Core.Dynamic.Fields.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.Interfaces
{
	public interface IFieldAnalyzer<T, D>
	{
		AnalysisResult Analyse(object model, FieldInformation<T, D> fieldInformation);
	}
}