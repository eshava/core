namespace Eshava.Test.Core.Linq.Models
{
	public class SubDataModel
	{
		public int? SubId { get; set; }
		public string SubName { get; set; }
		public int? NullableStuff { get; set; }
		public SubSubDataModel Sub { get; set; }
	}
}