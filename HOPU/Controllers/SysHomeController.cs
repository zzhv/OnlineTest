using System.Web.Mvc;

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