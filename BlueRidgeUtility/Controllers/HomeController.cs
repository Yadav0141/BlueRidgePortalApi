using BlueRidgeUtility_BAL.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueRidgeUtility.Controllers
{
    public class HomeController : Controller
    {
        IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public FileResult GetDocument(Guid userId,int docTypeId)
        {
           var documentModel= _userService.getDocument(userId, docTypeId);
            if (documentModel != null)
            {
                return File(documentModel.document, documentModel.content_type);
            }
            else
            {
                return null;
            }
        }

    }
}
