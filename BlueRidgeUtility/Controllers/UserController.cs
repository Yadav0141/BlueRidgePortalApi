using BlueRidgeUtility.Validators;
using BlueRidgeUtility_BAL.Enums;
using BlueRidgeUtility_BAL.Managers;
using BlueRidgeUtility_BAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BlueRidgeUtility.Controllers
{
    [Authorize]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        IUserModelValidator _userModelValidator;
        IUserService _userService;
        public UserController(IUserModelValidator userModelValidator, IUserService userService) {
            _userModelValidator = userModelValidator;
            _userService = userService;
        }
        [HttpGet]
        [Route("GetAllUsers")]
        public IHttpActionResult GetUsersList(int pageNumber,int pageCount) {

            var result = _userService.GetAllUsersList(pageNumber, pageCount);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Data Not Found.");
            }
        }

        [HttpPost]
        [Route("AddEditUser")]
        public IHttpActionResult AddEditUser()
        {
            var model = HttpContext.Current.Request.Form["model"];
            var files = HttpContext.Current.Request.Files;
           if (String.IsNullOrEmpty(model) || String.IsNullOrWhiteSpace(model))
            {
                return BadRequest("Model not passed.");
            }
            UserModel userModel = JsonConvert.DeserializeObject<UserModel>(model);
            Validate(userModel);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            _userModelValidator.ValidateUserModel(userModel,ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            userModel.lstDocuments = new List<AddEditUserDocumentModel>();
            if (files?.Count > 0)
            {
               
                var panCardFile = files["FileType-1"];
                if (panCardFile != null)
                {
                    using (var binaryReader = new BinaryReader(panCardFile.InputStream))
                    {
                        var panCardImage = new AddEditUserDocumentModel();

                        panCardImage.document = binaryReader.ReadBytes(panCardFile.ContentLength);
                        panCardImage.documentExt = Path.GetExtension(panCardFile.FileName);
                        panCardImage.documentName = panCardFile.FileName;
                        panCardImage.documentTypeId = Convert.ToInt32(DocumentType.PAN_Card);
                        userModel.lstDocuments.Add(panCardImage);
                    }
                   
                }
               
                var aadhaarCardFile = files["FileType-2"];
                if (aadhaarCardFile != null)
                {
                    using (var binaryReader = new BinaryReader(aadhaarCardFile.InputStream))
                    {
                       
                        var aadhaarCardImage = new AddEditUserDocumentModel();

                        aadhaarCardImage.document = binaryReader.ReadBytes(aadhaarCardFile.ContentLength);
                        aadhaarCardImage.documentExt = Path.GetExtension(aadhaarCardFile.FileName);
                        aadhaarCardImage.documentName = aadhaarCardFile.FileName;
                        aadhaarCardImage.documentTypeId = Convert.ToInt32(DocumentType.Aadhaar_Card);
                        userModel.lstDocuments.Add(aadhaarCardImage);
                    }
                }
            }
            var userId=  _userService.AddEditUser(userModel);
            if (userId == null)
            {
                ModelState.AddModelError("notadded", "User not able to registered.");
                return BadRequest(ModelState);
            }


            return Ok(userId);
        }

        [HttpGet]
        [Route("GetAllSelectListForAddEditUser")]
        public IHttpActionResult GetAllSelectListForAddEditUser() {
            AddEditSelectListModel responseModel = new AddEditSelectListModel();
            responseModel.roleList = _userService.getDesignationsSelectList();
            responseModel.userList = _userService.getUsersSelectList();
            if(responseModel.userList.Count==0)
            {
                responseModel.userList = new List<BlueRidgeUtility_BAL.SelectListItems.SelectListItem> { new BlueRidgeUtility_BAL.SelectListItems.SelectListItem { text="ABC",value=Guid.NewGuid().ToString() }};

            }
            return Ok(responseModel);
        }

        [HttpGet]
        [Route("GetUserById")]
        public IHttpActionResult GetUserById(Guid userId)
        {
           var userModel=  _userService.GetUserById(userId);
            if (userModel != null)
            {
                return Ok(userModel);
            }
            else
            {
                return BadRequest("Data Not Found.");
            }
        }


        [Route("DeleteUser"), HttpDelete]
        public IHttpActionResult DeleteUser(Guid? userId)
        {
            if (userId == null)
            {
                return BadRequest("User id not passed.");

            }
            var result = _userService.DeleteUser(userId);
            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("User not able to delete.Please check.");
            }
            
        }

    }
}