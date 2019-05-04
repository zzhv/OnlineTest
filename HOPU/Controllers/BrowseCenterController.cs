using HOPU.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HOPU.Controllers
{
    [Authorize]
    public class BrowseCenterController : Controller
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        #region 顺序浏览 AllBrowse

        public ActionResult AllBrowse()
        {
            return View("AllBrowse");
        }


        [HttpPost]
        public JsonResult AllBrowse(int? Tid)
        {
            List<Topic> list = GetTopicInfomation(Tid ?? 1).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        protected static IEnumerable<Topic> GetTopicInfomation(int topicid)
        {
            HopuDBDataContext c = new HopuDBDataContext();
            return c.Topic.Where(x => x.TopicID == topicid).Select(x => x);
        }

        #endregion

        #region 分类列表 ClassifieTypedBrowse
        public ActionResult ClassifieTypedBrowse()
        {
            var vm = new CourseNameListViewModel
            {
                CourseName = GetTopicType().ToList(),
                TypeName = db.TypeInfo.Select(x => x).ToList()
            };
            return View(vm);
        }

        private static IQueryable<CourseNameViewModel> GetTopicType()
        {
            //select distinct * from[dbo].[TypeInfo] a,[dbo].[Course] b where a.TID = b.TID
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

        #region 分类浏览 ClassifieBrowse
        public ActionResult ClassifieBrowse(int courseId, string courseName)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.Topic.Where(a => a.CourseID == courseId).Select(b => b.TopicID).Max();//题目最大ID
            var result2 = db.Topic.Where(a => a.CourseID == courseId).Select(b => b.TopicID).Min();//题目最小ID
            ViewBag.maxTopicId = result;
            ViewBag.minTopicId = result2;
            ViewBag.courseName = courseName;
            return View();
        }
        #endregion

        #region 分类表 GetCourseInfo

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

        #region 权限检测 IsAdmin?

        public bool IsAdmin()
        {
            if (GetUserType.GetUserTypeInfo(User.Identity.GetUserId()))//如果是admin权限
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        #endregion
    }
}