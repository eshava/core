using System.Collections.Generic;

namespace Eshava.Core.Dynamic.Fields.Models
{
	public class AnalysisResult
	{
		public AnalysisResult(Dictionary<string, BaseField> result)
		{
			Result = result;
		}

		public Dictionary<string, BaseField> Result { get; private set; }
	}
}