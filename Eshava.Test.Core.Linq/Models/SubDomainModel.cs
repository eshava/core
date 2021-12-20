namespace Eshava.Test.Core.Linq.Models
{
	public class SubDomainModel
	{
		public int SubId { get; set; }
		public string SubName { get; set; }
		public int? NullableStuff { get; set; }
		public SubSubDomainModel Sub { get; set; }
	}
}