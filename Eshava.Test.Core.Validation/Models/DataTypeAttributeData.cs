using System;
using System.ComponentModel.DataAnnotations;

namespace Eshava.Test.Core.Validation.Models
{
	internal class DataTypeAttributeData
	{
		[DataType(DataType.Password)]
		public string Alpha { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime Beta { get; set; }

		[DataType(DataType.Date)]
		public DateTime Gamma { get; set; }

		[DataType(DataType.Time)]
		public DateTime Delta { get; set; }

		[DataType(DataType.MultilineText)]
		public string Epsilon { get; set; }

		[DataType(DataType.EmailAddress)]
		public string Zeta { get; set; }

		[DataType(DataType.Url)]
		public string Eta { get; set; }

		[DataType(DataType.Custom)]
		public Guid Theta { get; set; }

		[DataType(DataType.Custom)]
		public DateTime Iota { get; set; }
	}
}