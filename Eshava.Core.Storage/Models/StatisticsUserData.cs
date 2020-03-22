using System;

namespace Eshava.Core.Storage.Models
{
	public class StatisticsUserData
	{
		public string Name { get; set; }
		public string LoginName { get; set; }
		public string Database { get; set; }
		public DateTime? Created { get; set; }
		public bool Disabled { get; set; }
	}
}