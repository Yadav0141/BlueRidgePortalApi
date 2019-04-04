
using BlueRidgeUtility.Security;
using BlueRidgeUtility.Validators;
using BlueRidgeUtility_BAL.Managers;
using BlueRidgeUtility_BAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Http;

namespace BlueRidgeUtility.Controllers
{
    [RoutePrefix("api/auth")]

    public class AuthenticationController : ApiController
    {
        private readonly string _appDomain = ConfigurationManager.AppSettings["JWT_DOMAIN"];
        private readonly string _jwtSecret = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
        IUserService _userService;
        IUserModelValidator _userModelValidator;
        public AuthenticationController(IUserService userService, IUserModelValidator userModelValidator)
        {
            _userService = userService;
            _userModelValidator = userModelValidator;
        }

        [Route("GetAllSourceDatabaseNames"), HttpGet]
        public IHttpActionResult GetAllSourceDatabaseNames()
        {
           


            return Ok("Success");

        }

        [Route("login"), HttpPost]
        public IHttpActionResult Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           var responseModel=   _userModelValidator.ValidateLoginModel(loginModel, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
       var jwtSecurityToken = new JwtSecurityToken
       (
           issuer: _appDomain,
           audience: _appDomain,
           claims: CreateClaims(responseModel),
           expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
           signingCredentials: _jwtSecret.ToIdentitySigningCredentials()
       );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            var responseMessage = Request.CreateResponse(responseModel);
            responseMessage.Headers.AddCookies(
                new[]
                    {
                        new CookieHeaderValue(
                           "api-jwt",
                           token) {HttpOnly = true, Path = "/"},
                       
                    });

            return ResponseMessage(responseMessage);

           
        }

        [Route("logout"), HttpGet]
        public IHttpActionResult LogOut()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = Request };
            responseMessage.Headers.AddCookies(
                new[]
                    {
                        new CookieHeaderValue(
                            "api-jwt",
                            "") {HttpOnly = true, Path = "/", Expires = DateTimeOffset.Now.AddDays(-1)},
                      
                    });

            return ResponseMessage(responseMessage);
        }

      

        private IEnumerable<Claim> CreateClaims(LoginResponseModel responseModel)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, responseModel.userId.ToString()),
            new Claim(ClaimTypes.Email, responseModel.userName),
            new Claim("user_object",JsonConvert.SerializeObject(responseModel))
        };
        return claims;
        }

        [Route("sendresetpasswordlink"), HttpPost]
        public IHttpActionResult SendResetPassword(ForgotPasswordModel model)
        {
          var userId=  _userModelValidator.ValidateForgotPasswordModel(model,ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          var result=  _userService.saveForgotPasswordRequest(userId);
            return Ok(result);
        }


        [Route("validateResetToken"), HttpPost]
        public IHttpActionResult validateResetToken(ResetPasswordTokenModel model)
        {
            var result = _userModelValidator.ValidateResetPasswordModel(model, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            return Ok(result);
        }


        [Route("setpassword"), HttpPost]
        public IHttpActionResult setpassword(SetPasswordModel model)
        {
            var result = _userModelValidator.VerifySetPasswordModel(model, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            result= _userService.setPassword(model);

            return Ok(result);
        }
    }
}