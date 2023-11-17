using System.Collections.Generic;
using System.Threading.Tasks;
using Eshava.Core.Storage.Enums;
using Eshava.Core.Storage.Models;

namespace Eshava.Core.Storage.Interfaces
{
	public interface IStorageEngine
	{
		StorageType StorageType { get; }

		Task<StorageResponse<bool>> CreateLoginAsync(UserDataRequest request);
		Task<StorageResponse<bool>> AddUserToDatabaseAsync(UserDataRequest request);
		Task<StorageResponse<bool>> DropUserFromDatabaseAsync(UserDataRequest request);
		Task<StorageResponse<bool>> DropLoginAsync(UserDataRequest request);

		Task<StorageResponse<bool>> CreateDatabaseAsync(CreateDatabaseRequest request);
		Task<StorageResponse<bool>> DropDatabaseAsync(DatabaseConnectionOptions server);
		Task<StorageResponse<bool>> CopyDatabaseAsync(CopyDatabaseRequest request);

		Task<StorageResponse<BackupDatabaseResponse>> BackupDatabaseAsync(BackupDatabaseRequest request, string fileNameWithoutExtension);
		Task<StorageResponse<bool>> RestoreDatabaseAsync(RestoreDatabaseRequest request);

		Task<StorageResponse<IEnumerable<StatisticsDatabase>>> GetDatabasesAsync(DatabaseConnectionOptions server);
		Task<StorageResponse<IEnumerable<StatisticsUserData>>> GetLoginsAsync(DatabaseConnectionOptions server);
	}
}