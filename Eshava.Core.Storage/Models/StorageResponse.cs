using System;

namespace Eshava.Core.Storage.Models
{
	public class StorageResponse<T>
	{
		public bool IsFaulty { get; set; }
		public T Data { get; set; }
		public Exception Exception { get; set; }
	}
}