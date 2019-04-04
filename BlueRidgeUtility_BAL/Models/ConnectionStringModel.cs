using BlueRidgeUtility_BAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Models
{
   public class ConnectionStringModel
    {
       
        public int connectionStringId { get; set; }
        public Backup_Restore_DatabaseType databaseType { get; set; }
        public string databaseServerName { get; set; }
        public string databaseConnectionString { get; set; }

        public string bkpFilePath { get; set; }
    }
}
