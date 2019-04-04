using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlueRidgeUtility_BAL.Models
{
    public class BackupRestoreModel
    {
        public int sourceServerId { get; set; }
        public int destinationServerId { get; set; }

        public int sourceDatabaseId { get; set; }
        [Required(ErrorMessage = "Source database name is required")]
        public string sourceDatabaseName { get; set; }
       
        [Required(ErrorMessage ="Destination database name is required")]
        public string destinationDatabaseName { get; set; }
        public string sourceDatabaseSelected { get; set; }

        public string sourceServerUsername { get; set; }
        public string sourceServerPassword { get; set; }

        public string destinationServerUsername { get; set; }
        public string destinationServerPassword { get; set; }

        public string sourceServerBKPPath { get; set; }
        public string destinationServerBKPPath { get; set; }

        public string bkpFilename { get; set; }
       

        public string uniqueTaskId { get; set; }

  

        public Guid? userId { get; set; }

        public int? bkpHistoryId { get; set; }
    }

    public class DatabaseSelectItems {
        public string name { get; set; }
        public int database_id { get; set; }

        public int server_id { get; set; }
        public string value { get; set; }
    }

    public class GetSourceDatabaseInitials
    {
        //public string sourceIpAddress { get; set; }
        //public string sourceUsername { get; set; }
        //public string sourcePassword { get; set; }
        
        //public string destinationIpAddress { get; set;}
        //public string destinationUsername { get; set; }
        //public string destinationPassword { get; set; }
        public IList<DatabaseSelectItems> sourceServerSelectList { get; set; }
        public IList<DatabaseSelectItems> destinationDBServers { get; set; }

    }
}