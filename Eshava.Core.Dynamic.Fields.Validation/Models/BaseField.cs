using System;

namespace Eshava.Core.Dynamic.Fields.Models
{
	public class BaseField
	{
		public string Id { get; set; }
		public Type Type { get; set; }
		public object Value { get; set; }
	}
}