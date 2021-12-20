using System;
using System.Linq.Expressions;

namespace Eshava.Core.Linq.Interfaces
{
	public interface ITransformQueryEngine
	{
		Expression<Func<Target, bool>> Transform<Source, Target>(Expression<Func<Source, bool>> expression);
		(MemberExpression Member, ParameterExpression Parameter) TransformMemberExpression<Source, Target>(MemberExpression memberExpression);
	}
}