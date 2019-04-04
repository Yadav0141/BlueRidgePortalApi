using BlueRidgeUtility_BAL.Managers;
using BlueRidgeUtility_BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace BlueRidgeUtility.Validators
{
    public class UserModelValidator : IUserModelValidator
    {
        IUserService _userService;
        public UserModelValidator(IUserService userService)
        {
            _userService = userService;
        }

        public void ValidateUserModel(UserModel userModel, ModelStateDictionary ModelState)
        {
            if (_userService.isEmailIdAlreadyExist(userModel))
            {
                ModelState.AddModelError("error", "Email id already exist.");
            }
            if (_userService.isEmployeeCodeExist(userModel))
            {
                ModelState.AddModelError("error", "Employee code already exist.");
            }

        }

        

        public LoginResponseModel ValidateLoginModel(LoginModel loginModel, ModelStateDictionary ModelState)
        {
            var responseModel = new LoginResponseModel();
            var response= _userService.ValidateLoginModel(loginModel, ref responseModel);
            switch (response)
            {
                case BlueRidgeUtility_BAL.Enums.LoginValidateResponseCode.INVALID_USERNAME_PASSWORD:
                    ModelState.AddModelError("error", "Invalid username or password.");
                    break;
                case BlueRidgeUtility_BAL.Enums.LoginValidateResponseCode.USER_NOT_EXIST:
                    ModelState.AddModelError("error", "User does not exist.");
                    break;
                case BlueRidgeUtility_BAL.Enums.LoginValidateResponseCode.USER_LOGIN_SUCCESS:
                    return responseModel;
                case BlueRidgeUtility_BAL.Enums.LoginValidateResponseCode.PASSWORD_NOT_SET:
                    ModelState.AddModelError("error", "Password not set.Please set password first.");
                    break;
                default:
                    break;
            }
            return null;
        }

        public Guid? ValidateForgotPasswordModel(ForgotPasswordModel model, ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            Guid? userId=null;
          var response=  _userService.ValidateForgotPassword(model,ref userId);
            switch (response)
            {
                case BlueRidgeUtility_BAL.Enums.ForgotPasswordResponseCode.USER_NOT_FOUND:
                    ModelState.AddModelError("error", "User with this email address does not exist.");
                    break;
                case BlueRidgeUtility_BAL.Enums.ForgotPasswordResponseCode.USER_EXIST:
                    return userId;
                   
                default:
                    return null;
            }
            return null;
        }

        public bool ValidateResetPasswordModel(ResetPasswordTokenModel model, ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }
            var result = _userService.ValidateResetPasswordModel(model);
            if (result)
            {

                return true;
            }
            else
            {
                ModelState.AddModelError("error", "Invalid token.You can request password reset link from forgot password link on login page.");
                return false;
            }
           

        }

       

        public bool VerifySetPasswordModel(SetPasswordModel model, ModelStateDictionary ModelState)
        {
            var result = false;
            if (!ModelState.IsValid)
            {
                return result;
            }
            result= _userService.VerifySetPasswordModel(model);
            if (!result)
            {
                ModelState.AddModelError("error", "Invalid Token.");
            }
            
            return result;
        }
    }
}