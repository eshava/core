namespace Eshava.Core.Communication.Models
{
	public class EmailAttachmentData
	{
		public string FileName { get; set; }
		public string ContentType{ get; set; }
		public byte[] Data { get; set; }
	}
}