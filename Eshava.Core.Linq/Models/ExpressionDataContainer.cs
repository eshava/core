using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Eshava.Core.Linq.Enums;

namespace Eshava.Core.Linq.Models
{
	internal class ExpressionDataContainer
	{
		public PropertyInfo PropertyInfo { get; set; }
		public MemberExpression Member { get; set; }
		public ConstantExpression ConstantValue { get; set; }
		public ParameterExpression Parameter { get; set; }
		public CompareOperator Operator { get; set; }
	}
}
