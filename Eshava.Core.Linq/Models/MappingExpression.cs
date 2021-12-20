using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Eshava.Core.Linq.Extensions;
using Eshava.Core.Linq.Interfaces;

namespace Eshava.Core.Linq.Models
{
	internal class MappingExpression<Source, Target> : IMappingExpression<Source, Target>, IMappingExpression
	{
		private Type _sourceType = typeof(Source);
		private Type _targetType = typeof(Target);
		private readonly List<MappingExpressionItem> _expressions = new List<MappingExpressionItem>();

		public MappingExpression()
		{
			_expressions = new List<MappingExpressionItem>();
		}

		private MappingExpression(List<MappingExpressionItem> expressions)
		{
			_expressions = expressions;
		}

		public Type SourceType { get => _sourceType; }
		public Type TargetType { get => _targetType; }

		public IMappingExpression<Source, Target> ForPath<SourceMember, TargetMember>(Expression<Func<Source, SourceMember>> sourceMember, Expression<Func<Target, TargetMember>> targetMember)
		{
			_expressions.Add(new MappingExpressionItem(sourceMember.Body, targetMember.Body));

			return this;
		}

		public IMappingExpression<Target, Source> ReverseMap()
		{
			var expressions = _expressions.Select(e => new MappingExpressionItem(e.Target, e.Source)).ToList();

			var mappingExpression = new MappingExpression<Target, Source>(expressions);

			MappingStore.Mappings.Add(mappingExpression);

			return mappingExpression;
		}

		public (bool HasMapping, Expression Expression) GetMapping(Expression sourceExpression, ParameterExpression parameterExpression)
		{
			if (sourceExpression is ParameterExpression)
			{
				if (sourceExpression.Type != _sourceType)
				{
					return (false, null);
				}

				var expressionItem = _expressions.FirstOrDefault(e => e.IsSourceParameterExpression);
				if (expressionItem == null)
				{
					return (false, null);
				}

				return (true, ChangeParameterExpression(expressionItem.Target, parameterExpression));
			}

			if (sourceExpression is MemberExpression)
			{
				var sourceExpressionString = sourceExpression.ToString();
				sourceExpressionString = sourceExpressionString.Substring(sourceExpressionString.IndexOf(".") + 1);
				
				var expressionItem = _expressions.FirstOrDefault(e => e.SourceString == sourceExpressionString);
				if (expressionItem == null)
				{
					return (false, null);
				}

				return (true, ChangeParameterExpression(expressionItem.Target, parameterExpression));
			}

			return (false, null);
		}

		private Expression ChangeParameterExpression(Expression expression, ParameterExpression parameterExpression)
		{
			if (expression is ParameterExpression)
			{
				return expression.ChangeParameterExpression(parameterExpression);
			}

			if (expression is MemberExpression)
			{
				var m = expression as MemberExpression;
				var parent = ChangeParameterExpression(m.Expression, parameterExpression);

				var targetPropertyInfo = default(PropertyInfo);
				if (parent is ParameterExpression)
				{
					targetPropertyInfo = _targetType.GetProperty(m.Member.Name);

					return Expression.MakeMemberAccess(parameterExpression, targetPropertyInfo);
				}

				targetPropertyInfo = parent.Type.GetProperties().FirstOrDefault(p => p.Name == m.Member.Name);

				return Expression.MakeMemberAccess(parent, targetPropertyInfo);
			}

			return expression;
		}
	}	
}