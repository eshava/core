using System;
using System.Collections.Generic;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation.Interfaces;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation
{
	public class FieldValidationRuleEngine<T, D> : IFieldValidationRuleEngine<T, D>
	{
		public IEnumerable<ValidationPropertyInfo> CalculateValidationRules(FieldInformation<T, D> fieldInformation)
		{
			var validationProperties = new List<ValidationPropertyInfo>();

			foreach (var assignment in fieldInformation.Assignments)
			{
				var definition = fieldInformation.Definitions.Single(d => d.Id.Equals(assignment.DefinitionId));

				if (definition.FieldType == FieldType.None || definition.FieldType == FieldType.DynamicCode)
				{
					continue;
				}

				var validationProperty = new ValidationPropertyInfo
				{
					PropertyName = definition.Id.ToString(),
					JsonName = definition.Id.ToString(),
					CustomerSpecificFields = true,
					Rules = new List<ValidationRule>()
				};

				SetDataType(definition, validationProperty);
				AddValidationRules(definition, validationProperty);

				validationProperties.Add(validationProperty);
			}

			return validationProperties;
		}

		private void AddValidationRules(IFieldDefinition<T> definition, ValidationPropertyInfo validationProperty)
		{
			foreach (var configuration in definition.Configurations)
			{
				switch (configuration.ConfigurationType)
				{
					case FieldConfigurationType.Required:
						validationProperty.Rules.Add(new ValidationRule { Rule = "Required" });
						break;
					case FieldConfigurationType.Maximum:
					case FieldConfigurationType.Minimum:
						AddRangeRule(definition, configuration, validationProperty);
						break;
					case FieldConfigurationType.EqualsTo:
						configuration.ValueString.Split(',').ToList().ForEach(fieldName =>
						{
							validationProperty.Rules.Add(new ValidationRule
							{
								Rule = "EqualsTo",
								Field = fieldName.Trim()
							});
						});
						break;
					case FieldConfigurationType.NotEqualsTo:
						AddNotEqualsRule(definition, configuration, validationProperty);
						break;
					case FieldConfigurationType.DecimalPlaces:
						validationProperty.Rules.Add(new ValidationRule
						{
							Rule = "Decimals",
							Value = configuration.ValueInteger ?? 0
						});
						break;
					case FieldConfigurationType.Date:
						validationProperty.DataType = "date";
						break;
					case FieldConfigurationType.Email:
						validationProperty.Rules.Add(new ValidationRule { Rule = "Email" });
						break;
					case FieldConfigurationType.Url:
						validationProperty.Rules.Add(new ValidationRule { Rule = "Url" });
						break;
					case FieldConfigurationType.RangeFrom:
					case FieldConfigurationType.RangeTo:
						AddRangeFromOrToRule(definition, configuration, validationProperty);
						break;
					case FieldConfigurationType.RangeBetween:
						AddRangeBetweenRule(definition, validationProperty);
						break;
				}
			}
		}

		private void AddRangeRule(IFieldDefinition<T> definition, IFieldConfiguration<T> configuration, ValidationPropertyInfo validationProperty)
		{
			var rule = validationProperty.Rules.FirstOrDefault(r => r.Rule == "Range");

			if (rule == null)
			{
				rule = new ValidationRule { Rule = "Range" };
				validationProperty.Rules.Add(rule);
			}

			var typeInteger = definition.FieldType == FieldType.NumberInteger;

			if (configuration.ConfigurationType == FieldConfigurationType.Minimum)
			{
				rule.Minimum = typeInteger ? Convert.ToDecimal(configuration.ValueInteger) : configuration.ValueDecimal ?? 0m;
			}
			else
			{
				rule.Maximum = typeInteger ? Convert.ToDecimal(configuration.ValueInteger) : configuration.ValueDecimal ?? 0m;
			}
		}

		private void AddRangeFromOrToRule(IFieldDefinition<T> definition, IFieldConfiguration<T> configuration, ValidationPropertyInfo validationProperty)
		{
			var ruleName = configuration.ConfigurationType == FieldConfigurationType.RangeFrom ? "RangeFrom" : "RangeTo";
			configuration.ValueString.Split(',').ToList().ForEach(fieldName =>
			{
				validationProperty.Rules.Add(new ValidationRule
				{
					Rule = ruleName,
					FieldTo = fieldName.Trim(),
					FieldToAllowNull = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.RangeRequired) == null
				});
			});
		}

		private void AddRangeBetweenRule(IFieldDefinition<T> definition, ValidationPropertyInfo validationProperty)
		{
			var rangeFrom = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.RangeBetweenFrom);
			var rangeTo = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.RangeBetweenTo);

			if (rangeFrom != null && rangeTo != null)
			{
				validationProperty.Rules.Add(new ValidationRule
				{
					Rule = "RangeBetween",
					FieldFrom = rangeFrom.ValueString,
					FieldTo = rangeTo.ValueString
				});
			}
		}

		private void AddNotEqualsRule(IFieldDefinition<T> definition, IFieldConfiguration<T> configuration, ValidationPropertyInfo validationProperty)
		{
			var configNotEqualDefault = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.NotEqualsDefault);
			object defaultValue = null;

			switch (definition.FieldType)
			{
				case FieldType.NumberInteger:
				case FieldType.ComboBoxInt:
					defaultValue = configNotEqualDefault?.ValueInteger;
					break;
				case FieldType.NumberDecimal:
				case FieldType.NumberDouble:
				case FieldType.NumberFloat:
					defaultValue = configNotEqualDefault?.ValueDecimal;
					break;
				case FieldType.DateTime:
				case FieldType.AutoComplete:
				case FieldType.Text:
				case FieldType.TextMultiline:
					defaultValue = configNotEqualDefault?.ValueString;
					break;
				case FieldType.ComboxBoxGuid:
				case FieldType.Guid:
					defaultValue = configNotEqualDefault?.ValueGuid;
					break;
			}

			configuration.ValueString.Split(',').ToList().ForEach(fieldName =>
			{
				validationProperty.Rules.Add(new ValidationRule
				{
					Rule = "NotEqualsTo",
					Field = fieldName.Trim(),
					DefaultValue = defaultValue?.ToString()
				});
			});
		}

		private void SetDataType(IFieldDefinition<T> definition, ValidationPropertyInfo validationProperty)
		{
			switch (definition.FieldType)
			{
				case FieldType.NumberInteger:
				case FieldType.NumberDecimal:
				case FieldType.NumberDouble:
				case FieldType.NumberFloat:
					validationProperty.DataType = "number";
					validationProperty.Rules.Add(new ValidationRule { Rule = "Number" });
					break;
				case FieldType.AutoComplete:
				case FieldType.Text:
					validationProperty.DataType = "string";
					break;
				case FieldType.TextMultiline:
					validationProperty.DataType = "multiline";
					break;
				case FieldType.DateTime:
					validationProperty.DataType = "dateTime";
					break;
				case FieldType.BoxedCheckbox:
				case FieldType.Checkbox:
					validationProperty.DataType = "boolean";
					break;
				case FieldType.ComboBoxInt:
					validationProperty.DataType = "select";
					validationProperty.Rules.Add(new ValidationRule { Rule = "Number" });
					break;
				case FieldType.ComboxBoxGuid:
					validationProperty.DataType = "select";
					validationProperty.Rules.Add(new ValidationRule { Rule = "Guid" });
					break;
				case FieldType.Guid:
					validationProperty.DataType = "string";
					validationProperty.Rules.Add(new ValidationRule { Rule = "Guid" });
					break;
			}
		}
	}
}