using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_DAL.Entities
{
    [Table("tblDocumentType")]
   public class tblDocumentType
    {
        [Key]
        public int documentTypeId { get; set; }
        public string documentTypeName { get; set; }
    }
}
