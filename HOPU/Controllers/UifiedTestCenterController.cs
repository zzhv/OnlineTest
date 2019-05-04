using HOPU.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using X.PagedList;

namespace HOPU.Controllers
{
    [Authorize]
    public class UifiedTestCenterController : Controller
    {
        #region 统测列表 UnifiedTestType
        public ActionResult UnifiedTestType(int? page)
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
            var vmu = onePage.Select(x => new UniteTest
            {
                UtId = x.UtId,
                StartTime = x.StartTime,
                TimeLenth = x.TimeLenth,
                TopicCount = x.TopicCount,
                CourseName = x.CourseName,
                CourseId = x.CourseName
            });
            var vm = new UnifiedTestTypeViewModel
            {
                CourseNames = vms,
                UniteTests = vmu
            };
            return View(vm);
        }

        protected static IEnumerable<UniteTest> GetTestInfo()
        {

            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.UniteTest.Select(a => a).OrderByDescending(a => a.UtId);
            return result;
        }

        #endregion

        #region 统测 UnifiedTest
        public ActionResult UnifiedTest(int? Id)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            int? UtId = Id;
            ViewBag.Title = UtId;
            bool joinUniteTest;
            if (UtId == null)
            {
                return PartialView("Error");
            }
            var vmt = db.UniteTest.Where(a => a.UtId == UtId).ToList().Select(x => new UniteTest
            {
                UtId = x.UtId,
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
            var UniteTestScoreInfo = db.UniteTestScore.Where(a => a.UtId == UtId && a.UserName == User.Identity.GetUserName()).FirstOrDefault();
            if (UniteTestScoreInfo == null)
            {
                joinUniteTest = true;
            }
            else
            {
                return RedirectToAction("Score", "ScoreCenter", new { Id });
            }
            //以上验证完成，如果能加入统测
            if (joinUniteTest)
            {
                //据UtId获取题目列表
                string UserName = User.Identity.GetUserName();
                var TopicInfo = db.UniteTestInfo.Where(a => a.UtId == UtId).ToList().Select(c =>
                    new UnifiedTestNewTopicIdViewModel
                    {
                        UtId = c.UtId,
                        Title = c.Title,
                        AnswerA = c.AnswerA,
                        AnswerB = c.AnswerB,
                        AnswerC = c.AnswerC,
                        AnswerD = c.AnswerD,
                        Answer = c.Answer,
                        CourseID = c.CourseID,
                        TopicID = ToHMACSHA1(c.TopicID.ToString(), UserName)
                    });
                var vm = new UnifiedTestViewModel
                {
                    TopicInfo = TopicInfo.OrderBy(x => x.TopicID),
                    TimeInfo = vmt
                };
                return View(vm);
            }
            return View();
        }
        #endregion

        #region 校验统测答案
        //校验答案
        [HttpPost]
        public JsonResult UifiedTest(string[] Answer, int UtId)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            //判断时间是否在考试时间段内
            bool commitAnswer = false;
            var timeInfo = db.UniteTest.Where(a => a.UtId == UtId).Select(a => a);
            foreach (var item in timeInfo)
            {
                //如果结束时间大于当前时间，可以提交
                if (Convert.ToDateTime(item.StartTime).AddMinutes(item.TimeLenth) > DateTime.Now.AddSeconds(-5))//给五秒的冗余时间 否则js倒计时0时提交会失败
                {
                    commitAnswer = true;
                    break;
                }
                else
                {
                    return Json(false);
                }

            }
            if (commitAnswer)
            {
                //据UtId获取答案
                string UserName = User.Identity.GetUserName();
                var topicListResult = db.UniteTestInfo.Where(a => a.UtId == UtId).ToList().Select(c =>
                new UnifiedTestNewTopicIdViewModel
                {
                    TopicID = ToHMACSHA1(c.TopicID.ToString(), UserName),
                    Answer = c.Answer,
                });
                //答案集合
                List<UnifiedTestNewTopicIdViewModel> answerList = topicListResult.OrderBy(s => s.TopicID).ToList();
                //开始校验答案
                List<UnifiedTestQAViewModel> result = new List<UnifiedTestQAViewModel>();
                //先算出每题多少分 
                double itemScore = 100F / answerList.Count;
                double sumScore = 0;
                //校验答案
                for (int i = 0; i < answerList.Count; i++)
                {
                    if (answerList[i].Answer.Equals(Answer[i]))
                    {
                        UnifiedTestQAViewModel resultinfo = new UnifiedTestQAViewModel
                        {
                            UserAnswer = Answer[i],
                            RealAnswer = answerList[i].Answer,
                            IsTrue = true
                        };
                        result.Add(resultinfo);
                        sumScore += itemScore;
                    }
                    else
                    {
                        UnifiedTestQAViewModel resultinfo = new UnifiedTestQAViewModel
                        {
                            UserAnswer = Answer[i],
                            RealAnswer = answerList[i].Answer,
                            IsTrue = false
                        };
                        result.Add(resultinfo);
                    }
                }
                var UniteTestScoreInfo = db.UniteTestScore.Where(a => a.UtId == UtId && a.UserName == User.Identity.GetUserName()).FirstOrDefault();
                //如果是第一次提交答案
                if (UniteTestScoreInfo == null)
                {
                    //将考试结果存入数据库
                    var uniteTestScore = new UniteTestScore
                    {
                        UtId = UtId,
                        RealUserName = GetRealUserName.GetRealName(User.Identity.GetUserId()),
                        UserName = User.Identity.GetUserName(),
                        EndTime = DateTime.Now,
                        Score = Convert.ToInt32(Math.Round(sumScore, 0, MidpointRounding.AwayFromZero))
                    };
                    db.UniteTestScore.InsertOnSubmit(uniteTestScore);
                    db.SubmitChanges();
                }
                else
                {
                    return Json(false);
                }
                return Json(result);
            }
            return Json(false);
        }
        #endregion

        #region 添加统测 AddTest
        //添加统测
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTest(string[] submitCheckbox, int topicCount, int timeLenth)
        {
            HopuDBDataContext db = new HopuDBDataContext();

            //if (!IsAdmin())//如果不是admin权限
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
            string selectTopicSql = "select top " + topicCount + " * from Topic where " + addSelectTopic + " order by NEWID()";
            var topicList = db.ExecuteQuery<Topic>(selectTopicSql).ToList();
            //DataTable dt = SQLHelper.GetTable(selectTopicSql);
            if (topicList.Count < topicCount)//如果所取题目总数小于要求的数量
            {
                return Json("所填数量不得大于实际数量" + topicList.Count);
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
            List<UniteTestInfo> uniteTestInfos = new List<UniteTestInfo>();
            foreach (var s in topicList)
            {
                UniteTestInfo u = new UniteTestInfo
                {
                    UtId = realMaxUtid,
                    Title = s.Title,
                    TopicID = Convert.ToInt32(s.TopicID),
                    AnswerA = s.AnswerA,
                    AnswerB = s.AnswerB,
                    AnswerC = s.AnswerC,
                    AnswerD = s.AnswerD,
                    Answer = s.Answer,
                    CourseID = s.CourseID.ToString()
                };
                uniteTestInfos.Add(u);
            }
            //想UniteTest表添加统测信息
            var newTest = new UniteTest
            {
                UtId = realMaxUtid,
                StartTime = DateTime.Now,
                TimeLenth = timeLenth,
                TopicCount = topicCount,
                CourseId = strCourseId,
                CourseName = courseName
            };
            try
            {
                db.UniteTest.InsertOnSubmit(newTest);
                db.UniteTestInfo.InsertAllOnSubmit(uniteTestInfos);
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return Json(new { Success = true });
        }
        #endregion

        #region 删除统测
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteTest(int UtId)
        {
            using (var db = new HopuDBDataContext())
            {

                if (!IsAdmin())//如果不是admin权限
                {
                    return Json(null);
                }
                //把要删除的统测题目查出来
                var testInfo = db.UniteTestInfo.Where(a => a.UtId == UtId).Select(a => a);
                //把要删除的统测查出来
                var test = db.UniteTest.SingleOrDefault(a => a.UtId == UtId);
                //把要删除的成绩信息查出来
                var scoreInfo = db.UniteTestScore.Where(a => a.UtId == UtId);
                if (test == null || testInfo == null)
                {
                    return Json(UtId + "不存在！");
                }
                else
                {
                    db.UniteTestScore.DeleteAllOnSubmit(scoreInfo);
                    db.UniteTestInfo.DeleteAllOnSubmit(testInfo);
                    db.UniteTest.DeleteOnSubmit(test);
                    db.SubmitChanges();
                }
            }
            return Json(new { Success = true });
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