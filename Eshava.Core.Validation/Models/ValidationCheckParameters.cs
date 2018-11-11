using System;
using System.Reflection;

namespace Eshava.Core.Validation.Models
{
	internal class ValidationCheckParameters
	{
		public Type DataType { get; set; }
		public object Model { get; set; }
		public PropertyInfo PropertyInfo { get; set; }
		public object PropertyValue { get; set; }
		public bool NotEquals { get; set; }
		public bool AllowNull { get; set; }
	}
}