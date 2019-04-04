using BlueRidgeUtility_BAL.Enums;
using BlueRidgeUtility_BAL.Helpers;
using BlueRidgeUtility_BAL.Mappers;
using BlueRidgeUtility_BAL.Models;
using BlueRidgeUtility_BAL.SelectListItemModels;
using BlueRidgeUtility_BAL.SelectListItems;
using BlueRidgeUtility_DAL;
using BlueRidgeUtility_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlueRidgeUtility_BAL.Managers
{
   public class UserService : IUserService
    {
        IPasswordHasher _passwordHasher;
        IUrlTokenGenerator _urlTokenGenerator;
        IEmailService _emailService;
        string SP_GetAllUsers = "usp_GetAllUsers";
        string FORGOT_PASSWORD_LINK = ConfigurationManager.AppSettings["FORGOT_PASSWORD_LINK"];
        public UserService(IPasswordHasher passwordHasher,IUrlTokenGenerator urlTokenGenerator,
             IEmailService emailService) {
            _passwordHasher = passwordHasher;
            _urlTokenGenerator = urlTokenGenerator;
            _emailService = emailService;
        }
        public List<SelectListItem> getDesignationsSelectList() {
            using (var context = new BlueRidgeUtilityDBContext()) {
                var roles= context.TblRoles.AsNoTracking().Select(role =>new RoleSelectListModel { roleId=role.roleId,roleDescription=role.roleDescription })?.ToList();
                if (roles != null)
                {
                    return Mapping.Mapper.Map<List<RoleSelectListModel>, List<SelectListItem>>(roles);
                }
                else
                {
                    return null;
                }
            }
               
        }

        public List<SelectListItem> getUsersSelectList() {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var users = context.TblUsers.AsNoTracking().Where(x=>x.isDeleted!=true).Select
                    (user => 
                    new UserSelectListItemModel { userId = user.userId.ToString(),
                        userName = user.firstName+" "+user.lastName }
                    )?.ToList();

                if (users != null)
                {
                    return Mapping.Mapper.Map<List<UserSelectListItemModel>, List<SelectListItem>>(users);
                }
                else
                {
                    return null;
                }
            }
        }


        public bool isEmailIdAlreadyExist(UserModel userModel) {

            using (var context = new BlueRidgeUtilityDBContext())
            {
                userModel.emailId = userModel.emailId.ToLower();
                return context.TblUsers.AsNoTracking().Any(user => user.userId != userModel.userId && user.emailId.ToLower() == userModel.emailId);
            }
               
        }

        public bool isEmployeeCodeExist(UserModel userModel)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                userModel.employeeId = userModel.employeeId.ToLower();
                return context.TblUsers.AsNoTracking().Any(user => user.userId != userModel.userId && user.employeeId.ToLower() == userModel.employeeId);
            }
        }

        public Guid? AddEditUser(UserModel userModel) {
            var isUserModelValid = false;
            using (var context = new BlueRidgeUtilityDBContext())
            {
                if (userModel.userId == null)
                {
                    tblUser tblUser = Mapping.Mapper.Map<tblUser>(userModel);

                    tblUser.userId = Guid.NewGuid();
                    if (!(String.IsNullOrEmpty(userModel.password) && String.IsNullOrWhiteSpace(userModel.password)))
                     {
                     var hashPassworModel=   _passwordHasher.HashPassword(userModel.password);
                     tblUser.password = hashPassworModel.Hash;
                     tblUser.passwordSalt = hashPassworModel.Salt;

                    }
                    tblUser.createdDate = DateTime.Now;
                    tblUser.isDeleted = false;
                    context.TblUsers.Add(tblUser);
                    context.SaveChanges();
                    isUserModelValid = true;
                    userModel.userId = tblUser.userId;
                }
                else
                {
                    tblUser tblUser = context.TblUsers.FirstOrDefault(user => user.userId == userModel.userId);
                    if (tblUser != null)
                    {
                        tblUser=Mapping.Mapper.Map(userModel, tblUser);
                        if (!(String.IsNullOrEmpty(userModel.password) && String.IsNullOrWhiteSpace(userModel.password)))
                        {
                            var hashPassworModel = _passwordHasher.HashPassword(userModel.password);
                            tblUser.password = hashPassworModel.Hash;
                            tblUser.passwordSalt = hashPassworModel.Salt;

                        }
                        tblUser.modifiedDate = DateTime.Now;
                        tblUser.isDeleted = false;
                        context.Entry(tblUser).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        userModel.userId = tblUser.userId;
                        isUserModelValid = true;
                    }
                    else
                    {
                        return null;
                    }
                }
                if (isUserModelValid == true)
                {
                    if (userModel.lstDocuments.Count > 0)
                    {
                        userModel.lstDocuments.ForEach(document =>
                        {
                            document.userId = (Guid)userModel.userId;
                            AddEditUserDocument(document);
                        });
                      
                    }
                      
                }
            }
            return userModel.userId;
        }

       
     

        private int AddEditUserDocument(AddEditUserDocumentModel addEditUserDocumentModel)
        {
            int documentId = 0;
            
            using (var context = new BlueRidgeUtilityDBContext())
            {
                tblDocument tblUserDocument = context.TblDocuments.FirstOrDefault(doc => doc.userId==addEditUserDocumentModel.userId && doc.isDeleted != true && doc.documentTypeId == addEditUserDocumentModel.documentTypeId);
                if (tblUserDocument != null)
                {
                    tblUserDocument = Mapping.Mapper.Map(addEditUserDocumentModel, tblUserDocument);
                    tblUserDocument.modifiedDate = DateTime.Now;
                    context.Entry(tblUserDocument).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    documentId = tblUserDocument.documentid;

                }
                else
                {
                    tblUserDocument = Mapping.Mapper.Map<tblDocument>(addEditUserDocumentModel);
                    tblUserDocument.createdDate = DateTime.Now;
                    context.TblDocuments.Add(tblUserDocument);
                    context.SaveChanges();
                    documentId = tblUserDocument.documentid;

                }
            }
            return documentId;
        }


        
        public UserListViewModel GetAllUsersList(int pageNumber,int pageCount)
        {

            using (var context = new BlueRidgeUtilityDBContext())
            {
                var totalCount = context.TblUsers.AsNoTracking().Count(user => user.isDeleted != true);
                if (totalCount > 0)
                {
                   var lstUsers = context.Database.SqlQuery<UserListModel>(SP_GetAllUsers).Skip((pageNumber-1)* pageCount).Take(pageCount).ToList();
                    UserListViewModel viewModel = new UserListViewModel();
                    viewModel.lstUsers = lstUsers;
                    viewModel.totalCount = totalCount;
                    return viewModel;
                }
               
            }
                return null;
        }
        public UserModel GetUserById(Guid userId)
        {
            using (var context=new BlueRidgeUtilityDBContext())
            {
                var tblUser = context.TblUsers.AsNoTracking().FirstOrDefault(x => x.userId == userId);
                if (tblUser != null)
                {
                  var userModel=  Mapping.Mapper.Map<UserModel>(tblUser);
                    userModel.panCardFileName=(from document in context.TblDocuments.AsNoTracking()
                                               where (document.userId == userModel.userId && document.documentTypeId == (int)DocumentType.PAN_Card)
                                                select document.documentName).FirstOrDefault();
                   
                    userModel.aadhaarCardFileName = (from document in context.TblDocuments.AsNoTracking()
                                                     where (document.userId == userModel.userId && document.documentTypeId == (int)DocumentType.Aadhaar_Card)
                                                     select document.documentName).FirstOrDefault();
                    return userModel;
                }
               
            }
            return null;
        }

        public AddEditUserDocumentModel getDocument(Guid userId, int docTypeId)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
               var documentModel= context.TblDocuments.AsNoTracking().FirstOrDefault(x => x.userId == userId && x.documentTypeId== docTypeId);
                if (documentModel != null)
                {
                    return Mapping.Mapper.Map<AddEditUserDocumentModel>(documentModel);
                }
                else
                {
                    return null;
                }
            }
           
        }

        public LoginValidateResponseCode ValidateLoginModel(LoginModel loginModel,ref LoginResponseModel responseModel) {

            using (var context =new BlueRidgeUtilityDBContext())
            {
               
              var user=  context.TblUsers.AsNoTracking().FirstOrDefault(x => x.emailId == loginModel.username && x.isDeleted!=true);
                if (user != null)
                {
                    if (String.IsNullOrEmpty(user.password) || String.IsNullOrWhiteSpace(user.password))
                    {
                        return LoginValidateResponseCode.PASSWORD_NOT_SET;
                    }
                 var hashedPassword=   _passwordHasher.HashPassword(loginModel.password, user.passwordSalt);
                    if (String.Equals(user.password, hashedPassword, StringComparison.OrdinalIgnoreCase))
                    {
                        responseModel.userId = user.userId;
                        responseModel.superVisorId = user.superVisorId;
                        responseModel.firstName = user.firstName;
                        responseModel.lastName = user.lastName;
                        responseModel.userName = loginModel.username;
                        if (user.roleId != null)
                        {
                            responseModel.roleId = user.roleId;
                            responseModel.roleDescription = context.TblRoles.FirstOrDefault(role => role.roleId == user.roleId)?.roleDescription;
                            
                        }
                        responseModel.modules = (from permission in context.TblPermissions
                                                 join modules in context.TblModules on permission.moduleId equals modules.moduleId
                                                 join users in context.TblUsers on permission.roleId equals users.roleId
                                                 where user.userId == user.userId
                                                 select modules.moduleName)?.ToArray();

                        

                        return LoginValidateResponseCode.USER_LOGIN_SUCCESS;
                    }
                    else
                    {
                        return LoginValidateResponseCode.INVALID_USERNAME_PASSWORD;
                    }
                }
              
            }
            return LoginValidateResponseCode.USER_NOT_EXIST;
        }
       
        public ForgotPasswordResponseCode ValidateForgotPassword(ForgotPasswordModel model,ref Guid? userId)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var user = context.TblUsers.FirstOrDefault(x => x.emailId == model.emailId && x.isDeleted!=true);
                if (user != null)
                {
                    userId = user.userId;
                    return ForgotPasswordResponseCode.USER_EXIST;
                }
                else
                {
                    return ForgotPasswordResponseCode.USER_NOT_FOUND;
                }
               
            }
        }

       

        public bool saveForgotPasswordRequest(Guid? userId)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var user = context.TblUsers.FirstOrDefault(x => x.userId == userId && x.isDeleted != true);
                if (user != null)
                {
                    user.setPasswordToken = _urlTokenGenerator.GenerateUrlToken();
                    user.setTokenExpirationDate= DateTime.UtcNow.AddDays(1);
                    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    ForgotPasswordEmailModel emailmodel = new ForgotPasswordEmailModel();
                    emailmodel.name = $"{user.firstName} {user.lastName}";
                    emailmodel.link = $"{FORGOT_PASSWORD_LINK}{ user.setPasswordToken}";
                    emailmodel.toEmailId = user.emailId;
                    emailmodel.subject = "Reset Password Email";
                    Thread t = new Thread(() => _emailService.sendResetPasswordEmail(emailmodel));
                    t.Start();
                    return true;
                }

            }
            return false;
        }

        public bool ValidateResetPasswordModel(ResetPasswordTokenModel model)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
               return context.TblUsers.Any(x => x.setPasswordToken == model.resetRequestToken &&
                   x.setTokenExpirationDate >= DateTimeOffset.UtcNow && x.isDeleted != true);
               

            }
        }

        public bool VerifySetPasswordModel(SetPasswordModel model)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                return context.TblUsers.Any(x => x.setPasswordToken == model.resetRequestToken &&
                    x.setTokenExpirationDate >= DateTimeOffset.UtcNow && x.isDeleted != true);


            }
        }

        public bool setPassword(SetPasswordModel model)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var user= context.TblUsers.FirstOrDefault(x => x.setPasswordToken == model.resetRequestToken &&
                    x.setTokenExpirationDate >= DateTimeOffset.UtcNow && x.isDeleted != true);
                if (user != null)
                {
                    var hashedPassword = _passwordHasher.HashPassword(model.password);
                    user.setPasswordToken = null;
                    user.setTokenExpirationDate = null;
                    user.password = hashedPassword.Hash;
                    user.passwordSalt = hashedPassword.Salt;
                    user.modifiedDate = DateTime.Now;
                    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }


            }
        }

        public bool DeleteUser(Guid? userId)
        {
            using (var context = new BlueRidgeUtilityDBContext())
            {
                var user = context.TblUsers.FirstOrDefault(x => x.userId == userId && x.isDeleted != true);
                if (user != null)
                {

                    user.isDeleted = true; ;
                    user.modifiedDate = DateTime.Now;
                    context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
            }
               
            return false;
        }
    }
} 
