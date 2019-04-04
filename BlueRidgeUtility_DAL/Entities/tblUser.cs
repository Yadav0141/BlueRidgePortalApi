using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueRidgeUtility_DAL.Entities
{
    [Table("tblUser")]
    public class tblUser
    {
        [Key]
        public Guid userId { get; set; }
        public string employeeId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string emailId { get; set; }
        public string password { get; set; }
        public string passwordSalt { get; set; }
        public Nullable<int> roleId { get; set; }
        public Nullable<Guid> superVisorId { get; set; }
        public Nullable<DateTime> createdDate { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public Nullable<DateTime> exitDate { get; set; }
        public string phoneNumber { get; set; }
        public string aadhaarNumber { get; set; }
        public string panCardNumber { get; set; }
        public Nullable<DateTime> joiningDate { get; set; }
       
        public string address { get; set; }

        public Nullable<DateTime> modifiedDate { get; set; }
        public string setPasswordToken { get; set; }
        public Nullable<DateTime> setTokenExpirationDate { get; set; }
    }
}
