using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Eshava.Core.Validation.Attributes;

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

		[ReadOnly(true)]
		public string Zeta { get; set; }

		[ReadOnly(false)]
		public string Eta { get; set; }

		[Enumeration(invalidateZero: true)]
		public Color PrimaryColor { get; set; }

		[Enumeration(skipValidation: true)]
		public Color SecondaryColor { get; set; }

		[Required]
		[Enumeration]
		public Color? BaseColor { get; set; }
	}
}