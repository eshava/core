﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshava.Core.Dynamic.Fields.Enums;
using Eshava.Core.Dynamic.Fields.Interfaces;
using Eshava.Core.Dynamic.Fields.Models;
using Eshava.Core.Dynamic.Fields.Validation.Interfaces;
using Eshava.Core.Extensions;
using Eshava.Core.Validation.Attributes;
using Eshava.Core.Validation.Extension;

namespace Eshava.Core.Dynamic.Fields.Validation
{
	public class FieldAnalyzer<FD, FA, FV, T, D> : IFieldAnalyzer<FD, FA, FV, T, D> where FD : IFieldDefinition<T> where FA : IFieldAssignment<T, D> where FV : IFieldValue<T>
	{
		private static List<(Func<FieldType, bool> Check, Func<IFieldValue<T>, BaseField> Convert)> _mapping =
			new List<(Func<FieldType, bool> Check, Func<IFieldValue<T>, BaseField> Convert)>
			{
				(type => type == FieldType.NumberInteger, field => new BaseField { Id = field.Id.ToString(), Type = typeof(int), Value = field.ValueInteger }),
				(type => type == FieldType.ComboBoxInteger, field => new BaseField { Id = field.Id.ToString(), Type = typeof(int), Value = field.ValueInteger }),
				(type => type == FieldType.NumberLong, field => new BaseField { Id = field.Id.ToString(), Type = typeof(long), Value = field.ValueLong }),
				(type => type == FieldType.NumberDouble, field => new BaseField { Id = field.Id.ToString(), Type = typeof(double), Value = field.ValueDouble }),
				(type => type == FieldType.NumberFloat, field => new BaseField { Id = field.Id.ToString(), Type = typeof(float), Value = field.ValueFloat }),
				(type => type == FieldType.NumberDecimal, field => new BaseField { Id = field.Id.ToString(), Type = typeof(decimal), Value = field.ValueDecimal }),
				(type => type == FieldType.Text, field => new BaseField { Id = field.Id.ToString(), Type = typeof(string), Value = field.ValueString }),
				(type => type == FieldType.TextMultiline, field => new BaseField { Id = field.Id.ToString(), Type = typeof(string), Value = field.ValueString }),
				(type => type == FieldType.AutoComplete, field => new BaseField { Id = field.Id.ToString(), Type = typeof(string), Value = field.ValueString }),
				(type => type == FieldType.DateTime, field => new BaseField { Id = field.Id.ToString(), Type = typeof(DateTime), Value = field.ValueDateTime }),
				(type => type == FieldType.Checkbox, field => new BaseField { Id = field.Id.ToString(), Type = typeof(bool), Value = field.ValueBoolean }),
				(type => type == FieldType.Guid, field => new BaseField { Id = field.Id.ToString(), Type = typeof(Guid), Value = field.ValueGuid }),
				(type => type == FieldType.ComboxBoxGuid, field => new BaseField { Id = field.Id.ToString(), Type = typeof(Guid), Value = field.ValueGuid })
			};

		public AnalysisResult Analyse(object model, FieldInformation<FD, FA, FV, T, D> fieldInformation)
		{
			var result = AddFieldAssignments(fieldInformation);
			result = Analyse(model, result);

			return new AnalysisResult(result);
		}

		private Dictionary<string, BaseField> AddFieldAssignments(FieldInformation<FD, FA, FV, T, D> fieldInformation)
		{
			var result = new Dictionary<string, BaseField>();

			if (!(fieldInformation?.IsValid ?? false))
			{
				return result;
			}

			foreach (var fieldAssignment in fieldInformation.Assignments)
			{
				if (result.ContainsKey(fieldAssignment.Id.ToString()))
				{
					continue;
				}

				var fieldValue = fieldInformation.Values.FirstOrDefault(f => Equals(f.AssignmentId, fieldAssignment.Id));
				var fieldDefinition = fieldInformation.Definitions.FirstOrDefault(f => Equals(f.Id, fieldAssignment.DefinitionId));

				if (fieldDefinition == null || fieldValue == null)
				{
					continue;
				}

				var mapping = _mapping.Single(m => m.Check(fieldDefinition.FieldType));
				result.Add(fieldAssignment.Id.ToString(), mapping.Convert(fieldValue));
			}

			return result;
		}

		private Dictionary<string, BaseField> Analyse(object model, Dictionary<string, BaseField> result)
		{
			if (model == null)
			{
				return result;
			}

			var modelType = model.GetType();
			foreach (var propertyInfo in modelType.GetProperties())
			{
				result = AnalyseProperty(model, propertyInfo, result);
			}

			return result;
		}

		private Dictionary<string, BaseField> AnalyseProperty(object model, PropertyInfo propertyInfo, Dictionary<string, BaseField> result)
		{
			if (propertyInfo.PropertyType.ImplementsIEnumerable() || propertyInfo.HasAttribute<ValidationIgnoreAttribute>())
			{
				return result;
			}

			var value = propertyInfo.GetValue(model);
			if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
			{
				return value == null ? result : Analyse(value, result);
			}

			if (!result.ContainsKey(propertyInfo.Name))
			{
				result.Add(propertyInfo.Name, new BaseField { Id = propertyInfo.Name, Type = propertyInfo.GetDataType(), Value = value });
			}

			return result;
		}
	}
}