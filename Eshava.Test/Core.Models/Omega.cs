using System.ComponentModel.DataAnnotations;

namespace Eshava.Test.Core.Models
{
	public class Omega
	{
		public string Psi { get; set; }
		[MaxLength(20)]
		public string Chi { get; set; }
	}
}