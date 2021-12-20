using System;
using Eshava.Test.Core.Linq.Enums;

namespace Eshava.Test.Core.Linq.Models
{
	public class DomainModel
	{
		public int Id { get; set; }
		public int? NullableStuff { get; set; }
		public string Name { get; set; }
		public DateTime Date { get; set; }
		public Color Color { get; set; }
		public SubDomainModel Sub { get; set; }

		public string IAmAString { get; set; }
		public int IAmAnInteger { get; set; }
		public decimal IAmAnDecimal { get; set; }
	}
}