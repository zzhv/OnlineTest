using HOPU.Models;
using HOPU.Services;
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
        private readonly ICourse _course;
        private readonly IUniteTest _uniteTest;
        private readonly IUniteTestScore _uniteTestScore;
        private readonly IUniteTestInfo _uniteTestInfo;

        public UifiedTestCenterController(ICourse course, IUniteTest uniteTest, IUniteTestScore uniteTestScore, IUniteTestInfo uniteTestInfo)
        {
            _course = course;
            _uniteTest = uniteTest;
            _uniteTestScore = uniteTestScore;
            _uniteTestInfo = uniteTestInfo;
        }

        public ActionResult UnifiedTestType(int? page)
        {
            var vm = new UnifiedTestTypeViewModel
            {
                CourseNames = _course.GetListOfCourseAndTypeInfo(),
                UniteTests = _uniteTest.GetUniteTestInfo().ToPagedList(page ?? 1, 5)
            };
            return View(vm);
        }

        #region 进入统测统测 UnifiedTest
        public ActionResult UnifiedTest(int? Id)
        {
            int? UtId = Id;
            string UserName = User.Identity.GetUserName();
            ViewBag.Title = UtId;
            if (UtId == null)
            {
                return PartialView("Error");
            }
            var vmt = _uniteTest.GetUniteTestInfo(UtId);
            //以下综合判断能否进入考试 有一项不符合就直接拒绝
            if (!vmt.Any())
            {
                //如果统测号不存在
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
            //如果没有提交过答案
            bool joinUniteTest;
            var UniteTestScoreInfo = _uniteTestScore.GetUniteTestScore(UtId, UserName);
            if (!UniteTestScoreInfo.Any())
            {
                joinUniteTest = true;
            }
            else
            {
                return RedirectToAction("Score", "ScoreCenter", new { Id });
            }
            //以上验证完成，就能能加入统测
            if (joinUniteTest)
            {
                var vm = new UnifiedTestViewModel
                {
                    TopicInfo = _uniteTestInfo.GetUnifiedTestTopics(UtId, UserName).OrderBy(x => x.TopicID),
                    TimeInfo = vmt
                };
                return View(vm);
            }
            return View();
        }
        #endregion

        #region 校验统测答案
        [HttpPost]
        public JsonResult UifiedTest(string[] Answer, int UtId)
        {
            string UserName = User.Identity.GetUserName();
            bool commitAnswer = false;
            //判断时间是否在考试时间段内
            var timeInfo = _uniteTest.GetUniteTestInfo(UtId);
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
                //此处和取题目必须一样
                List<UnifiedTestNewTopicIdViewModel> answerList = _uniteTestInfo.GetUnifiedTestTopics(UtId, UserName).OrderBy(s => s.TopicID).ToList();
                //开始校验答案
                List<UnifiedTestQAViewModel> results = new List<UnifiedTestQAViewModel>();//成绩信息
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
                        results.Add(resultinfo);
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
                        results.Add(resultinfo);
                    }
                }
                //如果是第一次提交答案
                var UniteTestScoreInfo = _uniteTestScore.GetUniteTestScore(UtId, UserName);
                if (!UniteTestScoreInfo.Any())
                {
                    //将考试结果存入数据库
                    UniteTestScore uniteTestScoreInfo = new UniteTestScore
                    {
                        UtId = UtId,
                        RealUserName = GetRealUserName.GetRealName(User.Identity.GetUserId()),
                        UserName = User.Identity.GetUserName(),
                        EndTime = DateTime.Now,
                        Score = Convert.ToInt32(Math.Round(sumScore, 0, MidpointRounding.AwayFromZero))
                    };
                    //如果存入成功
                    if (_uniteTestScore.InsertScore(uniteTestScoreInfo))
                    {
                        return Json(results);
                    }
                }
                else
                {
                    return Json(false);
                }
            }
            return Json(false);
        }
        #endregion

        //添加统测
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddTest(string[] submitCheckbox, int topicCount, int timeLenth)
        {
            if (submitCheckbox.Count() == 0 || topicCount <= 0 || timeLenth <= 0)
            {
                return Json("不得留空！");
            }
            string Result = _uniteTestInfo.AddTest(submitCheckbox, topicCount, timeLenth);
            if (Result.Equals("true"))
            {
                return Json(new { Success = true });
            }
            else
            {
                return Json(Result);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public JsonResult DeleteTest(int UtId)
        {
            if (_uniteTestInfo.DeleteTest(UtId))
                return Json(new { Success = true });
            return Json(new { Success = false });
        }
    }
}