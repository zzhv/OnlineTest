using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using HOPU.Models;
using Microsoft.AspNet.Identity;
using X.PagedList;

namespace HOPU.Controllers
{
    [Authorize]
    public class SelfTestCenterController : Controller
    {
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
            var vm = new SelfTestTypeViewModel
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
        protected static IEnumerable<CourseNameViewModel> GetCourseInfo()
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

        #region 独测
        public ActionResult SelfTest(int? Id)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            int? StId = Id;
            ViewBag.Title = StId;
            bool joinSelfTest;
            if (StId == null)
            {
                return PartialView("Error");
            }
            var vmt = db.SelfTest.Where(a => a.StId == StId).ToList().Select(x => new SelfTest
            {
                StId = x.StId,
                StartTime = x.StartTime,
                TimeLenth = x.TimeLenth,
                TopicCount = x.TopicCount,
            });
            //以下综合判断能否进入考试 有一项不符合就直接拒绝
            if (!vmt.Any())
            {
                return HttpNotFound();
            }
            foreach (var item in vmt)
            {
                //如果结束时间大于当前时间，可以进入考试
                if (Convert.ToDateTime(item.StartTime).AddMinutes(item.TimeLenth) > DateTime.Now)
                {
                    break;
                }
                return RedirectToAction("Score", "ScoreCenter", new { Id });
            }
            //如果是第一次提交答案
            var SelfTestScoreInfo = db.SelfTestScore.Where(a => a.StId == StId && a.UserName == User.Identity.GetUserName()).FirstOrDefault();
            if (SelfTestScoreInfo == null)
            {
                joinSelfTest = true;
            }
            else
            {
                return RedirectToAction("Score", "ScoreCenter", new { Id });
            }
            //以上验证完成，如果能加入统测
            if (joinSelfTest)
            {
                //据StId获取题目列表
                //string UserName = User.Identity.GetUserName();
                var TopicInfo = db.SelfTestInfo.Where(a => a.StId == StId).ToList().Select(c =>
                    new SelfTestInfo()
                    {
                        StId = c.StId,
                        Title = c.Title,
                        AnswerA = c.AnswerA,
                        AnswerB = c.AnswerB,
                        AnswerC = c.AnswerC,
                        AnswerD = c.AnswerD,
                        Answer = c.Answer,
                        CourseID = c.CourseID,
                        TopicID = c.TopicID
                    });
                var vm = new SelfTestViewModel()
                {
                    TopicInfo = TopicInfo.OrderBy(x => x.TopicID),
                    TimeInfo = vmt
                };
                return View(vm);
            }
            return View();
        }
        #endregion

        #region 添加独测 AddTest
        //添加统测
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTest(string[] submitCheckbox, int topicCount, int timeLenth)
        {
            HopuDBDataContext db = new HopuDBDataContext();

            if (submitCheckbox.Count() == 0 || topicCount <= 0 || timeLenth <= 0)
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
            string selectTopicSql = "select top " + topicCount + " * from Topic where " + addSelectTopic + " order by NEWID()";
            var topicList = db.ExecuteQuery<Topic>(selectTopicSql).ToList();
            //DataTable dt = SQLHelper.GetTable(selectTopicSql);
            if (topicList.Count < topicCount)//如果所取题目总数小于要求的数量
            {
                return Json("所填数量不得大于实际数量" + topicList.Count);
            }
            //获取最大独测StId
            int realMaxStId;
            var maxStId = db.SelfTest.Select(a => a.StId);
            realMaxStId = !maxStId.Any() ? 1 : Convert.ToInt32(maxStId.Max() + 1);
            //向SelfTestInfo表添加题目信息
            List<SelfTestInfo> SelfTestInfos = new List<SelfTestInfo>();
            foreach (var s in topicList)
            {
                SelfTestInfo u = new SelfTestInfo
                {
                    StId = realMaxStId,
                    Title = s.Title,
                    TopicID = Convert.ToInt32(s.TopicID),
                    AnswerA = s.AnswerA,
                    AnswerB = s.AnswerB,
                    AnswerC = s.AnswerC,
                    AnswerD = s.AnswerD,
                    Answer = s.Answer,
                    CourseID = s.CourseID.ToString()
                };
                SelfTestInfos.Add(u);
            }
            //想SelfTest表添加统测信息
            var newTest = new SelfTest
            {
                StId = realMaxStId,
                StartTime = DateTime.Now,
                TimeLenth = timeLenth,
                TopicCount = topicCount,
                CourseId = strCourseId,
                CourseName = courseName
            };
            db.SelfTest.InsertOnSubmit(newTest);
            db.SelfTestInfo.InsertAllOnSubmit(SelfTestInfos);
            db.SubmitChanges();
            return Json(new { Success = true });
        }
        #endregion

        #region 删除统测
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteTest(int StId)
        {
            using (var db = new HopuDBDataContext())
            {
                //把要删除的统测题目查出来
                var testInfo = db.SelfTestInfo.Where(a => a.StId == StId).Select(a => a);
                //把要删除的统测查出来
                var test = db.SelfTest.SingleOrDefault(a => a.StId == StId);
                //把要删除的成绩信息查出来
                var scoreInfo = db.SelfTestScore.Where(a => a.StId == StId);
                if (test == null || testInfo == null)
                {
                    return Json(StId + "不存在！");
                }
                db.SelfTestScore.DeleteAllOnSubmit(scoreInfo);
                db.SelfTestInfo.DeleteAllOnSubmit(testInfo);
                db.SelfTest.DeleteOnSubmit(test);
                db.SubmitChanges();
            }
            return Json(new { Success = true });
        }
        #endregion

        public static string ToHMACSHA1(string encryptText, string encryptKey)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(encryptKey);
            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(encryptText);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }
    }
}