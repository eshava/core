using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Eshava.Core.Communication.Ftp.Interfaces;
using Eshava.Core.Communication.Models;
using Eshava.Core.Extensions;

namespace Eshava.Core.Communication.Ftp
{
	public class WebRequestFTPEngine : IFTPEngine
	{
		public string Type => "System.Net.WebRequest";

		/// <summary>
		/// Download the specified file
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="fileName">Name of the file on ftp server</param>
		/// <param name="targetPath">Path of the target directory on local file system</param>
		/// <returns></returns>
		public async Task<bool> DownloadAsync(FTPSettings settings, string fileName, string targetPath)
		{
			var directoryInfo = new DirectoryInfo(targetPath);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}

			var request = CreateRequest(settings, fileName, WebRequestMethods.Ftp.DownloadFile);
			var response = await request.GetResponseAsync();

			if (!(response is FtpWebResponse ftpWebResponse))
			{
				throw new NotSupportedException($"Unexpected request response. Expected: {nameof(FtpWebResponse)}, but found {response.GetType()}");
			}

			var responseStream = ftpWebResponse.GetResponseStream();

			using (var fsStream = File.Open(Path.Combine(targetPath, fileName), FileMode.Create, FileAccess.Write, FileShare.None))
			{
				await WriteAsync(responseStream, fsStream);
				fsStream.Close();
			}

			return ftpWebResponse.StatusCode == FtpStatusCode.ClosingData;
		}

		/// <summary>
		/// Upload the specified file
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="fileName">Name of the file on ftp server</param>
		/// <param name="fullFileName">Name of the file, inclusive path, of the file</param>
		/// <returns></returns>
		public async Task<bool> UploadAsync(FTPSettings settings, string fileName, string fullFileName)
		{
			var request = CreateRequest(settings, fileName, WebRequestMethods.Ftp.UploadFile);
			var ftpStream = await request.GetRequestStreamAsync();
			var fsStream = File.OpenRead(fullFileName);

			await WriteAsync(fsStream, ftpStream);

			fsStream.Close();

			var response = (FtpWebResponse)request.GetResponse();

			ftpStream.Close();

			return response.StatusCode == FtpStatusCode.ClosingData;
		}

		/// <summary>
		/// Delete the specified directory or file from ftp server
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="directoryOrFileName">Name of the directory or file</param>
		/// <returns></returns>
		public async Task<bool> DeleteAsync(FTPSettings settings, string directoryOrFileName)
		{
			var request = CreateRequest(settings, directoryOrFileName, WebRequestMethods.Ftp.DeleteFile);
			var response = await request.GetResponseAsync();

			response.Close();

			if (!(response is FtpWebResponse ftpWebResponse))
			{
				throw new NotSupportedException($"Unexpected request response. Expected: {nameof(FtpWebResponse)}, but found {response.GetType()}");
			}

			return true;
		}

		/// <summary>
		/// Get all file names, inclusive path, of the directory on ftp server
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="recursive">Enumerate sub directories</param>
		/// <returns></returns>
		public async Task<IEnumerable<string>> GetFileNamesAsync(FTPSettings settings, bool recursive)
		{
			var request = CreateRequest(settings, "", WebRequestMethods.Ftp.ListDirectoryDetails);
			var response = await request.GetResponseAsync();

			if (!(response is FtpWebResponse ftpWebResponse))
			{
				throw new NotSupportedException($"Unexpected request response. Expected: {nameof(FtpWebResponse)}, but found {response.GetType()}");
			}

			var responseStream = ftpWebResponse.GetResponseStream();
			var reader = new StreamReader(responseStream);
			var fileNames = await ProcessFileInformationResultsAsync(
				reader.ReadToEnd(),
				recursive, settings.ServerPath,
				async newServerPath =>
				{
					var subFileSettigs = new FTPSettings
					{
						ServerUrl = settings.ServerUrl,
						ServerPort = settings.ServerPort,
						ServerPath = newServerPath,
						Username = settings.Username,
						Password = settings.Password
					};

					return await GetFileNamesAsync(subFileSettigs, true);
				});

			return fileNames.OrderBy(f => f).ToList();
		}

		private async Task<IEnumerable<string>> ProcessFileInformationResultsAsync(string responseString, bool recursive, string servicePath, Func<string, Task<IEnumerable<string>>> readFileNames)
		{
			var fileInformationResults = responseString.Replace("\r", "").TrimEnd('\n').Split('\n');
			var fileNames = new List<string>();

			foreach (var fileInformation in fileInformationResults)
			{
				var fileName = fileInformation.Substring(55);
				if (fileInformation.StartsWith("d"))
				{
					if (fileName == ".." || !recursive)
					{
						continue;
					}

					var subServerPath = servicePath + "/" + fileName;
					var subFiles = await readFileNames(subServerPath);
					if (subFiles.Any())
					{
						fileNames.AddRange(subFiles.Select(f => $"{subServerPath}/{f}".TrimStart('/')));
					}
				}
				else
				{
					fileNames.Add(fileName);
				}
			}

			return fileNames;
		}

		private FtpWebRequest CreateRequest(FTPSettings settings, string fileName, string method)
		{
			if (settings.ServerPort <= 0)
			{
				settings.ServerPort = 21;
			}

			if (!settings.ServerPath.IsNullOrEmpty() && !settings.ServerPath.EndsWith("/"))
			{
				settings.ServerPath += '/';
			}

			if (!settings.ServerUrl.ToLower().StartsWith("ftp://"))
			{
				settings.ServerUrl = "ftp://" + settings.ServerUrl;
			}

			if (settings.ServerUrl.EndsWith("/"))
			{
				settings.ServerUrl = settings.ServerUrl.Substring(0, settings.ServerUrl.Length - 1);
			}

			var request = (FtpWebRequest)WebRequest.Create(new Uri($"{settings.ServerUrl}:{settings.ServerPort}/{settings.ServerPath ?? ""}{fileName}"));
			request.Method = method;
			request.Credentials = new NetworkCredential(settings.Username, settings.Password);

			return request;
		}

		private async Task WriteAsync(Stream source, Stream target)
		{
			var buffer = new byte[1024];
			int byteRead;

			do
			{
				byteRead = source.Read(buffer, 0, 1024);
				await target.WriteAsync(buffer, 0, byteRead);
			}
			while (byteRead != 0);
		}
	}
}