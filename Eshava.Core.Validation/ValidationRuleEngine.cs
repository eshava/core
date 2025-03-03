using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Extension;
using Eshava.Core.Validation.Interfaces;
using Eshava.Core.Validation.Models;
using Newtonsoft.Json;

namespace Eshava.Core.Validation
{
	public class ValidationRuleEngine : IValidationRuleEngine
	{
		private static readonly List<(Func<Type, bool> Check, Action<PropertyInfo, ValidationPropertyInfo> SetDataType)> _dataTypeRules =
			new List<(Func<Type, bool> Check, Action<PropertyInfo, ValidationPropertyInfo> SetDataType)>
		{
				(type => type.IsNumber(), SetNumberDataType),
				(type => type.IsEnum(), (propertyInfo, validationProperty) => { SetDataTypeAndRule(validationProperty, "select", "Number"); }),
				(type => type.IsDateTime(), (propertyInfo, validationProperty) => {SetDataTypeAndRule(validationProperty, "dateTime"); }),
				(type => type.IsGuid(), (propertyInfo, validationProperty) => { SetDataTypeAndRule(validationProperty, "select", "Guid");  }),
				(type => type.IsBoolean(), (propertyInfo, validationProperty) => { SetDataTypeAndRule(validationProperty,"boolean");  })
		};

		private static readonly List<(Func<PropertyInfo, bool> Check, Action<ValidationPropertyInfo> SetDataType)> _dataTypeAttributeRules =
			new List<(Func<PropertyInfo, bool> Check, Action<ValidationPropertyInfo> SetDataType)>
		{
				(propertyInfo => propertyInfo.HasAttribute<TagsAttribute>(), validationProperty => { SetDataTypeAndRule(validationProperty,"tag"); }),
				(propertyInfo => propertyInfo.HasAttribute<TypeaheadAttribute>(), validationProperty => { SetDataTypeAndRule(validationProperty,"typeahead"); }),
				(propertyInfo => propertyInfo.HasAttribute<DropDownListAttribute>(), validationProperty => { SetDataTypeAndRule(validationProperty,"select"); })
		};


		public IEnumerable<ValidationPropertyInfo> CalculateValidationRules<T>(bool produceTreeStructure = false) where T : class
		{
			return CalculateValidationRules(typeof(T), produceTreeStructure);
		}

		private IEnumerable<ValidationPropertyInfo> CalculateValidationRules(Type type, bool produceTreeStructure)
		{
			var validationProperties = new List<ValidationPropertyInfo>();

			foreach (var propertyInfo in type.GetProperties())
			{
				if (propertyInfo.HasAttribute<ValidationIgnoreAttribute>())
				{
					continue;
				}

				if (propertyInfo.PropertyType.ImplementsIEnumerable())
				{
					var e = propertyInfo.PropertyType.GetDataTypeFromIEnumerable();
					if (e.IsComplexDataType())
					{
						var validationRules = MapRules(propertyInfo, CalculateValidationRules(e, produceTreeStructure), produceTreeStructure);

						TryAddValidationProperties(validationProperties, validationRules);
					}
					else
					{
						TryAddValidationProperty(validationProperties, CalculateValidationProperty(propertyInfo));
					}
				}
				else if (propertyInfo.PropertyType.IsComplexDataType())
				{
					var validationRules = MapRules(propertyInfo, CalculateValidationRules(propertyInfo.PropertyType, produceTreeStructure), produceTreeStructure);

					TryAddValidationProperties(validationProperties, validationRules);
				}
				else
				{
					TryAddValidationProperty(validationProperties, CalculateValidationProperty(propertyInfo));
				}
			}

			return validationProperties;
		}

		private IEnumerable<ValidationPropertyInfo> MapRules(PropertyInfo propertyInfo, IEnumerable<ValidationPropertyInfo> validationRules, bool produceTreeStructure)
		{
			if (!produceTreeStructure)
			{
				return validationRules;
			}

			return new List<ValidationPropertyInfo>
			{
				new ValidationPropertyInfo
				{
					PropertyName = propertyInfo.Name,
					IsClassContainer = true,
					Properties = validationRules.ToList()
				}
			};
		}

		private void TryAddValidationProperties(List<ValidationPropertyInfo> validationProperties, IEnumerable<ValidationPropertyInfo> validationPropertiesToAdd)
		{
			if (validationPropertiesToAdd == null)
			{
				return;
			}

			foreach (var validationProperty in validationPropertiesToAdd)
			{
				TryAddValidationProperty(validationProperties, validationProperty);
			}
		}

		private void TryAddValidationProperty(List<ValidationPropertyInfo> validationProperties, ValidationPropertyInfo validationProperty)
		{
			if (validationProperty != null && validationProperties.All(property => property.PropertyName != validationProperty.PropertyName))
			{
				validationProperties.Add(validationProperty);
			}
		}

		private ValidationPropertyInfo CalculateValidationProperty(PropertyInfo propertyInfo)
		{
			var validationProperty = new ValidationPropertyInfo
			{
				PropertyName = propertyInfo.Name,
				JsonName = GetJsonPropertyName(propertyInfo),
				Rules = new List<ValidationRule>()
			};

			AddRuleRequired(propertyInfo, validationProperty.Rules);
			AddRuleStringLength(propertyInfo, validationProperty.Rules);
			AddRuleRange(propertyInfo, validationProperty.Rules);
			AddRuleRangeFrom(propertyInfo, validationProperty.Rules);
			AddRuleRangeTo(propertyInfo, validationProperty.Rules);
			AddRuleRangeBetween(propertyInfo, validationProperty.Rules);
			AddRuleDecimalPlaces(propertyInfo, validationProperty.Rules);
			AddRuleEqualsTo(propertyInfo, validationProperty.Rules);
			AddRuleNotEqualsTo(propertyInfo, validationProperty.Rules);
			AddRuleCustom(propertyInfo, validationProperty.Rules);
			AddRuleRegularExpression(propertyInfo, validationProperty.Rules);
			AddRuleReadOnly(propertyInfo, validationProperty.Rules);

			SetDataType(propertyInfo, validationProperty);

			return validationProperty;
		}

		private string GetJsonPropertyName(PropertyInfo propertyInfo)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(JsonPropertyAttribute)) is JsonPropertyAttribute attJsonProperty)
			{
				return attJsonProperty.PropertyName;
			}

			return propertyInfo.Name.FormatToJsonPropertyName();
		}

		private void SetDataType(PropertyInfo propertyInfo, ValidationPropertyInfo validationProperty)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(DataTypeAttribute)) is DataTypeAttribute attDataType)
			{
				var exit = true;

				switch (attDataType.DataType)
				{
					case DataType.Password:
						SetDataTypeAndRule(validationProperty, "string", "Password");
						
						break;
					case DataType.DateTime:
						SetDataTypeAndRule(validationProperty, "dateTime");
						
						break;
					case DataType.Date:
						SetDataTypeAndRule(validationProperty, "date");
						
						break;
					case DataType.Time:
						SetDataTypeAndRule(validationProperty, "time");
						
						break;
					case DataType.MultilineText:
						SetDataTypeAndRule(validationProperty, "multiline");
						
						break;
					case DataType.EmailAddress:
						SetDataTypeAndRule(validationProperty, "string", "Email");
						
						break;
					case DataType.Url:
						SetDataTypeAndRule(validationProperty, "string", "Url");
						
						break;
					case DataType.Custom when propertyInfo.PropertyType.IsGuid():
						SetDataTypeAndRule(validationProperty, "string", "Guid");
						
						break;
					default:
						exit = false;
						
						break;
				}

				if (exit)
				{
					return;
				}
			}

			var dataType = propertyInfo.PropertyType.ImplementsIEnumerable() ? propertyInfo.PropertyType.GetDataTypeFromIEnumerable() : propertyInfo.PropertyType.GetDataType();
			var dataTypeRules = _dataTypeRules.Where(rule => rule.Check(dataType));
			if (dataTypeRules.Any())
			{
				dataTypeRules.Single().SetDataType(propertyInfo, validationProperty);

				return;
			}

			var dataTypeAttributeRules = _dataTypeAttributeRules.Where(rule => rule.Check(propertyInfo));
			if (dataTypeAttributeRules.Any())
			{
				dataTypeAttributeRules.Single().SetDataType(validationProperty);

				return;
			}

			SetDataTypeAndRule(validationProperty, "string");
		}

		private void AddRuleRequired(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(RequiredAttribute)) is RequiredAttribute)
			{
				rules.Add(new ValidationRule { Rule = "Required" });
			}
		}

		private void AddRuleCustom(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(SpecialValidationAttribute)) is SpecialValidationAttribute)
			{
				rules.Add(new ValidationRule { Rule = "Custom" });
			}
		}

		private void AddRuleRegularExpression(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(RegularExpressionAttribute)) is RegularExpressionAttribute regex)
			{
				rules.Add(new ValidationRule { Rule = "RegularExpression", RegEx = regex.Pattern });
			}
		}

		private void AddRuleStringLength(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (propertyInfo.PropertyType == typeof(string))
			{
				if (Attribute.GetCustomAttribute(propertyInfo, typeof(MaxLengthAttribute)) is MaxLengthAttribute attMaxLength)
				{
					rules.Add(new ValidationRule
					{
						Rule = "MaxLength",
						Value = attMaxLength.Length
					});
				}

				if (Attribute.GetCustomAttribute(propertyInfo, typeof(MinLengthAttribute)) is MinLengthAttribute attMinLength)
				{
					rules.Add(new ValidationRule
					{
						Rule = "MinLength",
						Value = attMinLength.Length
					});
				}
			}
		}

		private void AddRuleRange(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(RangeAttribute)) is RangeAttribute attRange)
			{
				rules.Add(new ValidationRule
				{
					Rule = "Range",
					Minimum = Convert.ToDecimal(attRange.Minimum, CultureInfo.InvariantCulture),
					Maximum = Convert.ToDecimal(attRange.Maximum, CultureInfo.InvariantCulture)
				});
			}
		}

		private void AddRuleRangeFrom(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(RangeFromAttribute)) is RangeFromAttribute attRangeFrom)
			{
				attRangeFrom.PropertyName.Split(',').ToList().ForEach(propertyName =>
				{
					rules.Add(new ValidationRule
					{
						Rule = "RangeFrom",
						PropertyNameFrom = propertyName.Trim(),
						PropertyNameFromAllowNull = attRangeFrom.AllowNull
					});
				});
			}
		}

		private void AddRuleRangeTo(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(RangeToAttribute)) is RangeToAttribute attRangeTo)
			{
				attRangeTo.PropertyName.Split(',').ToList().ForEach(propertyName =>
				{
					rules.Add(new ValidationRule
					{
						Rule = "RangeTo",
						PropertyNameTo = propertyName.Trim(),
						PropertyNameToAllowNull = attRangeTo.AllowNull
					});
				});
			}
		}

		private void AddRuleRangeBetween(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(RangeBetweenAttribute)) is RangeBetweenAttribute attRangeBetween)
			{
				rules.Add(new ValidationRule
				{
					Rule = "RangeBetween",
					PropertyNameFrom = attRangeBetween.PropertyNameFrom,
					PropertyNameTo = attRangeBetween.PropertyNameTo,
					PropertyNameFromAllowNull = propertyInfo.PropertyType.IsDataTypeNullable(),
					PropertyNameToAllowNull = propertyInfo.PropertyType.IsDataTypeNullable()
				});
			}
		}

		private void AddRuleDecimalPlaces(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(DecimalPlacesAttribute)) is DecimalPlacesAttribute attDecimalplaces)
			{
				rules.Add(new ValidationRule
				{
					Rule = "DecimalPlaces",
					Value = attDecimalplaces.DecimalPlaces
				});
			}
			else if (propertyInfo.PropertyType.IsNumber())
			{
				rules.Add(new ValidationRule
				{
					Rule = "DecimalPlaces",
					Value = 0
				});
			}
		}

		private void AddRuleEqualsTo(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(EqualsToAttribute)) is EqualsToAttribute attEqualsTo)
			{
				attEqualsTo.PropertyName.Split(',').ToList().ForEach(propertyName =>
				{
					rules.Add(new ValidationRule
					{
						Rule = "EqualsTo",
						PropertyName = propertyName.Trim()
					});
				});
			}
		}

		private void AddRuleNotEqualsTo(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(NotEqualsToAttribute)) is NotEqualsToAttribute attNotEqualsTo)
			{
				attNotEqualsTo.PropertyName.Split(',').ToList().ForEach(propertyName =>
				{
					rules.Add(new ValidationRule
					{
						Rule = "NotEqualsTo",
						PropertyName = propertyName.Trim(),
						DefaultValue = attNotEqualsTo.DefaultValue?.ToString()
					});
				});
			}
		}

		private void AddRuleReadOnly(PropertyInfo propertyInfo, IList<ValidationRule> rules)
		{
			if (Attribute.GetCustomAttribute(propertyInfo, typeof(ReadOnlyAttribute)) is ReadOnlyAttribute @readonly && @readonly.IsReadOnly)
			{
				rules.Add(new ValidationRule { Rule = "ReadOnly" });
			}
		}

		private static void SetNumberDataType(PropertyInfo propertyInfo, ValidationPropertyInfo validationProperty)
		{
			var dataType = propertyInfo.HasAttribute<DropDownListAttribute>() ? "select" : "number";
			SetDataTypeAndRule(validationProperty, dataType, "Number");
		}

		private static void SetDataTypeAndRule(ValidationPropertyInfo validationProperty, string dataType, string rule = null)
		{
			validationProperty.DataType = dataType;
			if (!rule.IsNullOrEmpty())
			{
				validationProperty.Rules.Add(new ValidationRule { Rule = rule });
			}
		}
	}
}