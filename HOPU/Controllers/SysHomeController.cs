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
        protected static IQueryable<Topic> GetTopicInfomation(int topicid)
        {
            HopuDBDataContext c = new HopuDBDataContext();
            var result = from p in c.Topic
                         where p.TopicID == topicid
                         select p;
            return result;
        }

        #endregion

        #region 分类列表 ClassifieTypedBrowse
        public ActionResult ClassifieTypedBrowse(int? Tid)
        {
            List<ClassifieTypedBrowseModel> topicType = GetTopicType(Tid ?? 1).ToList();
            ViewBag.topictype = topicType;
            return View();
        }

        private static IQueryable<ClassifieTypedBrowseModel> GetTopicType(int topicid)
        {
            //select distinct * from[dbo].[TypeInfo] a,[dbo].[Course] b where a.TID = b.TID
            HopuDBDataContext db = new HopuDBDataContext();
            var result = from p in db.TypeInfo
                         join c in db.Course on p.TID equals c.TID
                         orderby p.TID
                         select new ClassifieTypedBrowseModel
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
        public ActionResult ClassifieBrowse(int CourseId, string CourseName)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.Topic.Where(a => a.CourseID == CourseId).Select(b => b.TopicID).Max();//题目最大ID
            var result2 = db.Topic.Where(a => a.CourseID == CourseId).Select(b => b.TopicID).Min();//题目最小ID
            ViewBag.maxTopicId = result;
            ViewBag.minTopicId = result2;
            ViewBag.courseName = CourseName;
            return View();
        }
        #endregion

        #region 统测列表 UnifiedTestType
        public ActionResult UnifiedTestType(int? page)
        {
            //if (page == null)
            //{
            //    ViewBag.Js = "<script>alert(123456)</script>";
            //}
            var products = GetTestInfo().ToList();
            var pageNumber = page ?? 1;
            var onePage = products.ToPagedList(pageNumber, 10);
            ViewBag.uniteTest = onePage;

            List<ClassifieTypedBrowseModel> topicType = GetCourseInfo().ToList();
            ViewBag.topictype = topicType;

            return View();
        }

        protected static IQueryable<UniteTest> GetTestInfo()
        {

            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.UniteTest.Select(a => a).OrderByDescending(a => a.UtId);
            return result;
        }

        #endregion

        #region SubmitTest

        public ActionResult SubmitTest()
        {
            if (!IsAdmin())//如果不是admin权限
            {
                return RedirectToAction("Error");

                //return HttpNotFound();
            }
            //分类表
            List<ClassifieTypedBrowseModel> topicType = GetCourseInfo().ToList();
            ViewBag.topictype = topicType;
            return View();
        }
        #endregion

        #region 分类表 GetCourseInfo

        //获取分类表
        protected static IQueryable<ClassifieTypedBrowseModel> GetCourseInfo()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = from p in db.TypeInfo
                         join c in db.Course on p.TID equals c.TID
                         orderby p.TID
                         select new ClassifieTypedBrowseModel
                         {
                             TId = p.TID,
                             TypeName = p.TypeName,
                             CourseId = c.CourseID,
                             CourseName = c.CourseName
                         };
            return result;
        }

        #endregion

        #region 添加统测 AddTest
        //添加统测
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTest(string[] submitCheckbox, int topicCount, int timeLenth)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            if (!IsAdmin())//如果不是admin权限
            {
                return HttpNotFound();
            }
            else if (submitCheckbox.Count() == 0 || topicCount <= 0 || timeLenth <= 0)
            {
                return Json("不得留空！");
            }
            //生成where、courseName、CourseId的字符串
            string strCourseId = "", courseName = "", addSelectTopic = "";
            for (int i = 0; i < submitCheckbox.Length; i++)
            {
                strCourseId += submitCheckbox[i] + " ";
                addSelectTopic += "CourseId = " + submitCheckbox[i];
                //把courseName挨个儿查出来
                var dbCourseName = db.Course.Where(a => a.CourseID == Convert.ToDouble(submitCheckbox[i])).Select(a => a.CourseName).ToArray();
                courseName += dbCourseName[0];
                if (i < submitCheckbox.Length - 1)
                {
                    addSelectTopic += " or ";
                    courseName += "|";
                }
            }
            //以上步骤正常的话按要求获取题目
            List<UniteTestInfo> uniteTestInfos = new List<UniteTestInfo>();
            string selectTopicSql = "select top " + topicCount + " * from Topic where " + addSelectTopic + " order by NEWID()";
            DataTable dt = SQLHelper.GetTable(selectTopicSql);
            if (dt.Rows.Count < topicCount)//如果所取题目总数小于要求的数量
            {
                return Json("所填数量不得大于实际数量" + dt.Rows.Count);
            }
            //获取最大统测UtId
            int realMaxUtid = 0;
            var maxUtid = db.UniteTest.Select(a => a.UtId);
            if (maxUtid.Count() == 0)
            {
                realMaxUtid = 1;
            }
            else
            {
                realMaxUtid = Convert.ToInt32(maxUtid.Max() + 1);
            }
            //向uniteTestInfo表添加题目信息
            foreach (DataRow dr in dt.Rows)
            {
                UniteTestInfo u = new UniteTestInfo
                {
                    UtId = realMaxUtid,
                    Title = Convert.ToString(dr["Title"]),
                    TopicID = Convert.ToInt32(dr["TopicId"]),
                    AnswerA = Convert.ToString(dr["AnswerA"]),
                    AnswerB = Convert.ToString(dr["AnswerB"]),
                    AnswerC = Convert.ToString(dr["AnswerC"]),
                    AnswerD = Convert.ToString(dr["AnswerD"]),
                    Answer = Convert.ToString(dr["Answer"]),
                    CourseID = Convert.ToString(dr["CourseID"])
                };
                uniteTestInfos.Add(u);
            }
            var newTest = new UniteTest
            {
                UtId = realMaxUtid,
                StartTime = DateTime.Now,
                TimeLenth = timeLenth,
                TopicCount = topicCount,
                CourseId = strCourseId,
                CourseName = courseName
            };
            //以上步骤没问题就向uniteTest表中添加统测信息*2s
            db.UniteTest.InsertOnSubmit(newTest);
            db.UniteTestInfo.InsertAllOnSubmit(uniteTestInfos);
            db.SubmitChanges();
            //var list = db.Topic;
            //var where = PredicateBuilder.False<Topic>();
            //if (!string.IsNullOrEmpty(submitCheckbox[0]))
            //{
            //    double id = Convert.ToDouble(submitCheckbox[0]);
            //    where = where.Or(p => p.CourseID == id);
            //}
            //此处不知道怎么用linq to sql来实现，用ado.net来写
            //var result = list.Where(where.Compile()).Take(topicCount).OrderBy(x => Guid.NewGuid());

            return Json(new { Success = true });
        }
        #endregion

        #region 删除统测
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteTest(int UtId)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            if (!IsAdmin())//如果不是admin权限
            {
                return HttpNotFound();
            }
            var testInfo = db.UniteTestInfo.Where(a => a.UtId == UtId).Select(a => a);
            var test = db.UniteTest.SingleOrDefault(a => a.UtId == UtId);
            if (test == null || testInfo == null)
            {
                return Json(UtId + "不存在！");
            }
            db.UniteTestInfo.DeleteAllOnSubmit(testInfo);
            db.UniteTest.DeleteOnSubmit(test);
            db.SubmitChanges();
            return Json(new { Success = true });
        }
        #endregion

        #region 统测 UnifiedTest
        //public ActionResult UnifiedTest()
        //{
        //    return View();

        //}

        public ActionResult UnifiedTest(int? UtId)
        {
            bool joinUniteTest = false;
            if (UtId == null)
            {
                return HttpNotFound();
            }
            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.UniteTest.Where(a => a.UtId == UtId).Select(a => a);
            List<UniteTest> timeInfo = result.ToList();//时间等信息
            ViewBag.timeInfo = timeInfo;
            ViewBag.Title = UtId;
            //能否进入考试
            foreach (var item in result)
            {
                //如果结束时间大于当前时间，可以进入考试
                if (Convert.ToDateTime(item.StartTime).AddMinutes(item.TimeLenth) > DateTime.Now)
                {
                    joinUniteTest = true;
                    break;
                }
                else
                {

                    return RedirectToAction("UnifiedTestType");
                }

            }
            var UniteTestScoreInfo = db.UniteTestScore.Where(a => a.UtId == UtId && a.UserName == User.Identity.GetUserName()).FirstOrDefault();
            if (UniteTestScoreInfo == null)
            {
                joinUniteTest = true;
            }
            else
            {
                joinUniteTest = false;
            }
            //如果能加入统测
            if (joinUniteTest)
            {
                //据UtId获取题目列表
                int UserName = Convert.ToInt32(User.Identity.GetUserName());
                var topicListResult = db.UniteTestInfo.Where(a => a.UtId == UtId).ToList().Select(c =>
                new UniteTestInfo
                {
                    UtId = c.UtId,
                    Title = c.Title,
                    AnswerA = c.AnswerA,
                    AnswerB = c.AnswerB,
                    AnswerC = c.AnswerC,
                    AnswerD = c.AnswerD,
                    Answer = c.Answer,
                    CourseID = c.CourseID,
                    TopicID = (c.TopicID * (UserName - 200000000)) % 3 * 100,

                });
                if (UserName % 2 == 0)
                {
                    ViewBag.topicList = topicListResult.OrderBy(s => s.TopicID).ToList();
                }
                else
                {
                    ViewBag.topicList = topicListResult.OrderByDescending(s => s.TopicID).ToList();
                }
                return View();
            }
            else
            {
                return RedirectToAction("UnifiedTestType", new { page = "null" });
            }
        }
        #endregion

        #region 校验统测答案
        //校验答案
        [HttpPost]
        public JsonResult UnifiedTest(string[] Answer, int UtId)
        {
            //据UtId获取答案
            HopuDBDataContext db = new HopuDBDataContext();
            int UserName = Convert.ToInt32(User.Identity.GetUserName());
            var topicListResult = db.UniteTestInfo.Where(a => a.UtId == UtId).ToList().Select(c =>
            new UniteTestInfo
            {
                TopicID = (c.TopicID * (UserName - 200000000)) % 3 * 100,
                Answer = c.Answer,
            });
            List<UniteTestInfo> answerList = new List<UniteTestInfo>();
            if (UserName % 2 == 0)
            {
                answerList = topicListResult.OrderBy(s => s.TopicID).ToList();
            }
            else
            {
                answerList = topicListResult.OrderByDescending(s => s.TopicID).ToList();
            }
            //校验答案
            List<UnifiedTestModel> result = new List<UnifiedTestModel>();
            //先算出每题多少分 
            double itemScore = 100F / answerList.Count;
            double SumScore = 0;
            //校验答案
            for (int i = 0; i < answerList.Count; i++)
            {
                if (answerList[i].Answer.Equals(Answer[i]))
                {
                    UnifiedTestModel resultinfo = new UnifiedTestModel
                    {
                        UserAnswer = Answer[i],
                        RealAnswer = answerList[i].Answer,
                        IsTrue = true
                    };
                    result.Add(resultinfo);
                    SumScore += itemScore;
                }
                else
                {
                    UnifiedTestModel resultinfo = new UnifiedTestModel
                    {
                        UserAnswer = Answer[i],
                        RealAnswer = answerList[i].Answer,
                        IsTrue = false
                    };
                    result.Add(resultinfo);
                }
            }
            var UniteTestScoreInfo = db.UniteTestScore.Where(a => a.UtId == UtId && a.UserName == User.Identity.GetUserName()).FirstOrDefault();
            //如果没有提交过
            if (UniteTestScoreInfo == null)
            {
                //将考试结果存入数据库
                var uniteTestScore = new UniteTestScore
                {
                    UtId = UtId,
                    UserName = User.Identity.GetUserName(),
                    EndTime = DateTime.Now,
                    Score = Convert.ToInt32(Math.Round(SumScore, 0, MidpointRounding.AwayFromZero))
                };
                db.UniteTestScore.InsertOnSubmit(uniteTestScore);
                db.SubmitChanges();
            }
            else
            {
                return Json(null);
            }
            return Json(result);
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

        #region 关于 About
        public ActionResult About()
        {
            //test github
            return View("About");
        }
        #endregion


    }
    #region Linq To Sql 动态条件 PredicateBuilder 

    public static class PredicateBuilder
    {

        /// <summary>
        /// 机关函数应用True时：单个AND有效，多个AND有效；单个OR无效，多个OR无效；混应时写在AND后的OR有效  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        /// <summary>
        /// 机关函数应用False时：单个AND无效，多个AND无效；单个OR有效，多个OR有效；混应时写在OR后面的AND有效  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
    #endregion
}