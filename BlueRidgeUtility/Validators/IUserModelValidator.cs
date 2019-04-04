using System;
using System.Web.Http.ModelBinding;
using BlueRidgeUtility_BAL.Models;

namespace BlueRidgeUtility.Validators
{
    public interface IUserModelValidator
    {
        void ValidateUserModel(UserModel userModel, ModelStateDictionary ModelState);
        LoginResponseModel ValidateLoginModel(LoginModel loginModel, ModelStateDictionary ModelState);
        Guid?  ValidateForgotPasswordModel(ForgotPasswordModel model, ModelStateDictionary ModelState);
        bool ValidateResetPasswordModel(ResetPasswordTokenModel model, ModelStateDictionary ModelState);
     
        bool VerifySetPasswordModel(SetPasswordModel model, ModelStateDictionary ModelState);
    }
}