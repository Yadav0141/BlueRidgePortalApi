using BlueRidgeUtility_BAL.Enums;
using BlueRidgeUtility_BAL.Mappers;
using BlueRidgeUtility_BAL.Models;
using BlueRidgeUtility_DAL;
using BlueRidgeUtility_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Managers
{
  public  class BackupRestoreService : IBackupRestoreService
    {
        string SP_GetAllBackupHistoryByUser = "usp_GetAllBackupRestoreHistoryByUser @userId";
        public int? InsertNewBackupRestoreEntry(BackupRestoreModel model) {

          var tblrestoreHistory=  Mapping.Mapper.Map<tblBackupRestoreHistory>(model);
            if (tblrestoreHistory != null)
            {
                using (var context = new BlueRidgeUtilityDBContext())
                {
                    context.TblBackupRestoreHistory.Add(tblrestoreHistory);
                    context.Entry(tblrestoreHistory).State = System.Data.Entity.EntityState.Added;
                    context.SaveChanges();
                    return tblrestoreHistory.backupRestoreHistoryId;
                }
            }
            return null;
        }

        public void InsertErrorInformation(int? bkpHistoryId, Exception exception)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
              var tblbkpHistory=  context.TblBackupRestoreHistory.Where(x => x.backupRestoreHistoryId == bkpHistoryId).FirstOrDefault();
                tblbkpHistory.errorInformation = $"{DateTime.Now} - {exception.Message} - {exception.InnerException} - {exception.StackTrace}";
                tblbkpHistory.isErrored = true;
                tblbkpHistory.status = Convert.ToInt32(BackRestoreStatus.Errored);
                context.Entry(tblbkpHistory).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void updateBackupFileName(int? bkpHistoryId, string filename)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var tblbkpHistory = context.TblBackupRestoreHistory.Where(x => x.backupRestoreHistoryId == bkpHistoryId).FirstOrDefault();
                tblbkpHistory.bkpFileName = filename;
                context.Entry(tblbkpHistory).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void updateBackupPercentage(int? bkpHistoryId, int percentage)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var tblbkpHistory = context.TblBackupRestoreHistory.Where(x => x.backupRestoreHistoryId == bkpHistoryId).FirstOrDefault();
                tblbkpHistory.backupPercentage = percentage;
                if (percentage == 100)
                {
                    tblbkpHistory.isBackupCompleted = true;
                 
                }
                context.Entry(tblbkpHistory).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public void updateRestorePercentage(int? bkpHistoryId, int percentage)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var tblbkpHistory = context.TblBackupRestoreHistory.Where(x => x.backupRestoreHistoryId == bkpHistoryId).FirstOrDefault();
                tblbkpHistory.restorePercentage = percentage;
                if (percentage == 100)
                {
                    tblbkpHistory.isRestoreCompleted = true;
                    tblbkpHistory.status = Convert.ToInt32(BackRestoreStatus.Completed);
                }
                context.Entry(tblbkpHistory).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public BackupRestoreHistoryListByUserViewModel GetBackupRestoreHistoryListByUser(Guid? userId, int pageNumber, int pageCount)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var totalCount = context.TblBackupRestoreHistory.AsNoTracking().Count(user => user.userId==userId && user.isDeleted != true);
                if (totalCount > 0)
                {
                    var lstHistory = context.Database.SqlQuery<BackupRestoreHistoryListByUserModel>(SP_GetAllBackupHistoryByUser, new SqlParameter("userId", userId)).Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList();
                    BackupRestoreHistoryListByUserViewModel viewModel = new BackupRestoreHistoryListByUserViewModel();
                    viewModel.lstBackupRestoreHistoryListByUserModel = lstHistory;
                    viewModel.totalCount = totalCount;
                    return viewModel;
                }

            }
            return null;
        }

      


       public List<ConnectionStringModel> getAllDatabaseServers(int databaseType)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var tblSourceDatabase = context.TblAllDatabaseConnectionStrings.Where(x => x.databaseType == databaseType && x.isDeleted != true)?.ToList();
                if (tblSourceDatabase != null)
                {
                    return Mapping.Mapper.Map<List<ConnectionStringModel>>(tblSourceDatabase);

                }

                return null;
            }
        }
        public void updateBKP_RestoreHistoryStatus(int? bkpHistoryId, BackRestoreStatus backRestoreStatus) {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var tblbkpHistory = context.TblBackupRestoreHistory.Where(x => x.backupRestoreHistoryId == bkpHistoryId).FirstOrDefault();
                tblbkpHistory.status =Convert.ToInt32( backRestoreStatus);
                context.Entry(tblbkpHistory).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        public bool DeleteBackupHistory(int bkpHistoryId)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var tblbkpHistory = context.TblBackupRestoreHistory.Where(x => x.backupRestoreHistoryId == bkpHistoryId).FirstOrDefault();
                tblbkpHistory.isDeleted = true;
                context.Entry(tblbkpHistory).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return true;
            }
        }

        public ConnectionStringModel getDatabaseConnectionStringModel(int connectionStringId)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var tblSourceDatabase = context.TblAllDatabaseConnectionStrings.Where(x => x.connectionStringId == connectionStringId && x.isDeleted != true).FirstOrDefault();
                if (tblSourceDatabase != null)
                {
                    return Mapping.Mapper.Map<ConnectionStringModel>(tblSourceDatabase);

                }

                return null;
            }
        }


    }
}
