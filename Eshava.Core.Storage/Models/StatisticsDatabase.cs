using System;

namespace Eshava.Core.Storage.Models
{
	public class StatisticsDatabase
	{
		public string Database { get; set; }
		public DateTime? Created { get; set; }
		public string Status { get; set; }
		public DateTime? LastBackup { get; set; }
	}
}