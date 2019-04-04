using BlueRidgeUtility_BAL.Enums;
using BlueRidgeUtility_BAL.Models;
using BlueRidgeUtility_BAL.SelectListItems;
using System;
using System.Collections.Generic;

namespace BlueRidgeUtility_BAL.Managers
{
    public interface IUserService
    {
        List<SelectListItem> getDesignationsSelectList();
        List<SelectListItem> getUsersSelectList();
        bool isEmailIdAlreadyExist(UserModel userModel);
        bool isEmployeeCodeExist(UserModel userModel);
        Guid? AddEditUser(UserModel userModel);
        UserListViewModel GetAllUsersList(int pageNumber, int pageCount);
        UserModel GetUserById(Guid userId);

        AddEditUserDocumentModel getDocument(Guid userId, int docTypeId);
        LoginValidateResponseCode ValidateLoginModel(LoginModel loginModel, ref LoginResponseModel responseModel);

        ForgotPasswordResponseCode ValidateForgotPassword(ForgotPasswordModel model, ref Guid? userId);
        bool saveForgotPasswordRequest(Guid? userId);
        bool ValidateResetPasswordModel(ResetPasswordTokenModel model);
        bool setPassword(SetPasswordModel model);
        bool VerifySetPasswordModel(SetPasswordModel model);

        bool DeleteUser(Guid? userId);
    }
}