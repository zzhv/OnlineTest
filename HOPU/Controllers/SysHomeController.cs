using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HOPU.Models;

namespace HOPU.Controllers
{
    [Authorize]
    public class SysHomeController : Controller
    {
        private HopuDBDataContext db = new HopuDBDataContext();


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
        //select avg(Score) svoreAvg,b.RealUserName from  UniteTestScore a,AspNetUsers b where a.UserName=b.UserName group by b.RealUserName order by avg(Score)

        public JsonResult GetSScoreAvg()
        {
            using (db)
            {
                string sql =
                    "select avg(Score) number,b.RealUserName from  SelfTestScore a,AspNetUsers b where a.UserName=b.UserName group by b.RealUserName";
                var selfTestScoreAvg = db.ExecuteQuery<UGetSScoreAvgModel>(sql).ToList().OrderByDescending(s => s.Number).Take(10);
                return Json(selfTestScoreAvg, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetUHYinfo()
        {
            var SScoreAvgList = new List<UGetSScoreAvgModel>
            {
                new UGetSScoreAvgModel{ Number = 100,RealUserName = "张1"},
                new UGetSScoreAvgModel{ Number =99,RealUserName = "张2"},
                new UGetSScoreAvgModel{ Number =88,RealUserName = "张3"},
                new UGetSScoreAvgModel{ Number =77,RealUserName = "张4"},
                new UGetSScoreAvgModel{ Number =98,RealUserName = "张5"},
                new UGetSScoreAvgModel{ Number =65,RealUserName = "张六"},
                new UGetSScoreAvgModel{ Number =15,RealUserName = "张七"},
                new UGetSScoreAvgModel{ Number =78,RealUserName = "张八"},
                new UGetSScoreAvgModel{ Number =12,RealUserName = "张九"},
                new UGetSScoreAvgModel{ Number =79,RealUserName = "张拾"}
            };
            return Json(SScoreAvgList, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}