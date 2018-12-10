using System.ComponentModel.DataAnnotations;

namespace Eshava.Test.Core.Validation.Models
{
	public class Omega
	{
		[Required]
		public string Psi { get; set; }

		[MaxLength(20)]
		public string Chi { get; set; }
		public int? Sigma { get; set; }
	}
}