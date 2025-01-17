using System;
using System.Xml;
using Eshava.Core.Logging.Models;
using Microsoft.Extensions.Logging;

namespace Eshava.Test.Core.Logging.Models
{
	public class Alpha
	{
		public int Beta { get; set; }
		public string Gamma { get; set; }
		public LogLevel Delta { get; set; }
		public LogInformationDto Epsilon { get; set; }
		public long Zeta { get; set; }
		public decimal Eta { get; set; }
		public float Theta { get; set; }
		public double Iota { get; set; }
		public Guid Kappa { get; set; }
		public DateTime Lambda{ get; set; }
		public XmlDocument My { get; set; }
		public byte[] Ny { get; set; }
		public bool Xi { get; set; }
	}
}