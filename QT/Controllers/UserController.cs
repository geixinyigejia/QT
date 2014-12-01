using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GetLBAMVC.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TaskRecords()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        public ActionResult Suggestions()
        {
            return View();
        }
    }
}
