using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eshava.Core.Storage.Enums;
using Eshava.Core.Storage.Interfaces;
using Eshava.Core.Storage.Models;
using Eshava.Core.Storage.Sql.Constants;
using Eshava.Core.Storage.Sql.Enums;
using Eshava.Core.Storage.Sql.Extensions;
using Microsoft.Data.SqlClient;

namespace Eshava.Core.Storage.Sql
{
	public class DatabaseEngineMsSql : IStorageEngine
	{
		public StorageType StorageType => StorageType.MsSql;

		public async Task<StorageResponse<bool>> CreateLoginAsync(UserDataRequest userData)
		{
			if (userData.Username.IsNullOrEmpty())
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = new ArgumentNullException(nameof(userData.Username)) };
			}

			if (userData.Password.IsNullOrEmpty())
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = new ArgumentNullException(nameof(userData.Password)) };
			}

			if (userData.Password.Contains(ConstantsMsSql.COMMENT) || userData.Password.Contains(ConstantsMsSql.STRING))
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = new NotSupportedException("At least one signs is not supported") };
			}

			var sqlCommand = new StringBuilder();
			sqlCommand.Append($"{ConstantsMsSql.CREATE} {ConstantsMsSql.LOGIN} [{userData.Username}] ");
			sqlCommand.Append($"{ConstantsMsSql.WITH} {ConstantsMsSql.PASSWORD} = '{userData.Password}', ");
			sqlCommand.Append($"{ConstantsMsSql.CHECK_POLICY}={ConstantsMsSql.OFF}, ");
			sqlCommand.AppendLine($"{ConstantsMsSql.DEFAULT_DATABASE}=[{userData.Server.DatabaseName}];");

			try
			{
				await ExecuteNonQueryAsync(userData.Server, sqlCommand.ToString(), ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		public async Task<StorageResponse<bool>> AddUserToDatabaseAsync(UserDataRequest userData)
		{
			if (userData.Username.IsNullOrEmpty())
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = new ArgumentNullException(nameof(userData.Username)) };
			}

			if (userData.Server.DatabaseName.IsNullOrEmpty())
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = new ArgumentNullException(nameof(userData.Server.DatabaseName)) };
			}

			var sqlCommand = new StringBuilder();
			sqlCommand.AppendLine($"{ConstantsMsSql.CREATE} {ConstantsMsSql.USER} [{userData.Username}];");
			sqlCommand.AppendLine($"{ConstantsMsSql.EXEC} {ConstantsMsSql.SP_ADDROLEMEMBER} N'{ConstantsMsSql.DBROLE_READER}', N'{userData.Username}';");
			sqlCommand.AppendLine($"{ConstantsMsSql.EXEC} {ConstantsMsSql.SP_ADDROLEMEMBER} N'{ConstantsMsSql.DBROLE_WRITER}', N'{userData.Username}';");
			sqlCommand.AppendLine($"{ConstantsMsSql.EXEC} {ConstantsMsSql.SP_ADDROLEMEMBER} N'{ConstantsMsSql.DBROLE_BACKUP}', N'{userData.Username}';");
			sqlCommand.AppendLine($"{ConstantsMsSql.EXEC} {ConstantsMsSql.SP_ADDROLEMEMBER} N'{ConstantsMsSql.DBROLE_DDLADMIN}', N'{userData.Username}';");
			sqlCommand.AppendLine($"{ConstantsMsSql.GRANT_EXECUTE_TO} [{userData.Username}];");

			try
			{
				await ExecuteNonQueryAsync(userData.Server, sqlCommand.ToString());
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		public async Task<StorageResponse<bool>> DropUserFromDatabaseAsync(UserDataRequest userData)
		{
			if (userData.Username.IsNullOrEmpty())
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = new ArgumentNullException(nameof(userData.Username)) };
			}

			try
			{
				await ExecuteNonQueryAsync(userData.Server, $"{ConstantsMsSql.DROP} {ConstantsMsSql.USER} [{userData.Username}]");
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		public async Task<StorageResponse<bool>> DropLoginAsync(UserDataRequest userData)
		{
			if (userData.Username.IsNullOrEmpty())
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = new ArgumentNullException(nameof(userData.Username)) };
			}

			var killResult = await KillUserConncetionsAsync(userData.Server, userData.Username);
			if (killResult.IsFaulty)
			{
				return killResult;
			}

			try
			{
				await ExecuteNonQueryAsync(userData.Server, $"{ConstantsMsSql.DROP} {ConstantsMsSql.LOGIN} [{userData.Username}]", ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		public async Task<StorageResponse<bool>> CreateDatabaseAsync(CreateDatabaseRequest request)
		{
			var sqlCommand = new StringBuilder();
			sqlCommand.AppendLine($"{ConstantsMsSql.USE} {ConstantsMsSql.MASTER};");
			sqlCommand.Append($"{ConstantsMsSql.CREATE} {ConstantsMsSql.DATABASE} ");
			sqlCommand.AppendLine($"[{request.Server.DatabaseName}]");

			AddFileDescription(DatabaseFileType.Data, request.Server, request.DataFileOptions, sqlCommand);
			AddFileDescription(DatabaseFileType.Log, request.Server, request.LogFileOptions, sqlCommand);

			try
			{
				await ExecuteNonQueryAsync(request.Server, sqlCommand.ToString(), ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		public async Task<StorageResponse<bool>> DropDatabaseAsync(DatabaseConnectionOptions server)
		{
			var killResult = await KillDatabaseConncetionsAsync(server);
			if (killResult.IsFaulty)
			{
				return killResult;
			}

			try
			{
				var sqlCommand = $"{ConstantsMsSql.DROP} {ConstantsMsSql.DATABASE} [{server.DatabaseName}]";
				await ExecuteNonQueryAsync(server, sqlCommand, ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		public async Task<StorageResponse<BackupDatabaseResponse>> BackupDatabaseAsync(BackupDatabaseRequest request, string fileNameWithoutExtension)
		{
			var filePath = request.BackupPath;

			try
			{
				var fullFileName = System.IO.Path.Combine(filePath, fileNameWithoutExtension + ConstantsMsSql.FILE_EXTENSION_BAK);

				var sqlCommand = new StringBuilder();
				sqlCommand.AppendLine($"{ConstantsMsSql.BACKUP} {ConstantsMsSql.DATABASE} [{request.Server.DatabaseName}]");
				sqlCommand.AppendLine(JoinStatement(ConstantsMsSql.TODISK, fullFileName));
				sqlCommand.AppendLine($"{ConstantsMsSql.WITHFORMAT},");
				sqlCommand.Append(JoinStatement(ConstantsMsSql.MEDIANAME, request.Server.DatabaseName));
				sqlCommand.AppendLine(",");
				sqlCommand.AppendLine(JoinStatement(ConstantsMsSql.NAME, request.Server.DatabaseName));

				await ExecuteNonQueryAsync(request.Server, sqlCommand.ToString(), ConstantsMsSql.MASTER);

				return new StorageResponse<BackupDatabaseResponse>
				{
					Data = new BackupDatabaseResponse
					{
						IsSuccessful = true,
						BackupFullFileName = fullFileName
					}
				};
			}
			catch (Exception ex)
			{
				return new StorageResponse<BackupDatabaseResponse>
				{
					IsFaulty = true,
					Exception = ex
				};
			}
		}

		public async Task<StorageResponse<bool>> RestoreDatabaseAsync(RestoreDatabaseRequest request)
		{
			await KillDatabaseConncetionsAsync(request.Server);

			try
			{
				var dataFileName = GetDatabaseName(DatabaseFileType.Data, request.Server.DatabaseName);
				var logFileName = GetDatabaseName(DatabaseFileType.Log, request.Server.DatabaseName);

				var dataFullFileName = System.IO.Path.Combine(request.FilePathData, dataFileName + ConstantsMsSql.FILE_EXTENSION_MDF);
				var logFullFileName = System.IO.Path.Combine(request.FilePathLog, logFileName + ConstantsMsSql.FILE_EXTENSION_LDF);

				var sqlCommand = new StringBuilder();
				sqlCommand.AppendLine($"{ConstantsMsSql.IF} DB_ID('{request.Server.DatabaseName}') {ConstantsMsSql.ISNOTNULL}");
				sqlCommand.AppendLine(ConstantsMsSql.BEGIN);
				sqlCommand.AppendLine($"{ConstantsMsSql.DROP} {ConstantsMsSql.DATABASE} [{request.Server.DatabaseName}]");
				sqlCommand.AppendLine(ConstantsMsSql.END);
				sqlCommand.AppendLine($"{ConstantsMsSql.RESTORE} {ConstantsMsSql.DATABASE} [{request.Server.DatabaseName}]");
				sqlCommand.AppendLine($"{ConstantsMsSql.FROM} {ConstantsMsSql.DISK} = '{request.BackupFullFileName}'");
				sqlCommand.AppendLine($"{ConstantsMsSql.WITH} {ConstantsMsSql.MOVE} '{dataFileName}' {ConstantsMsSql.TO} '{dataFullFileName}',");
				sqlCommand.AppendLine($"{ConstantsMsSql.MOVE} '{logFileName}' {ConstantsMsSql.TO} '{logFullFileName}',");
				sqlCommand.AppendLine($"{ConstantsMsSql.STATS} = 10;");

				await ExecuteNonQueryAsync(request.Server, sqlCommand.ToString(), ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		public async Task<StorageResponse<bool>> CopyDatabaseAsync(CopyDatabaseRequest request)
		{
			// 1) Create database backup
			var databaseBackupFilePath = System.IO.Path.Combine(request.BackupPath,  $"{request.DatabaseNameSource}_{Guid.NewGuid()}{ConstantsMsSql.FILE_EXTENSION_BAK}");
			var sourceDataFileName = GetDatabaseName(DatabaseFileType.Data, request.DatabaseNameSource);
			var sourceLogFileName = GetDatabaseName(DatabaseFileType.Log, request.DatabaseNameSource);

			try
			{
				var sqlCommand = new StringBuilder();
				sqlCommand.AppendLine($"{ConstantsMsSql.BACKUP} {ConstantsMsSql.DATABASE} [{request.DatabaseNameSource}]");
				sqlCommand.AppendLine(JoinStatement(ConstantsMsSql.TODISK, databaseBackupFilePath));
				sqlCommand.AppendLine(ConstantsMsSql.WITHCOPYONLY);
				
				await ExecuteNonQueryAsync(request.Server, sqlCommand.ToString(), ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool>
				{
					IsFaulty = true,
					Exception = ex
				};
			}

			// 2) Restore backup as new database
			try
			{
				var targetDataFileName = GetDatabaseName(DatabaseFileType.Data, request.DatabaseNameTarget);
				var targetlogFileName = GetDatabaseName(DatabaseFileType.Log, request.DatabaseNameTarget);

				var targetDataFullFileName = System.IO.Path.Combine(request.TargetFilePathData, targetDataFileName + ConstantsMsSql.FILE_EXTENSION_MDF);
				var targetLogFullFileName = System.IO.Path.Combine(request.TargetFilePathLog, targetlogFileName + ConstantsMsSql.FILE_EXTENSION_LDF);

				var sqlCommand = new StringBuilder();
				sqlCommand.AppendLine($"{ConstantsMsSql.RESTORE} {ConstantsMsSql.DATABASE} [{request.DatabaseNameTarget}]");
				sqlCommand.AppendLine($"{ConstantsMsSql.FROM} {ConstantsMsSql.DISK} = '{databaseBackupFilePath}'");
				sqlCommand.AppendLine($"{ConstantsMsSql.WITH} {ConstantsMsSql.MOVE} '{sourceDataFileName}' {ConstantsMsSql.TO} '{targetDataFullFileName}',");
				sqlCommand.AppendLine($"{ConstantsMsSql.MOVE} '{sourceLogFileName}' {ConstantsMsSql.TO} '{targetLogFullFileName}',");
				sqlCommand.AppendLine(ConstantsMsSql.REPLACE);

				await ExecuteNonQueryAsync(request.Server, sqlCommand.ToString(), ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool>
				{
					IsFaulty = true,
					Exception = ex
				};
			}

			// 3) Delete backup file
			try
			{
				System.IO.File.Delete(databaseBackupFilePath);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool>
				{
					Data = true,
					IsFaulty = false,
					Exception = ex
				};
			}

			return new StorageResponse<bool>
			{
				Data = true
			};
		}

		public async Task<StorageResponse<IEnumerable<StatisticsDatabase>>> GetDatabasesAsync(DatabaseConnectionOptions server)
		{
			try
			{
				var sqlCommand = new StringBuilder();

				sqlCommand.Append(ConstantsMsSql.SELECT);
				sqlCommand.Append(" sys.databases.name,");
				sqlCommand.Append(" sys.databases.create_date,");
				sqlCommand.Append(" sys.databases.state_desc,");
				sqlCommand.Append($" {ConstantsMsSql.MAX}({ConstantsMsSql.MSDB}.{ConstantsMsSql.DBO}.backupset.backup_finish_date) {ConstantsMsSql.AS} last_backup ");
				sqlCommand.Append(ConstantsMsSql.FROM);
				sqlCommand.Append(" sys.databases");
				sqlCommand.Append($" {ConstantsMsSql.LEFTOUTERJOIN} {ConstantsMsSql.MSDB}.{ConstantsMsSql.DBO}.backupset ON {ConstantsMsSql.MSDB}.{ConstantsMsSql.DBO}.backupset.database_name = sys.databases.name");
				sqlCommand.Append($" {ConstantsMsSql.GROUPBY} sys.databases.Name, sys.databases.create_date, sys.databases.state_desc");

				var databases = await ExecuteReaderAsync(server, sqlCommand.ToString(),
					reader => new StatisticsDatabase
					{
						Database = reader.GetString(reader.GetOrdinal("name")),
						Created = reader.GetDateTime(reader.GetOrdinal("create_date")),
						Status = reader.GetString(reader.GetOrdinal("state_desc")),
						LastBackup = reader[reader.GetOrdinal("last_backup")] == DBNull.Value ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("last_backup"))
					}, ConstantsMsSql.MASTER);

				databases = databases.Where(database => !database.Database.ToLower().StartsWith(ConstantsMsSql.MASTER) &&
												   !database.Database.ToLower().StartsWith(ConstantsMsSql.MODEL) &&
												   !database.Database.ToLower().StartsWith(ConstantsMsSql.MSDB) &&
												   !database.Database.ToLower().StartsWith(ConstantsMsSql.TEMPDB));

				return new StorageResponse<IEnumerable<StatisticsDatabase>>
				{
					Data = databases.ToList()
				};
			}
			catch (Exception ex)
			{
				return new StorageResponse<IEnumerable<StatisticsDatabase>>
				{
					IsFaulty = true,
					Exception = ex
				};
			}
		}

		public async Task<StorageResponse<IEnumerable<StatisticsUserData>>> GetLoginsAsync(DatabaseConnectionOptions server)
		{
			try
			{
				var sqlCommand = new StringBuilder();

				sqlCommand.Append(ConstantsMsSql.SELECT);
				sqlCommand.Append(" createdate,");
				sqlCommand.Append(" syslogins.name,");
				sqlCommand.Append(" dbname,");
				sqlCommand.Append(" loginname, ");
				sqlCommand.Append(" is_disabled ");
				sqlCommand.Append(ConstantsMsSql.FROM);
				sqlCommand.Append(" syslogins");
				sqlCommand.Append($" {ConstantsMsSql.LEFTOUTERJOIN} sys.sql_logins ON sys.sql_logins.sid = syslogins.sid");

				var users = await ExecuteReaderAsync(server, sqlCommand.ToString(),
					reader => new StatisticsUserData
					{
						Database = reader.GetString(reader.GetOrdinal("dbname")),
						Created = reader.GetDateTime(reader.GetOrdinal("createdate")),
						LoginName = reader.GetString(reader.GetOrdinal("loginname")),
						Name = reader.GetString(reader.GetOrdinal("name")),
						Disabled = reader[reader.GetOrdinal("is_disabled")] != DBNull.Value && reader.GetBoolean(reader.GetOrdinal("is_disabled"))
					}, ConstantsMsSql.MASTER);

				users = users.Where(login => !login.LoginName.ToLower().StartsWith("#") && !login.LoginName.ToLower().StartsWith("nt"));

				return new StorageResponse<IEnumerable<StatisticsUserData>>
				{
					Data = users.ToList()
				};
			}
			catch (Exception ex)
			{
				return new StorageResponse<IEnumerable<StatisticsUserData>>
				{
					IsFaulty = true,
					Exception = ex
				};
			}
		}

		private async Task<StorageResponse<bool>> KillDatabaseConncetionsAsync(DatabaseConnectionOptions server)
		{
			try
			{
				var sqlCommand = new StringBuilder();
				sqlCommand.AppendLine($"{ConstantsMsSql.DECLARE}  @kill varchar(8000) = '';");
				sqlCommand.AppendLine($"{ConstantsMsSql.SELECT} @kill = @kill + 'kill ' + {ConstantsMsSql.CONVERT}(varchar(5), session_id) + ';' ");
				sqlCommand.AppendLine($"{ConstantsMsSql.FROM} sys.dm_exec_sessions");
				sqlCommand.AppendLine($"{ConstantsMsSql.WHERE} database_id  = db_id('{server.DatabaseName}')");
				sqlCommand.AppendLine($"{ConstantsMsSql.EXEC}(@kill);");

				await ExecuteNonQueryAsync(server, sqlCommand.ToString(), ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		private async Task<StorageResponse<bool>> KillUserConncetionsAsync(DatabaseConnectionOptions server, string username)
		{
			try
			{
				var sqlCommand = new StringBuilder();
				sqlCommand.AppendLine($"{ConstantsMsSql.DECLARE}  @kill varchar(8000) = '';");
				sqlCommand.AppendLine($"{ConstantsMsSql.SELECT} @kill = @kill + 'kill ' + {ConstantsMsSql.CONVERT}(varchar(5), session_id) + ';' ");
				sqlCommand.AppendLine($"{ConstantsMsSql.FROM} sys.dm_exec_sessions");
				sqlCommand.AppendLine($"{ConstantsMsSql.WHERE} login_name  = '{username}'");
				sqlCommand.AppendLine($"{ConstantsMsSql.EXEC}(@kill);");

				await ExecuteNonQueryAsync(server, sqlCommand.ToString(), ConstantsMsSql.MASTER);
			}
			catch (Exception ex)
			{
				return new StorageResponse<bool> { IsFaulty = true, Exception = ex };
			}

			return new StorageResponse<bool> { Data = true };
		}

		private string BuildConnectionString(DatabaseConnectionOptions server, string databasename = null)
		{
			var builder = new SqlConnectionStringBuilder
			{
				DataSource = server.ServerInstance,
				InitialCatalog = databasename ?? server.DatabaseName,
				IntegratedSecurity = server.IntegratedSecurity,
				TrustServerCertificate = server.TrustServerCertificate
			};

			if (!server.IntegratedSecurity)
			{
				builder.Password = server.Password;
				builder.UserID = server.Username;
			}

			return builder.ToString();
		}

		private async Task<SqlConnection> CreateAndOpenConnectionAsync(DatabaseConnectionOptions server, string databasename = null)
		{
			var connectionString = BuildConnectionString(server, databasename);
			var connection = new SqlConnection(connectionString);
			await connection.OpenAsync();

			return connection;
		}

		private async Task ExecuteNonQueryAsync(DatabaseConnectionOptions server, string sqlCommand, string databasename = null)
		{
			var connection = await CreateAndOpenConnectionAsync(server, databasename);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = sqlCommand;
				cmd.CommandType = CommandType.Text;
				cmd.Connection = connection;
				cmd.CommandTimeout = server.CommandTimeOut;

				await cmd.ExecuteNonQueryAsync();
				CloseConnection(connection);
			}
		}

		private void CloseConnection(SqlConnection connection)
		{
			if (connection != null)
			{
				if (connection.State != ConnectionState.Closed)
				{
					connection.Close();
				}

				connection.Dispose();
			}
		}

		private void AddFileDescription(DatabaseFileType type, DatabaseConnectionOptions server, DatabaseFileOptions option, StringBuilder sqlCommand)
		{
			var databasename = GetDatabaseName(type, server.DatabaseName);
			var initString = ConstantsMsSql.ON;
			var fileExtension = ConstantsMsSql.FILE_EXTENSION_MDF;
			var path = option.FilePath;

			if (type == DatabaseFileType.Log)
			{
				initString = $"{ConstantsMsSql.LOG} {initString}";
				fileExtension = ConstantsMsSql.FILE_EXTENSION_LDF;
			}

			sqlCommand.AppendLine(initString);
			sqlCommand.AppendLine("(");
			sqlCommand.Append(JoinStatement(ConstantsMsSql.NAME, databasename));
			sqlCommand.AppendLine(",");
			sqlCommand.AppendLine(JoinStatement(ConstantsMsSql.FILENAME, System.IO.Path.Combine(path, $"{databasename}{fileExtension}")));

			CheckOption(ConstantsMsSql.SIZE, option.Size, sqlCommand);
			CheckOption(ConstantsMsSql.MAXSIZE, option.MaxSize, sqlCommand);
			CheckOption(ConstantsMsSql.FILEGROWTH, option.FileGrowth, sqlCommand);

			sqlCommand.AppendLine();
			sqlCommand.AppendLine(")");
		}

		private string GetDatabaseName(DatabaseFileType type, string database)
		{
			return type == DatabaseFileType.Log ? $"{database}_log" : database;
		}

		private string JoinStatement(string name, string option)
		{
			return $"{name} = '{option}'";
		}

		private string JoinStatement(string name, int option)
		{
			return $"{name} = {option}";
		}

		private void CheckOption(string name, int option, StringBuilder sqlCommand)
		{
			if (option > 0)
			{
				sqlCommand.AppendLine(",");
				sqlCommand.Append(JoinStatement(name, option) + ConstantsMsSql.MB);
			}
		}

		private async Task<IEnumerable<T>> ExecuteReaderAsync<T>(DatabaseConnectionOptions server, string sqlCommand, Func<SqlDataReader, T> read, string databasename = null) where T : class
		{
			var items = new List<T>();
			var connection = await CreateAndOpenConnectionAsync(server, databasename);

			using (var cmd = new SqlCommand())
			{
				cmd.CommandText = sqlCommand;
				cmd.CommandType = CommandType.Text;
				cmd.Connection = connection;
				cmd.CommandTimeout = server.CommandTimeOut;

				var reader = await cmd.ExecuteReaderAsync();

				while (reader.Read())
				{
					items.Add(read(reader));
				}

				reader.Close();

				CloseConnection(connection);
			}

			return items;
		}
	}
}