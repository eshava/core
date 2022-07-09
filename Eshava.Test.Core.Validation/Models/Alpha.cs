using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eshava.Core.Validation.Attributes;

namespace Eshava.Test.Core.Validation.Models
{
	public class Alpha
	{
		public const string OMEGAREGEXFORMAT = @"^([a-zA-Z]{1})([\-]?[a-zA-Z\d]+)*$";

		public Alpha()
		{
			
		}

		public Alpha(int beta, string gamma)
		{
			Beta = beta;
			Gamma = gamma;
		}

		public Alpha(int beta, string gamma, string phi)
		{
			Beta = beta;
			Gamma = gamma;
			Phi = phi;
		}

		public int Beta { get; set; }
		public int? BetaNullable { get; set; }

		[EqualsTo("Delta")]
		[NotEqualsTo("Epsilon")]
		[Required]
		[MaxLength(10), MinLength(5)]
		public string Gamma { get; set; }

		[EqualsTo("Gamma")]
		[NotEqualsTo("Epsilon,EpsilonTwo")]
		[Unique]
		[MaxLength(10), MinLength(5)]
		public string Delta { get; set; }

		[NotEqualsTo("EpsilonTwo", "Alpha")]
		public string DeltaTwo { get; set; }

		[DataType(DataType.EmailAddress)]
		public string DeltaMail { get; set; }

		[DataType(DataType.Url)]
		public string DeltaUrl { get; set; }

		public string Epsilon { get; set; }

		public string EpsilonTwo { get; set; }

		public List<string> Zeta { get; set; }

		public List<string> Eta { get; set; }

		public List<int> Theta { get; set; }

		public List<Omega> Iota { get; set; }

		public Omega Kappa { get; set; }

		[Required]
		[Range(0, 15)]
		public int Lambda { get; set; }

		[Required]
		[Unique]
		public int? LambdaNullable { get; set; }

		[Range(0, 15)]
		public long LambdaLong { get; set; }

		[Required]
		[Unique]
		public long? LambdaLongNullable { get; set; }

		[DecimalPlaces(4)]
		[Range(-40.5, 70.5)]
		public decimal My { get; set; }

		[RangeTo("MyNullableTwo", false)]
		public decimal? MyNullableOne { get; set; }

		[RangeFrom("MyNullableOne", false)]
		public decimal? MyNullableTwo { get; set; }

		[RangeTo("MyNullableFive", true)]
		public decimal? MyNullableThree { get; set; }

		[RangeTo("MyNullableFive", true)]
		public decimal? MyNullableFour { get; set; }

		[RangeFrom("MyNullableThree,MyNullableFour", true)]
		public decimal? MyNullableFive { get; set; }

		[RangeBetween("MyNullableOne", "MyNullableTwo")]
		public decimal? MyNullableSix { get; set; }

		[Required]
		[Range(0.25, 15.75)]
		public float Ny { get; set; }
		public float NyNullable { get; set; }

		[DecimalPlaces(-2)]
		[Range(-40.0, 70.0)]
		public double Xi { get; set; }
		public double XiNullable { get; set; }

		[Range(1, 3)]
		public Alphabet Omikron { get; set; }

		[NotEqualsTo("Rho", 7)]
		public int Pi { get; set; }

		[NotEqualsTo("Pi", 7)]
		public int Rho { get; set; }

		[Required]
		public List<int> Sigma { get; set; }

		public Omega Tau { get; set; }
		public List<Omega> TauIEnumerable { get; set; }

		[MaxLength(10), MinLength(5)]
		public List<string> Ypsilon { get; set; }

		[MaxLength(5)]
		public string Phi { get; }

		public Guid? Chi { get; set; }

		[Required]
		public Guid ChiNotNull { get; set; }
		[Required]
		public Guid? ChiNullable { get; set; }

		public DateTime? Psi { get; set; }
		[Required]
		public DateTime PsiRequired { get; set; }
		[Required]
		public DateTime? PsiRequiredNullable { get; set; }

		[RangeTo("OmegaFloatTo", false)]
		public float OmegaFloatFrom { get; set; }
		[RangeFrom("OmegaFloatFrom", false)]
		public float OmegaFloatTo { get; set; }
		[RangeBetween("OmegaFloatFrom", "OmegaFloatTo")]
		public float OmegaFloat { get; set; }

		[RangeTo("OmegaIntegerTo", false)]
		public int OmegaIntegerFrom { get; set; }
		[RangeFrom("OmegaIntegerFrom", false)]
		public int OmegaIntegerTo { get; set; }
		[RangeBetween("OmegaIntegerFrom", "OmegaIntegerTo")]
		public int OmegaInteger { get; set; }

		[RangeTo("OmegaLongTo", false)]
		public long OmegaLongFrom { get; set; }
		[RangeFrom("OmegaLongFrom", false)]
		public long OmegaLongTo { get; set; }
		[RangeBetween("OmegaLongFrom", "OmegaLongTo")]
		public long OmegaLong { get; set; }

		[RangeTo("OmegaDoubleTo", false)]
		public double OmegaDoubleFrom { get; set; }
		[RangeFrom("OmegaDoubleFrom", false)]
		public double OmegaDoubleTo { get; set; }
		[RangeBetween("OmegaDoubleFrom", "OmegaDoubleTo")]
		public double OmegaDouble { get; set; }
		
		[RangeTo("OmegaDateTimeTo", false)]
		public DateTime OmegaDateTimeFrom { get; set; }
		[RangeFrom("OmegaDateTimeFrom", false)]
		public DateTime OmegaDateTimeTo { get; set; }
		[RangeBetween("OmegaDateTimeFrom", "OmegaDateTimeTo")]
		public DateTime OmegaDateTime { get; set; }

		[EqualsTo("OmegaIntegerEqualTwo")]
		public int OmegaIntegerEqualOne { get; set; }
		[EqualsTo("OmegaIntegerEqualOne")]
		public int OmegaIntegerEqualTwo { get; set; }
		[NotEqualsTo("OmegaIntegerEqualTwo")]
		public int OmegaIntegerNotEqual { get; set; }

		[EqualsTo("OmegaLongEqualTwo")]
		public long OmegaLongEqualOne { get; set; }
		[EqualsTo("OmegaLongEqualOne")]
		public long OmegaLongEqualTwo { get; set; }
		[NotEqualsTo("OmegaLongEqualTwo")]
		public long OmegaLongNotEqual { get; set; }

		public bool StigmaOne { get; set; }
		public bool? StigmaTwo { get; set; }

		[Required]
		[ValidationIgnore]
		public string StigmaThree { get; set; }

		[RegularExpression(OMEGAREGEXFORMAT)]
		public string OmegaRegEx { get; set; }
	}
}