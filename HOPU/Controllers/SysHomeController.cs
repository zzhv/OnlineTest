using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HOPU.Models;

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

        #region 排行榜

        public JsonResult GetSScoreAvg()
        {
            var SScoreAvgList = new List<SelfTestScore>
            {
                new SelfTestScore{ Score =100,RealUserName = "张一"},
                new SelfTestScore{ Score =99,RealUserName = "张二"},
                new SelfTestScore{ Score =88,RealUserName = "张三"},
                new SelfTestScore{ Score =77,RealUserName = "张四"},
                new SelfTestScore{ Score =98,RealUserName = "张五"},
                new SelfTestScore{ Score =65,RealUserName = "张六"},
                new SelfTestScore{ Score =15,RealUserName = "张七"},
                new SelfTestScore{ Score =78,RealUserName = "张八"},
                new SelfTestScore{ Score =12,RealUserName = "张九"},
                new SelfTestScore{ Score =79,RealUserName = "张拾"}
            };
            return Json(SScoreAvgList,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUHYinfo()
        {
            var SScoreAvgList = new List<SelfTestScore>
            {
                new SelfTestScore{ Score =100,RealUserName = "张1"},
                new SelfTestScore{ Score =99,RealUserName = "张2"},
                new SelfTestScore{ Score =88,RealUserName = "张3"},
                new SelfTestScore{ Score =77,RealUserName = "张4"},
                new SelfTestScore{ Score =98,RealUserName = "张5"},
                new SelfTestScore{ Score =65,RealUserName = "张六"},
                new SelfTestScore{ Score =15,RealUserName = "张七"},
                new SelfTestScore{ Score =78,RealUserName = "张八"},
                new SelfTestScore{ Score =12,RealUserName = "张九"},
                new SelfTestScore{ Score =79,RealUserName = "张拾"}
            };
            return Json(SScoreAvgList, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}