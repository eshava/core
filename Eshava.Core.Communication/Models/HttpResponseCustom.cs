namespace Eshava.Core.Communication.Models
{
	public class HttpResponseCustom<T>
	{
		public bool Successful { get; set; }
		public T Result { get; set; }
	}
}