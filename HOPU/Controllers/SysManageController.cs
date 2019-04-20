﻿using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using HOPU.Models;

namespace HOPU.Controllers
{
    [Authorize]
    public class SysManageController : Authorize
    {
        // GET: SysManage
        public ActionResult SysManageIndex()
        {
            return View();
        }

        public ActionResult topicmanage()
        {
            return View();
        }

        #region GetTopic


        [HttpPost]
        public JsonResult GetTopic(int limit, int offset, string keyword, string sortOrder, string sortName)
        {
            string sql = GetSql(limit, offset, keyword, sortOrder, sortName);
            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.ExecuteQuery<Topic>(sql).ToList();
            var totalq = result.Count;
            var rowsq = result.Skip(offset).Take(limit);
            return Json(new
            {
                total = totalq,
                rows = rowsq
            }, JsonRequestBehavior.AllowGet);
        }

        private static string GetSql(int limit, int offset, string keyword, string sortOrder, string sortName)
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


        [HttpPost]
        public JsonResult EditTopic(Topic topic)
        {
            char[] Answer =topic.Answer.ToUpper().ToCharArray();
            Array.Sort(Answer);
            topic.Answer = string.Join("", Answer);
            HopuDBDataContext db = new HopuDBDataContext();
            var Edit = db.Topic.Where(x => x.TopicID == topic.TopicID).Select(a => a);
            foreach (var i in Edit)
            {
                i.TopicID = topic.TopicID;
                i.Title = topic.Title;
                i.AnswerA = topic.AnswerA;
                i.AnswerB = topic.AnswerB;
                i.AnswerC = topic.AnswerC;
                i.AnswerD = topic.AnswerD;
                i.Answer = topic.Answer;
                i.CourseID = topic.CourseID;
            }
            db.SubmitChanges();
            if (topic.TopicID > 0)
            {
                return Json("success");
            }
            else
            {
                return Json(false);
            }
        }
    }
}