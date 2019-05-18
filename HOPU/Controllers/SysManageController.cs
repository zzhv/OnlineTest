using HOPU.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using HOPU.Services;

namespace HOPU.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SysManageController : Controller
    {
        private HopuDBDataContext db = new HopuDBDataContext();
        private readonly IBTTable _bTTableInfo;
        private readonly ICourse _course;
        private readonly ITopic _topic;
        private readonly ITypeinfo _typeinfo;
        private readonly ISelfTest _selfTest;

        public SysManageController(IBTTable bTTableInfo, ICourse course, ITopic topic, ITypeinfo typeinfo, ISelfTest selfTest)
        {
            _bTTableInfo = bTTableInfo;
            _course = course;
            _topic = topic;
            _typeinfo = typeinfo;
            _selfTest = selfTest;
        }

        public ActionResult SysManageIndex()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetTodaySelfTestCount()
        {
            return Json(_selfTest.GetTodaySelfTestCount());
        }


        public ActionResult TopicManage()
        {
            IEnumerable<SelectListItem> CourseTypeList = _course.GetSelectListItemOfCourseType();
            //给Table中的CourseID列提供数据
            ViewBag.CourseTypeJson = JsonConvert.SerializeObject(CourseTypeList.Select(a => new
            {
                value = Convert.ToInt32(a.Value),
                text = a.Text
            }));
            return View(CourseTypeList);
        }

        #region GetTopic
        /// <summary>
        /// 给题目管理的TopicTable提供数据
        /// </summary>
        /// <param name="limit">页面大小</param>
        /// <param name="offset">页码</param>
        /// <param name="keyword">搜索关键字</param>
        /// <param name="sortOrder">排序方式</param>
        /// <param name="sortName">排序字段</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetTopic(int limit, int offset, string keyword, string sortOrder, string sortName)
        {
            var result = _bTTableInfo.GetTableInfo<Topic>(GetTopicSqlStr(limit, offset, keyword, sortOrder, sortName)).ToList();
            var totalq = result.Count;
            var rowsq = result.Skip(offset).Take(limit);
            return Json(new
            {
                total = totalq,
                rows = rowsq
            }, JsonRequestBehavior.AllowGet);
        }

        //给GetTopic()态生成Sql语句
        private static string GetTopicSqlStr(int limit, int offset, string keyword, string sortOrder, string sortName)
        {
            string sql = @"SELECT * FROM Topic";
            if (keyword != "")
            {
                var keywordList = keyword.Split(new char[2] { ',', '，' });
                //搜索条件
                for (int i = 0; i < keywordList.Count(); i++)
                {
                    sql += i == 0 ? " WHERE " : " AND ";
                    sql += @"CONCAT(TopicID, Title, AnswerA, AnswerB, AnswerC, AnswerD, Answer, CourseID) "
                           + "LIKE '%" + keywordList[i] + "%'";
                }
            }
            //排序
            if (sortName != "")
            {
                //排序条件
                sql += @" ORDER BY '" + sortName + "' " + sortOrder;
            }
            return sql;
        }

        #endregion

        /// <summary>
        /// 单项题目的编辑
        /// </summary>
        /// <param name="topic">Topic对象</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditTopic(Topic topic)
        {
            if (_topic.EditTopic(topic))
                return Json("success");
            return Json(false);
        }

        /// <summary>
        /// 单项删除
        /// </summary>
        /// <param name="topic">topic对象</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteTopic(Topic topic)
        {
            if (_topic.DeleteTopic(topic))
                return Json("success");
            return Json(false);
        }

        /// <summary>
        /// 多选删除
        /// </summary>
        /// <param name="topics">题目题号的数组</param>                               
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteAllTopic(double[] topics)
        {
            if (_topic.DeleteAllTopic(topics))
                return Json(true);
            return Json(false);
        }

        /// <summary>
        /// 从文件导入题目
        /// </summary>
        /// <param name="files">xls文件</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TopicManage(IEnumerable<HttpPostedFileBase> files)
        {
            if (files == null || files.Count() == 0 || files.ToList()[0] == null)
            {
                ViewBag.ErrorMessage = "请选择文件！";
                ViewBag.MtClick = "$('#mtBtn').click()";//让模态框弹出来
                return View();
            }
            string filePath = string.Empty;
            Guid gid = Guid.NewGuid();
            foreach (HttpPostedFileBase file in files)
            {
                //filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"), gid.ToString() + Path.GetExtension(file.FileName));
                filePath = AppDomain.CurrentDomain.BaseDirectory + "Uploads\\" + gid.ToString() + Path.GetExtension(file.FileName);
                file.SaveAs(filePath);
            }
            var topics = Tools.DataSetToList.DataSetToIList<Topic>(Tools.ExcelToDS.excelToDS(filePath), "Topics");
            //System.IO.File.Delete(filePath);
            try
            {
                ViewBag.MtClick = "$('#mtBtn').click()";//让模态框弹出来
                db.Topic.InsertAllOnSubmit(topics);
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View();
            }
            ViewBag.data = topics;
            return View(_course.GetSelectListItemOfCourseType());
        }


        /// <summary>
        /// 添加单个题目
        /// </summary>
        /// <param name="topic">Topic对象</param>
        /// <param name="Answer">题目答案，因为是数组的问题，单独获取</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult InsertOneTopic(Topic topic, string[] Answer)
        {
            if (_topic.InsertOneTopic(topic, Answer))
                return Json(new { Success = false, msg = "错误代码：没有" });
            return Json(new { Success = true, topic });
        }

        /// <summary>
        /// 为添加单题获取最大TopicID+1
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMaxTopicID()
        {
            return Json(_topic.GetMaxTopicID(1));
        }

        public ActionResult CourseTypeManage()
        {
            var Type = _course.GetSelectListItemOfTypeInfo();
            return View(Type);
        }

        [HttpPost]
        public JsonResult GetCourse(int limit, int offset, string keyword, string sortOrder, string sortName)
        {
            var result = _bTTableInfo.GetTableInfo<CourseNameViewModel>(GetCourseSqlStr(limit, offset, keyword, sortOrder, sortName)).ToList();
            var totalq = result.Count;
            var rowsq = result.Skip(offset).Take(limit);
            return Json(new
            {
                total = totalq,
                rows = rowsq
            }, JsonRequestBehavior.AllowGet);
        }

        private static string GetCourseSqlStr(int limit, int offset, string keyword, string sortOrder, string sortName)
        {
            //select c.*,t.TypeName FROM TypeInfo t,Course c WHERE t.TID=c.TID 
            string sql = @"SELECT c.*,t.TypeName FROM TypeInfo t,Course c";
            if (keyword != "")
            {
                var keywordList = keyword.Split(new char[2] { ',', '，' });
                //搜索条件
                for (int i = 0; i < keywordList.Count(); i++)
                {
                    sql += i == 0 ? " WHERE t.TID = c.TID " : " AND ";
                    sql += @" AND CONCAT(c.CourseID, c.CourseName, c.TID, t.TypeName) "
                           + "LIKE '%" + keywordList[i] + "%'";
                }
            }
            //排序
            if (sortName != "")
            {
                //排序条件
                if (keyword == "")
                {
                    sql += " WHERE t.TID = c.TID";
                };
                sql += @" ORDER BY '" + sortName + "' " + sortOrder;
            }
            return sql;
        }

        [HttpPost]
        public JsonResult EditCourse(CourseNameViewModel course)
        {
            if (_course.EditCourseNameAndTypeName(course))
            {
                return Json(true);
            }
            return Json(false);

        }

        [HttpPost]
        public JsonResult DeleteCourse(CourseNameViewModel course)
        {
            if (_course.DeleteCourse(course))
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddTypeAndCourse(CourseNameViewModel course)
        {
            if (_course.AddTypeAndCourse(course))
            {
                return Json(new { row = course, flag = true });
            }
            return Json(new { msg = "错误代码：没有", flag = false });
        }

        [HttpPost]
        public JsonResult GetMaxCourseId_TID()
        {
            return Json(new { MaxCourseID = _course.MaxCourseID() + 1, MaxTID = _course.MaxTID() + 1 });
        }

        [HttpPost]
        public JsonResult GetMaxCourseId()
        {
            return Json(_course.MaxCourseID() + 1);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddOneCourse(CourseNameViewModel course)
        {
            if (_course.AddOneCourse(course))
            {
                return Json(new { row = course, flag = true });
            }
            return Json(new { msg = "错误代码：没有", flag = false });
        }
    }
}
