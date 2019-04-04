using System.Collections.Generic;
using BlueRidgeUtility_BAL.Models;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace BlueRidgeUtility.BlueRidgeUtility_BLL
{
    public interface IDBBackupRestoreManager
    {

        string BackupDataBase(BackupRestoreModel backupModel, string connectionString);
        void backup_Complete(object sender, ServerMessageEventArgs e, string taskId);
        void backup_PercentComplete(object sender, PercentCompleteEventArgs e, string taskId, int? bkpHistoryId);
        string Backup_Restore(BackupRestoreModel backupModel);
        int CreateDestinationDatabase(string connectionString, string databaseName);

        List<DatabaseSelectItems> GetAllDatabaseNameFromConnectionString(string connectionString);
      
        IList<DatabaseSelectItems> GetAllSourceDatabaseNames();
        GetSourceDatabaseInitials GetDatabase_Backup_Utility_Details();

        bool isDatabaseNameExistOnDestination(string databaseName, int connectionStringId);
        void myRestore_Complete(object sender, ServerMessageEventArgs e, string taskId);
        void RestoreDataBase(BackupRestoreModel backupModel, string connectionString);

        void myRestore_PercentComplete(object sender, PercentCompleteEventArgs e, string taskId, int? bkpHistoryId);
        Server GetServerUsingConnectionString(string connectionString);
    }
}