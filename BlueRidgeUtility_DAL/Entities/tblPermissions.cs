using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_DAL.Entities
{
    [Table("tblPermissions")]
    public class tblPermissions
    {
        [Key]
        public int permissionId { get; set; }
        public Nullable<int> roleId { get; set; }
        public Nullable<int> moduleId { get; set; }
    }
}
