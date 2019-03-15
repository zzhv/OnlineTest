using System.Web.Mvc;
using HOPU.Models;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using X.PagedList;
using Microsoft.AspNet.Identity;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;
using System.Reflection;

namespace HOPU.Controllers
{
    [Authorize]

    public class SysHomeController : Controller
    {

        #region 首页 Home

        // GET: SysHome
        public ActionResult Index()
        {
            return View("Home");
        }

        public ActionResult Home()
        {

            return View();
        }
        #endregion

        #region 关于 About
        public ActionResult About()
        {
            //test github
            return View("About");
        }
        #endregion

    }
}