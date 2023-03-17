using System.Collections.Generic;
using System.Threading.Tasks;
using Eshava.Core.Communication.Models;
using Eshava.Core.Models;

namespace Eshava.Core.Communication.Ftp.Interfaces
{
	public interface IFTPEngine
	{
		string Type { get; }

		/// <summary>
		/// Upload the specified file
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="fileName">Name of the file on ftp server</param>
		/// <param name="fullFileName">Name of the file, inclusive path, of the file</param>
		/// <returns></returns>
		Task<ResponseData<bool>> UploadAsync(FTPSettings settings, string fileName, string fullFileName);

		/// <summary>
		/// Download the specified file
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="fileName">Name of the file on ftp server</param>
		/// <param name="targetPath">Path of the target directory on local file system</param>
		/// <returns></returns>
		Task<ResponseData<bool>> DownloadAsync(FTPSettings settings, string fileName, string targetPath);

		/// <summary>
		/// Get all file names, inclusive path, of the directory on ftp server
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="recursive">Enumerate sub directories</param>
		/// <returns></returns>
		Task<ResponseData<IEnumerable<string>>> GetFileNamesAsync(FTPSettings settings, bool recursive);

		/// <summary>
		/// Get all file names, inclusive path, of the directory on ftp server
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="recursive">Enumerate sub directories</param>
		/// <returns></returns>
		Task<ResponseData<bool>> DeleteAsync(FTPSettings settings, string directoryOrFileName);
	}
}