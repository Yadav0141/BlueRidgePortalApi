using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_DAL.Entities
{
    [Table("tblDocument")]
    public class tblDocument
    {
        [Key]
        public int documentid { get; set; }
        public Nullable<Guid> userId { get; set; }
        public string documentName { get; set; }
        public string documentExt { get; set; }
        public Nullable<int> documentTypeId { get; set; }
        public byte[] document { get; set; }
        public Nullable<DateTime> createdDate { get; set; }
        public Nullable<DateTime> modifiedDate { get; set; }
        public Nullable<bool> isDeleted { get; set; }
    }
}
