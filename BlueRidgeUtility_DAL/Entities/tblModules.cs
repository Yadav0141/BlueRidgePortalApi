using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_DAL.Entities
{

    [Table("tblModules")]
    public class tblModules
    {
        [Key]
        public int moduleId { get; set; }
        public string moduleName { get; set; }
        public string moduleDescription { get; set; }
    }
}
