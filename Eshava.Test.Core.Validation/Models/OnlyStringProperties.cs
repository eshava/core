using System.ComponentModel.DataAnnotations;
using Eshava.Core.Validation.Attributes;

namespace Eshava.Test.Core.Validation.Models
{
	internal class OnlyStringProperties
	{
		public string Alpha { get; set; }

		[MinLength(2)]
		public string Beta { get; set; }

		[MaxLength(10)]
		public string Gamma { get; set; }

		[MinLength(2)]
		[MaxLength(10)]
		public string Delta { get; set; }

		[EqualsTo("Zeta, Eta")]
		public string Epsilon { get; set; }

		[EqualsTo("Eta")]
		public string Zeta { get; set; }

		[EqualsTo("Zeta")]
		[NotEqualsTo("Theta,Iota", "Omega")]
		public string Eta { get; set; }

		[NotEqualsTo("Iota", "Omega")]
		public string Theta { get; set; }

		[NotEqualsTo("Theta", "Omega")]
		public string Iota { get; set; }
	}
}