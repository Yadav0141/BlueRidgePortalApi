using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_DAL.Entities
{
    [Table("tblBackupRestoreHistory")]
    public class tblBackupRestoreHistory
    {
        [Key]
        public int backupRestoreHistoryId { get; set; }
        public Guid? userId { get; set; }
      
        public string bkpFileName { get; set; }
        public Nullable<bool> isBackupCompleted { get; set; }
        public Nullable<bool> isRestoreCompleted { get; set; }
        public Nullable<int> backupPercentage { get; set; }
        public Nullable<int> restorePercentage { get; set; }
        public Nullable<bool> isErrored { get; set; }
        public string errorInformation { get; set; }
        public Nullable<DateTime> createdDate { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public string sourceDatabaseName { get; set; }
        public string destinationDatabaseName { get; set; }

        public Nullable<int> status { get; set; }

        public Nullable<int> sourceConnectionStringId { get; set; }
        public Nullable<int> destinationConnectionStringId { get; set; }
    }
}
