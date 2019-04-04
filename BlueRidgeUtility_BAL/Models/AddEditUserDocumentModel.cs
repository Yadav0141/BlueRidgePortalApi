using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Models
{
    public class AddEditUserDocumentModel
    {
        public Guid userId { get; set; }
        public byte[] document { get; set; }
        public int documentTypeId { get; set; }
        public string documentName { get; set; }
        public string documentExt { get; set; }

        public string content_type { get; set; }
    }
}
