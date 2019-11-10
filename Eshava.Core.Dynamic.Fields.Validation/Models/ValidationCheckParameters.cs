using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.Models
{
	internal class ValidationCheckParameters<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		public AnalysisResult AnalysisResult { get; set; }
		public FieldInformation<FD, FA, FV, T, D> FieldInformation { get; set; }
		public FD FieldDefinition { get; set; }
		public BaseField Field { get; set; }
		public FV PropertyInfo { get; set; }
		public bool NotEquals { get; set; }
		public bool AllowNull { get; set; }

		public IEnumerable<IFieldConfiguration<T>> GetConfigurations(FieldConfigurationType type)
		{
			return FieldDefinition.Configurations.Where(c => c.ConfigurationType == type);
		}
	}
}