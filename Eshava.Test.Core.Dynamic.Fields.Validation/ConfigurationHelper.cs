using System.Collections.Generic;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Extensions;
using Eshava.Test.Core.Dynamic.Fields.Validation.Models;

namespace Eshava.Test.Core.Dynamic.Fields.Validation
{
	internal static class ConfigurationHelper
	{
		public static DynamicFieldDefinition CreateDefinition(FieldType fieldType, FieldConfigurationType? fieldConfigurationType = null, string nameSuffix = null)
		{
			return CreateDefinition(fieldType.ToString(), fieldType, fieldConfigurationType, nameSuffix);
		}

		public static DynamicFieldDefinition CreateDefinition(string fieldName, FieldType fieldType, FieldConfigurationType? fieldConfigurationType = null, string nameSuffix = null)
		{
			var configurations = new List<DynamicFieldConfiguration>();
			if (fieldConfigurationType.HasValue)
			{
				configurations.Add(new DynamicFieldConfiguration { ConfigurationType = fieldConfigurationType.Value });
			}

			return new DynamicFieldDefinition
			{
				FieldType = fieldType,
				Id = fieldName + (nameSuffix ?? ""),
				Configurations = configurations
			};
		}

		public static DynamicFieldAssignment CreateAssignment(FieldType fieldType, string nameSuffix = null)
		{
			return CreateAssignment(fieldType.ToString(), nameSuffix);
		}

		public static DynamicFieldAssignment CreateAssignment(string fieldName, string nameSuffix = null)
		{
			return new DynamicFieldAssignment
			{
				Id = fieldName + "Assignment" + (nameSuffix ?? ""),
				DefinitionId = fieldName + (nameSuffix ?? "")
			};
		}

		public static DynamicFieldValue CreateField(FieldType fieldType, string valuePropertyName = null, object value = null, string nameSuffix = null)
		{
			return CreateField(fieldType.ToString(), valuePropertyName, value, nameSuffix);
		}

		public static DynamicFieldValue CreateField(string fieldName, string valuePropertyName = null, object value = null, string nameSuffix = null)
		{
			var field = new DynamicFieldValue
			{
				Id = fieldName + "Value" + (nameSuffix ?? ""),
				AssignmentId = fieldName + "Assignment" + (nameSuffix ?? "")
			};

			if (!valuePropertyName.IsNullOrEmpty() && value != null)
			{
				typeof(DynamicFieldValue).GetProperty(valuePropertyName).SetValue(field, value);
			}

			return field;
		}
	}
}