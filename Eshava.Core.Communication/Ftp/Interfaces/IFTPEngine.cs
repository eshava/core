using System.Collections.Generic;
using System.Threading.Tasks;
using Eshava.Core.Communication.Models;

namespace Eshava.Core.Communication.Ftp.Interfaces
{
	public interface IFTPEngine
	{
		string Type { get; }
		Task<bool> UploadAsync(FTPSettings settings, string fileName, string fullFileName);
		Task<bool> DownloadAsync(FTPSettings settings, string fileName, string targetPath);
		Task<IEnumerable<string>> GetFileNamesAsync(FTPSettings settings, bool recursive);
	}
}