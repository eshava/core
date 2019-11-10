using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation.Interfaces;
using Eshava.Core.Validation.Models;

namespace Eshava.Core.Dynamic.Fields.Validation
{
	public class FieldValidationRuleEngine<FD, FA, FV, T, D> : IFieldValidationRuleEngine<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		public IEnumerable<ValidationPropertyInfo> CalculateValidationRules(FieldInformation<FD, FA, FV, T, D> fieldInformation)
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
					IsDynamicField = true,
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
					case FieldConfigurationType.MinLength:
					case FieldConfigurationType.MaxLength:
						AddStringLengthRule(definition, configuration, validationProperty);
						break;
					case FieldConfigurationType.Maximum:
					case FieldConfigurationType.Minimum:
						AddRangeRule(definition, configuration, validationProperty);
						break;
					case FieldConfigurationType.EqualsTo:
						validationProperty.Rules.Add(new ValidationRule
						{
							Rule = "EqualsTo",
							PropertyName = configuration.ValueString.Trim()
						});
						break;
					case FieldConfigurationType.NotEqualsTo:
						AddNotEqualsRule(definition, configuration, validationProperty);
						break;
					case FieldConfigurationType.DecimalPlaces:
						var decimalPlacesRule = validationProperty.Rules.SingleOrDefault(rule => rule.Rule == "DecimalPlaces");
						if (decimalPlacesRule == null)
						{
							validationProperty.Rules.Add(new ValidationRule
							{
								Rule = "DecimalPlaces",
								Value = configuration.ValueInteger ?? 0
							});
						}
						else
						{
							decimalPlacesRule.Value = configuration.ValueInteger ?? 0;
						}
						break;
					case FieldConfigurationType.Date:
						validationProperty.DataType = "date";
						break;
					case FieldConfigurationType.Time:
						validationProperty.DataType = "time";
						break;
					case FieldConfigurationType.Email:
						validationProperty.Rules.Add(new ValidationRule { Rule = "Email" });
						break;
					case FieldConfigurationType.Url:
						validationProperty.Rules.Add(new ValidationRule { Rule = "Url" });
						break;
					case FieldConfigurationType.RangeFrom:
						AddRangeFromRule(definition, configuration, validationProperty);
						break;
					case FieldConfigurationType.RangeTo:
						AddRangeToRule(definition, configuration, validationProperty);
						break;
					case FieldConfigurationType.RangeBetween:
						AddRangeBetweenRule(definition, validationProperty);
						break;
				}
			}
		}

		private void AddStringLengthRule(IFieldDefinition<T> definition, IFieldConfiguration<T> configuration, ValidationPropertyInfo validationProperty)
		{
			if (configuration.ValueInteger.HasValue)
			{
				var ruleName = configuration.ConfigurationType == FieldConfigurationType.MinLength ? "MinLength" : "MaxLength";
				var rule = new ValidationRule { Rule = ruleName, Value = configuration.ValueInteger.Value };
				validationProperty.Rules.Add(rule);
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

		private void AddRangeFromRule(IFieldDefinition<T> definition, IFieldConfiguration<T> configuration, ValidationPropertyInfo validationProperty)
		{
			configuration.ValueString.Split(',').ToList().ForEach(fieldName =>
			{
				validationProperty.Rules.Add(new ValidationRule
				{
					Rule = "RangeFrom",
					PropertyNameFrom = fieldName.Trim(),
					PropertyNameFromAllowNull = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.AllowNull) != null
				});
			});
		}

		private void AddRangeToRule(IFieldDefinition<T> definition, IFieldConfiguration<T> configuration, ValidationPropertyInfo validationProperty)
		{
			configuration.ValueString.Split(',').ToList().ForEach(fieldName =>
			{
				validationProperty.Rules.Add(new ValidationRule
				{
					Rule = "RangeTo",
					PropertyNameTo = fieldName.Trim(),
					PropertyNameToAllowNull = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.AllowNull) != null
				});
			});
		}

		private void AddRangeBetweenRule(IFieldDefinition<T> definition, ValidationPropertyInfo validationProperty)
		{
			var rangeFrom = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.RangeBetweenFrom);
			var rangeTo = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.RangeBetweenTo);

			if (rangeFrom != null && rangeTo != null)
			{
				var allowNull = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.AllowNull) != null;
				validationProperty.Rules.Add(new ValidationRule
				{
					Rule = "RangeBetween",
					PropertyNameFrom = rangeFrom.ValueString,
					PropertyNameTo = rangeTo.ValueString,
					PropertyNameFromAllowNull = allowNull,
					PropertyNameToAllowNull = allowNull
				});
			}
		}

		private void AddNotEqualsRule(IFieldDefinition<T> definition, IFieldConfiguration<T> configuration, ValidationPropertyInfo validationProperty)
		{
			var configNotEqualDefault = definition.Configurations.FirstOrDefault(c => c.ConfigurationType == FieldConfigurationType.NotEqualsDefault);
			string defaultValue = null;

			switch (definition.FieldType)
			{
				case FieldType.NumberInteger:
				case FieldType.ComboBoxInteger:
					defaultValue = configNotEqualDefault?.ValueInteger?.ToString(CultureInfo.InvariantCulture);
					break;
				case FieldType.NumberLong:
					defaultValue = configNotEqualDefault?.ValueLong?.ToString(CultureInfo.InvariantCulture);
					break;
				case FieldType.NumberDecimal:
					defaultValue = configNotEqualDefault?.ValueDecimal?.ToString(CultureInfo.InvariantCulture);
					break;
				case FieldType.NumberDouble:
					defaultValue = configNotEqualDefault?.ValueDouble?.ToString(CultureInfo.InvariantCulture);
					break;
				case FieldType.NumberFloat:
					defaultValue = configNotEqualDefault?.ValueFloat?.ToString(CultureInfo.InvariantCulture);
					break;
				case FieldType.DateTime:
					defaultValue = configNotEqualDefault?.ValueDateTime?.ToString("yyyy-MM-ddTHH:mm:ss");
					break;
				case FieldType.AutoComplete:
				case FieldType.Text:
				case FieldType.TextMultiline:
					defaultValue = configNotEqualDefault?.ValueString?.ToString(CultureInfo.InvariantCulture);
					break;
				case FieldType.ComboxBoxGuid:
				case FieldType.Guid:
					defaultValue = configNotEqualDefault?.ValueGuid.ToString();
					break;
			}

			validationProperty.Rules.Add(new ValidationRule
			{
				Rule = "NotEqualsTo",
				PropertyName = configuration.ValueString.Trim(),
				DefaultValue = defaultValue
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
					validationProperty.Rules.Add(new ValidationRule
					{
						Rule = "DecimalPlaces",
						Value = 0
					});
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
				case FieldType.ComboBoxInteger:
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