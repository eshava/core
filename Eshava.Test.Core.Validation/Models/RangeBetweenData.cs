using System;
using Eshava.Core.Validation.Attributes;

namespace Eshava.Test.Core.Validation.Models
{
	public class RangeBetweenData
	{

		public decimal? AlphaFrom { get; set; }
		[RangeBetween(nameof(AlphaFrom), nameof(AlphaTo))]
		public decimal? AlphaValue { get; set; }
		public decimal? AlphaTo { get; set; }

		public double? BetaFrom { get; set; }
		[RangeBetween(nameof(BetaFrom), nameof(BetaTo))]
		public double? BetaValue { get; set; }
		public double? BetaTo { get; set; }

		public float? GammaFrom { get; set; }
		[RangeBetween(nameof(GammaFrom), nameof(GammaTo))]
		public float? GammaValue { get; set; }
		public float? GammaTo { get; set; }

		public int? DeltaFrom { get; set; }
		[RangeBetween(nameof(DeltaFrom), nameof(DeltaTo))]
		public int? DeltaValue { get; set; }
		public int? DeltaTo { get; set; }

		public long? EpsilonFrom { get; set; }
		[RangeBetween(nameof(EpsilonFrom), nameof(EpsilonTo))]
		public long? EpsilonValue { get; set; }
		public long? EpsilonTo { get; set; }

		public DateTime? ZetaFrom { get; set; }
		[RangeBetween(nameof(ZetaFrom), nameof(ZetaTo))]
		public DateTime? ZetaValue { get; set; }
		public DateTime? ZetaTo { get; set; }
	}
}