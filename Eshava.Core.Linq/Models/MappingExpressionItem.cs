using System.Linq.Expressions;

namespace Eshava.Core.Linq.Models
{
	internal class MappingExpressionItem
	{
		public MappingExpressionItem(Expression source, Expression target)
		{
			Source = source;
			Target = target;

			SourceString = source.ToString();
			SourceString = SourceString.Substring(SourceString.IndexOf(".") + 1);

			IsSourceParameterExpression = Source is ParameterExpression;
			IsSourceMemberExpression = Source is MemberExpression;

			IsTargetParameterExpression = Target is ParameterExpression;
			IsTargetMemberExpression = Target is MemberExpression;
		}

		public string SourceString { get; }
		public Expression Source { get; }
		public Expression Target { get; }

		public bool IsSourceParameterExpression { get; set; }
		public bool IsSourceMemberExpression { get; set; }

		public bool IsTargetParameterExpression { get; set; }
		public bool IsTargetMemberExpression { get; set; }
	}
}