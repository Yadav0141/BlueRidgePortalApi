using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Models
{
  public  class BackupRestoreHistoryListByUserModel
    {
     
        public int historyId { get; set; }
       
        public Nullable<int> backupPercentage { get; set; }
        public Nullable<int> restorePercentage { get; set; }
        public Nullable<DateTime> createdDate { get; set; }
        public string sourceDatabaseName { get; set; }
        public string destinationDatabaseName { get; set; }
        public bool isErrored { get; set; }
        public string errorInformation { get; set; }

        public string status { get; set; }

      
    }

    public class BackupRestoreHistoryListByUserViewModel
    {
        public List<BackupRestoreHistoryListByUserModel> lstBackupRestoreHistoryListByUserModel { get; set; }
        public int totalCount { get; set; }
    }
}
