using BlueRidgeUtility_BAL.Enums;
using BlueRidgeUtility_BAL.Models;
using System;
using System.Collections.Generic;

namespace BlueRidgeUtility_BAL.Managers
{
    public interface IBackupRestoreService
    {
        int? InsertNewBackupRestoreEntry(BackupRestoreModel model);
        void InsertErrorInformation(int? bkpHistoryId, Exception exception);
        void updateBackupFileName(int? bkpHistoryId, string filename);
        void updateBackupPercentage(int? bkpHistoryId, int percentage);
        void updateRestorePercentage(int? bkpHistoryId, int percentage);
        BackupRestoreHistoryListByUserViewModel GetBackupRestoreHistoryListByUser(Guid? userId, int pageNumber, int pageCount);
        List<ConnectionStringModel> getAllDatabaseServers(int databaseType);
        void updateBKP_RestoreHistoryStatus(int? bkpHistoryId, BackRestoreStatus backRestoreStatus);
        bool DeleteBackupHistory(int bkpHistoryId);
        ConnectionStringModel getDatabaseConnectionStringModel(int connectionStringId);

    }
}