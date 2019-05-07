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
            HopuDBDataContext db = new HopuDBDataContext();
            string userName = User.Identity.GetUserName();
            var products = db.SelfTest.Where(x => x.UserName == userName).Select(a => a).OrderByDescending(a => a.StId).ToList();
            var pageNumber = page ?? 1;
            var onePage = products.ToPagedList(pageNumber, 5);
            var list = GetCourseInfo().ToList();
            var vms = list.Select(x => new CourseNameViewModel
            {
                CourseID = x.CourseID,
                CourseName = x.CourseName,
                TypeName = x.TypeName,
                TID = x.TID
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
        protected static IEnumerable<SelfTest> GetTestInfo(string userName)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.SelfTest.Where(x => x.UserName == userName).Select(a => a).OrderByDescending(a => a.StId);
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
                             TID = p.TID,
                             TypeName = p.TypeName,
                             CourseID = c.CourseID,
                             CourseName = c.CourseName
                         };
            return result;
        }
        #endregion

        #region 独测
        public ActionResult SelfTest(long? Id)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            long? stId = Id;
            ViewBag.Title = stId;
            string userName = User.Identity.GetUserName();
            if (stId == null)
            {
                return PartialView("Error");
            }
            var vmt = db.SelfTest.Where(a => a.StId == stId && a.UserName == userName).ToList().Select(x => new SelfTest
            {
                StId = x.StId,
                UserName = x.UserName,
                StartTime = x.StartTime,
                TimeLenth = x.TimeLenth,
                TopicCount = x.TopicCount,
            });
            if (!stId.ToString().Substring(0, 10).Equals(userName))
            {
                return Json(stId + "不存在！", JsonRequestBehavior.AllowGet);
            }
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
                return RedirectToAction("Score_S", "ScoreCenter", new { Id });
            }
            //如果是第一次提交答案
            var selfTestScoreInfo = db.SelfTestScore.FirstOrDefault(a => a.StId == stId && a.UserName == User.Identity.GetUserName());
            if (selfTestScoreInfo != null)
            {
                return RedirectToAction("Score_S", "ScoreCenter", new { Id });
            }
            //以上验证完成，如果能加入统测
            {
                //据StId获取题目列表
                var topicInfo = db.SelfTestInfo.Where(a => a.StId == stId && a.UserName == userName).ToList().Select(c =>
                      new SelfTestInfo()
                      {
                          StId = c.StId,
                          UserName = c.UserName,
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
                    TopicInfo = topicInfo.OrderBy(x => x.TopicID),
                    TimeInfo = vmt
                };
                return View(vm);
            }
        }
        #endregion

        #region 添加独测 AddTest
        //添加统测
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTest(string[] submitCheckbox, int topicCount, int timeLenth)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            string userName = User.Identity.GetUserName();
            long stIdUserNameHead = Convert.ToInt32(userName) * 10000L;
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
            var maxStId = db.SelfTest.Where(x => x.UserName == userName).Select(a => a.StId);
            var realMaxStId = !maxStId.Any() ? stIdUserNameHead + 1 : maxStId.Max() + 1L;
            //向SelfTestInfo表添加题目信息
            List<SelfTestInfo> SelfTestInfos = new List<SelfTestInfo>();
            foreach (var s in topicList)
            {
                SelfTestInfo u = new SelfTestInfo
                {
                    StId = realMaxStId,
                    UserName = userName,
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
                UserName = userName,
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
        public JsonResult DeleteTest(long stId)
        {
            using (var db = new HopuDBDataContext())
            {
                if (!stId.ToString().Substring(0, 10).Equals(User.Identity.GetUserName()))
                {
                    return Json(stId + "不存在！");
                }
                //把要删除的统测题目查出来
                var testInfo = db.SelfTestInfo.Where(a => a.StId == stId).Select(a => a);
                //把要删除的统测查出来
                var test = db.SelfTest.SingleOrDefault(a => a.StId == stId);
                //把要删除的成绩信息查出来
                var scoreInfo = db.SelfTestScore.Where(a => a.StId == stId);
                if (test == null)
                {
                    return Json(stId + "不存在！");
                }
                db.SelfTestScore.DeleteAllOnSubmit(scoreInfo);
                db.SelfTestInfo.DeleteAllOnSubmit(testInfo);
                db.SelfTest.DeleteOnSubmit(test);
                db.SubmitChanges();
            }
            return Json(new { Success = true });
        }
        #endregion

        #region 校验统测答案
        //校验答案
        [HttpPost]
        public JsonResult SelfTest(string[] Answer, long stId)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            //判断时间是否在考试时间段内
            bool commitAnswer = false;
            string userName = User.Identity.GetUserName();
            if (!stId.ToString().Substring(0, 10).Equals(userName))
            {
                return Json(stId + "不存在！");
            }
            var timeInfo = db.SelfTest.Where(a => a.StId == stId).Select(a => a);
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
                //据stId获取答案
                var topicListResult = db.SelfTestInfo.Where(a => a.StId == stId).ToList().Select(c =>
                new SelfTestNewTopicIdViewModel
                {
                    TopicID = ToHmacsha1(c.TopicID.ToString(), userName),
                    Answer = c.Answer,
                });
                //答案集合
                List<SelfTestNewTopicIdViewModel> AnswerList = topicListResult.OrderBy(s => s.TopicID).ToList();
                //开始校验答案
                List<SelfTestQAViewModel> result = new List<SelfTestQAViewModel>();
                //先算出每题多少分 
                double itemScore = 100F / AnswerList.Count;
                double sumScore = 0;
                //校验答案
                for (int i = 0; i < AnswerList.Count; i++)
                {
                    if (AnswerList[i].Answer.Equals(Answer[i]))
                    {
                        SelfTestQAViewModel resultinfo = new SelfTestQAViewModel
                        {
                            UserAnswer = Answer[i],
                            RealAnswer = AnswerList[i].Answer,
                            IsTrue = true
                        };
                        result.Add(resultinfo);
                        sumScore += itemScore;
                    }
                    else
                    {
                        SelfTestQAViewModel resultinfo = new SelfTestQAViewModel
                        {
                            UserAnswer = Answer[i],
                            RealAnswer = AnswerList[i].Answer,
                            IsTrue = false
                        };
                        result.Add(resultinfo);
                    }
                }
                var selfTestScoreInfo = db.SelfTestScore.FirstOrDefault(a => a.StId == stId && a.UserName == User.Identity.GetUserName());
                //如果是第一次提交答案
                if (selfTestScoreInfo == null)
                {
                    //将考试结果存入数据库
                    var selfTestScore = new SelfTestScore
                    {
                        StId = stId,
                        RealUserName = GetRealUserName.GetRealName(User.Identity.GetUserId()),
                        UserName = userName,
                        EndTime = DateTime.Now,
                        Score = Convert.ToInt32(Math.Round(sumScore, 0, MidpointRounding.AwayFromZero))
                    };
                    db.SelfTestScore.InsertOnSubmit(selfTestScore);
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
        public static string ToHmacsha1(string encryptText, string encryptKey)
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