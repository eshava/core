using System;
using System.ComponentModel.DataAnnotations;
using Eshava.Core.Validation.Attributes;

namespace Eshava.Test.Core.Validation.Models
{
	internal class DataTypeProperties
	{
		[Range(-5, 7)]
		public int? Alpha { get; set; }

		[RangeTo(nameof(Gamma), true)]
		public long? Beta { get; set; }

		[RangeFrom(nameof(Beta), false)]
		public double? Gamma { get; set; }

		[RangeBetween(nameof(Beta), nameof(Gamma))]
		public float? Delta { get; set; }

		[DecimalPlaces(3)]
		public decimal? Epsilon { get; set; }

		public DateTime? Zeta { get; set; }

		public Guid? Eta { get; set; }

		public bool? Theta { get; set; }

		public Alphabet Iota { get; set; }

		[Tags]
		public string Kappa { get; set; }

		[Typeahead]
		public string Lambda { get; set; }

		[DropDownList]
		public string My { get; set; }

		[DropDownList]
		public int? Ny { get; set; }

		[DecimalPlaces(3)]
		[Range(typeof(decimal), "0.001", "1.001")]
		public decimal? Xi { get; set; }
	}
}