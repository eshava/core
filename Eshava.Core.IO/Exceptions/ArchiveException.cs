using System;

namespace Eshava.Core.IO.Exceptions
{
	public class ArchiveException : Exception
	{
		public ArchiveException(string message, string archiveType, Exception innerException) : base(message, innerException)
		{
			ArchiveType = archiveType;
		}

		public string ArchiveType { get; }
	}
}