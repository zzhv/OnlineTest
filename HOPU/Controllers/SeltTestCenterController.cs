using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOPU.Controllers
{
    public class SeltTestCenterController : Controller
    {
        // GET: SeltTestCenter
        public ActionResult SelfTest()
        {
            return View();
        }

        public ActionResult SelfTestType()
        {

            return View();
        }
    }
}