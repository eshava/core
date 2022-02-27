using System;
using Eshava.Core.Linq.Enums;

namespace Eshava.Core.Linq.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class AllowedCompareOperatorAttribute : Attribute
	{
		public AllowedCompareOperatorAttribute(CompareOperator compareOperator)
		{
			CompareOperator = compareOperator;
		}

		public CompareOperator CompareOperator { get; }
	}
}