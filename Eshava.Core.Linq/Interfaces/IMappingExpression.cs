using System;
using System.Linq.Expressions;

namespace Eshava.Core.Linq.Interfaces
{
	internal interface IMappingExpression
	{
		public Type SourceType { get; }
		public Type TargetType { get; }

		(bool HasMapping, Expression Expression) GetMapping(Expression sourceExpression, ParameterExpression parameterExpression);
	}


	public interface IMappingExpression<Source, Target>
	{
		/// <summary>
		/// Create a expression mapping from the destination to the source expression. This allows for two-way mapping.
		/// </summary>
		/// <returns></returns>
		IMappingExpression<Target, Source> ReverseMap();

		/// <summary>
		///  Customize configuration for a target path expression.
		/// </summary>
		/// <typeparam name="Source"></typeparam>
		/// <typeparam name="Target"></typeparam>
		IMappingExpression<Source, Target> ForPath<SourceMember, TargetMember>(Expression<Func<Source, SourceMember>> sourceMember, Expression<Func<Target, TargetMember>> targetMember);
	}
}