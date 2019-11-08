using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.Extensions
{
	internal static class StringExtension
	{
		public static BaseField GetFieldSource<T, D>(this string fieldSourceName, ValidationCheckParameters<T, D> parameters)
		{
			var fieldSource = parameters.FieldInformation.GetField(fieldSourceName);
			if (fieldSource == null)
			{
				//No dynamic field, search directly in the analysis result
				return parameters.AnalysisResult.Result.ContainsKey(fieldSourceName) ? parameters.AnalysisResult.Result[fieldSourceName] : null;
			}

			var assignmentSourceId = fieldSource.AssignmentId.ToString();

			return parameters.AnalysisResult.Result.ContainsKey(assignmentSourceId) ? parameters.AnalysisResult.Result[assignmentSourceId] : null;
		}
	}
}