using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Models
{
    public class LoginResponseModel
    {
        public Guid userId { get; set; }
        public int? roleId { get; set; }
        public string roleDescription { get; set; }
        public string[] modules { get; set; }
        public Guid? superVisorId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userName { get; set; }

    }
}
