using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Eshava.Test.Core.Validation.Models
{
	internal class ComplexData
	{
		public const string EPSILONFORMAT = @"^([a-zA-Z]{1})([\-]?[a-zA-Z\d]+)*$";

		public IEnumerable<string> Alpha { get; set; }
		public IList<int> Beta { get; set; }
		public List<BasicRules> Gamma { get; set; }
		public BasicRules Delta { get; set; }

		[RegularExpression(EPSILONFORMAT)]
		public string Epsilon { get; set; }
	}
}