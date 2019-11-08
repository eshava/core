using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;

namespace Eshava.Core.Dynamic.Fields.Validation.Models
{
	internal class ValidationCheckParameters<T, D>
	{
		public AnalysisResult AnalysisResult { get; set; }
		public FieldInformation<T, D> FieldInformation { get; set; }
		public IFieldDefinition<T> FieldDefinition { get; set; }
		public BaseField Field { get; set; }
		public IFieldValue<T> PropertyInfo { get; set; }
		public bool NotEquals { get; set; }
		public bool AllowNull { get; set; }

		public IEnumerable<IFieldConfiguration<T>> GetConfigurations(FieldConfigurationType type)
		{
			return FieldDefinition.Configurations.Where(c => c.ConfigurationType == type);
		}
	}
}