using BlueRidgeUtility_BAL.SelectListItems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlueRidgeUtility_BAL.Models
{
    public class UserModel
    {
        public Guid? userId { get; set; }
        [Required(ErrorMessage = "Employee code is required")]
        public string employeeId { get; set; }
        [Required(ErrorMessage = "Employee Firstname is required")]

        public string firstName { get; set; }
        public string middleName { get; set; }
        [Required(ErrorMessage = "Employee Lastname is required")]
        public string lastName { get; set; }
        [Required(ErrorMessage = "Employee email id is required")]
        public string emailId { get; set; }

        public string panCardFileName { get; set; }
        public string aadhaarCardFileName { get; set; }

        public string password { get; set; }
        public string passwordSalt { get; set; }
        public int? roleId { get; set; }

        public Guid? superVisorId { get; set; }
        public DateTime? createdDate { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime? exitDate { get; set; }
        [Required(ErrorMessage = "Employee Phone number is required")]
        public string phoneNumber { get; set; }
        [Required(ErrorMessage = "Employee Aadhaar Card number is required")]
        public string aadhaarNumber { get; set; }
        [Required(ErrorMessage = "Employee PAN Card number is required")]
        public string panCardNumber { get; set; }

        public DateTime? joiningDate { get; set; }
      

        public List<AddEditUserDocumentModel> lstDocuments{get;set;}
        [Required(ErrorMessage = "Employee address is required")]
        public string address { get; set; }

        public Datepart parsedJoiningDate { get; set; }
        public Datepart parsedExitDate { get; set; }
    }
    public class Datepart {
        public int day { get; set; }
        public int month { get; set; }
        public int year { get; set; }
    }

    public class AddEditSelectListModel
    {
        public List<SelectListItem> userList { get; set; }
        public List<SelectListItem> roleList { get; set; }
    }

    public class ForgotPasswordModel {
        [Required]
        public string emailId { get; set; }
    }

    public class ResetPasswordTokenModel
    {
        [Required]
        public string resetRequestToken { get; set; }
    }

    public class SetPasswordModel
    {
        [Required]
        public string resetRequestToken { get; set; }

        [Required]
        public string password { get; set; }

        [Required, Display(Name = "Confirm Password"), Compare(nameof(password), ErrorMessage = "Passwords does not match.")]
        public string confirmPassword { get; set; }
    }


}