using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GetLBAMVC.Controllers
{
    public class ReceiveTaskController : Controller
    {
        //
        // GET: /ReceiveTask/

        public ActionResult Index()
        {
            return View();
        }   

        public ActionResult WaitToBeDone()
        {
            return View();
        }

        public ActionResult DoneTasks()
        {
            return View();
        }

       
    }
}
