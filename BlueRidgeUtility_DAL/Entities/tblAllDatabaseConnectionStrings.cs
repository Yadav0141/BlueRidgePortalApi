using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_DAL.Entities
{
    [Table("tblAllDatabaseConnectionStrings")]
    public class tblAllDatabaseConnectionStrings
    {
        [Key]
        public int connectionStringId { get; set; }
        public int databaseType { get; set; }
        public string databaseServerName { get; set; }
        public string databaseConnectionString { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public string bkpFilePath { get; set; }
    }
}
