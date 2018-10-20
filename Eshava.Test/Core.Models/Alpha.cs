using System.Collections.Generic;
using Eshava.Test.Core.Models.Interfaces;

namespace Eshava.Test.Core.Models
{
	public class Alpha : IAlpha
	{
		public Alpha()
		{
			
		}

		public Alpha(int beta, string gamma)
		{
			Beta = beta;
			Gamma = gamma;
		}

		public int Beta { get; set; }
		public int? BetaNullable { get; set; }
		public string Gamma { get; set; }
		public string Delta { get; set; }
		public string Epsilon { get; set; }
		public List<string> Zeta { get; set; }
		public List<string> Eta { get; set; }
		public List<int> Theta { get; set; }
		public List<Omega> Iota { get; set; }
		public Omega Kappa { get; set; }
	}
}