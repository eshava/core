using System.Collections.Generic;

namespace Eshava.Test.Core.Validation.Models
{
	internal class ComplexData
	{
		public IEnumerable<string> Alpha { get; set; }
		public IList<int> Beta { get; set; }
		public List<BasicRules> Gamma { get; set; }
		public BasicRules Delta { get; set; }
	}
}