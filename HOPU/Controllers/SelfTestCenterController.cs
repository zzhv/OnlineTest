using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HOPU.Models;
using X.PagedList;

namespace HOPU.Controllers
{
    [Authorize]
    public class SelfTestCenterController : Controller
    {
        // GET: SeltTestCenter
        public ActionResult SelfTest()
        {
            return View();
        }

        #region 独测列表 SelfTestType
        public ActionResult SelfTestType(int? page)
        {
            var products = GetTestInfo().ToList();
            var pageNumber = page ?? 1;
            var onePage = products.ToPagedList(pageNumber, 5);
            var list = GetCourseInfo().ToList();
            var vms = list.Select(x => new CourseNameViewModel
            {
                CourseId = x.CourseId,
                CourseName = x.CourseName,
                TypeName = x.TypeName,
                TId = x.TId
            });
            var vmu = onePage.Select(x => new SelfTest()
            {
                StId = x.StId,
                StartTime = x.StartTime,
                TimeLenth = x.TimeLenth,
                TopicCount = x.TopicCount,
                CourseName = x.CourseName,
                CourseId = x.CourseName
            });
            var vm = new SelfTestViewModel
            {
                CourseNames = vms,
                SelfTest = vmu
            };
            return View(vm);
        }
        protected static IEnumerable<SelfTest> GetTestInfo()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.SelfTest.Select(a => a).OrderByDescending(a => a.StId);
            return result;
        }
        //获取分类表
        protected static IQueryable<CourseNameViewModel> GetCourseInfo()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = from p in db.TypeInfo
                         join c in db.Course on p.TID equals c.TID
                         orderby p.TID
                         select new CourseNameViewModel
                         {
                             TId = p.TID,
                             TypeName = p.TypeName,
                             CourseId = c.CourseID,
                             CourseName = c.CourseName
                         };
            return result;
        }
        #endregion
    }
}