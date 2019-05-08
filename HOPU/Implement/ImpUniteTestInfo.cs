using HOPU.Models;
using HOPU.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HOPU.Implement
{
    public class ImpUniteTestInfo : IUniteTestInfo
    {
        private HopuDBDataContext db = new HopuDBDataContext();


        #region 添加统测
        public string AddTest(string[] submitCheckbox, int topicCount, int timeLenth)
        {
            string flag = "false";
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
                return ("所填数量不得大于实际数量" + topicList.Count);
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
            //向UniteTest表添加统测信息
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
                if (newTest.UtId > 0)
                {

                    flag = "true";
                }
            }
            catch (Exception)
            {
                flag = "false";
            }
            return flag;
        }
        #endregion

        public bool DeleteTest(int UtId)
        {
            bool flag = true;
            try
            {
                //把要删除的统测题目查出来
                var testInfo = db.UniteTestInfo.Where(a => a.UtId == UtId).Select(a => a);
                //把要删除的统测查出来
                var test = db.UniteTest.SingleOrDefault(a => a.UtId == UtId);
                //把要删除的成绩信息查出来
                var scoreInfo = db.UniteTestScore.Where(a => a.UtId == UtId);
                db.UniteTestScore.DeleteAllOnSubmit(scoreInfo);
                db.UniteTestInfo.DeleteAllOnSubmit(testInfo);
                db.UniteTest.DeleteOnSubmit(test);
                db.SubmitChanges();
            }
            catch (Exception)
            {
                flag = true;
            }
            return flag;
        }

        public IEnumerable<UnifiedTestNewTopicIdViewModel> GetUnifiedTestTopics(int? UtId, string UserName)
        {
            return db.UniteTestInfo.Where(a => a.UtId == UtId).ToList().Select(c =>
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
                      TopicID = Tools.ToHMACSHA1.toHMACSHA1(c.TopicID.ToString(), UserName)
                  });
        }
    }
}