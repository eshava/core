using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Eshava.Core.Communication.Ftp.Interfaces;
using Eshava.Core.Communication.Models;
using Eshava.Core.Extensions;
using Renci.SshNet;

namespace Eshava.Core.Communication.Ftp
{
	public class SshNetFTPEngine : IFTPEngine
	{
		public string Type => "SSH.NET.FTP";

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

			var connectionInfo = CreatConnectionInfo(settings);

			using (var sftp = new SftpClient(connectionInfo))
			{
				sftp.Connect();

				ChangeServerPath(sftp, settings.ServerPath);

				using (var saveFile = File.OpenWrite(Path.Combine(targetPath, fileName)))
				{
					await Task.Factory.FromAsync(sftp.BeginDownloadFile(fileName, saveFile), sftp.EndDownloadFile);
				}

				sftp.Disconnect();
			}

			return true;
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
			var connectionInfo = CreatConnectionInfo(settings);

			using (var sftp = new SftpClient(connectionInfo))
			{
				sftp.Connect();

				ChangeServerPath(sftp, settings.ServerPath);

				using (var fileStream = File.OpenRead(fullFileName))
				{
					await Task.Factory.FromAsync(sftp.BeginUploadFile(fileStream, fileName, true, (IAsyncResult result) => { }, null), sftp.EndUploadFile);
				}

				sftp.Disconnect();
			}

			return true;
		}

		/// <summary>
		/// Delete the specified directory or file from ftp server
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="directoryOrFileName">Name of the directory or file</param>
		/// <returns></returns>
		public Task<bool> DeleteAsync(FTPSettings settings, string directoryOrFileName)
		{
			var connectionInfo = CreatConnectionInfo(settings);
			var directoryOrFilePath = GetServerPath(settings.ServerPath);
			if (!directoryOrFilePath.EndsWith("/"))
			{
				directoryOrFilePath += "/";
			}
			directoryOrFilePath += directoryOrFileName;

			using (var sftp = new SftpClient(connectionInfo))
			{
				sftp.Connect();
				sftp.Delete(directoryOrFilePath);
				sftp.Disconnect();

			}

			return Task.FromResult(true);
		}

		/// <summary>
		/// Get all file names, inclusive path, of the directory on ftp server
		/// </summary>
		/// <param name="settings">ftp server settings</param>
		/// <param name="recursive">Enumerate sub directories</param>
		/// <returns></returns>
		public async Task<IEnumerable<string>> GetFileNamesAsync(FTPSettings settings, bool recursive)
		{
			var connectionInfo = CreatConnectionInfo(settings);
			var fileNames = new List<string>();
			var directoryNames = new List<string>();

			using (var sftp = new SftpClient(connectionInfo))
			{
				sftp.Connect();

				await Task.Factory.FromAsync(sftp.BeginListDirectory(GetServerPath(settings.ServerPath), null, null), (IAsyncResult result) =>
				{
					var files = sftp.EndListDirectory(result);
					foreach (var file in files)
					{
						if (file.IsDirectory)
						{
							if (file.Name == "." || file.Name == ".." || !recursive)
							{
								continue;
							}

							directoryNames.Add(file.Name);
						}
						else
						{
							fileNames.Add(file.FullName);
						}
					}
				});

				sftp.Disconnect();
			}

			if (recursive && directoryNames.Count > 0)
			{
				var childSettings = new FTPSettings
				{
					ServerUrl = settings.ServerUrl,
					ServerPort = settings.ServerPort,
					Password = settings.Password,
					Username = settings.Username
				};
				foreach (var directoryName in directoryNames)
				{
					childSettings.ServerPath = String.Join("/", settings.ServerPath, directoryName);
					fileNames.AddRange(await GetFileNamesAsync(childSettings, true));
				}
			}

			return fileNames;
		}

		private void ChangeServerPath(SftpClient sftp, string serverPath)
		{
			var serverPathParts = GetServerPath(serverPath).Split('/');

			foreach (var serverPathPart in serverPathParts)
			{
				try
				{
					sftp.ChangeDirectory(serverPathPart);
				}
				catch
				{
					sftp.CreateDirectory(serverPathPart);
					sftp.ChangeDirectory(serverPathPart);
				}
			}
		}

		private string GetServerPath(string serverPath)
		{
			if (!serverPath.IsNullOrEmpty())
			{
				if (!serverPath.StartsWith("/"))
				{
					serverPath = "/" + serverPath;
				}
			}
			else
			{
				serverPath = "/";
			}

			return serverPath;
		}

		private ConnectionInfo CreatConnectionInfo(FTPSettings settings)
		{
			if (settings.ServerUrl.ToLower().StartsWith("sftp://"))
			{
				settings.ServerUrl = settings.ServerUrl.Substring(7);
			}

			return new ConnectionInfo(settings.ServerUrl, settings.Username, new PasswordAuthenticationMethod(settings.Username, settings.Password));
		}
	}
}