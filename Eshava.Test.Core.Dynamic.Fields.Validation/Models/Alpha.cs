using System.Collections.Generic;
using Eshava.Core.Validation.Attributes;

namespace Eshava.Test.Core.Dynamic.Fields.Validation.Models
{
	public class Alpha
	{
		public string Beta { get; set; }
		public string Gamma { get; set; }
		public Omega Delta { get; set; }

		[ValidationIgnore]
		public Alpha Epsilon { get; set; }
		public IEnumerable<DynamicFieldValue> FieldValues { get; set; }
	}
}