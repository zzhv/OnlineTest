using HOPU.Models;
using Microsoft.AspNet.Identity;
using MvcSiteMapProvider.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;

namespace HOPU.Controllers
{
    public class ScoreCenterController : Controller
    {
        // GET: ScoreCenter
        public ActionResult Index()
        {
            return View();
        }
        #region  成绩列表 Score 

        public ActionResult Score(int? Id)
        {
            var pageNumber = Id ?? 1;
            ViewBag.UtIdList = UnifiedTestModel.GetUtId();
            ViewBag.UtId = pageNumber;
            return View();
        }

        [HttpGet]
        public JsonResult AdminGetScore(int Id, int limit, int offset, string keyword, string sortOrder, string sortName)
        {

            HopuDBDataContext db = new HopuDBDataContext();
            string sql = GetSql(Id, keyword, sortName, sortOrder);
            //数据请求
            var scoreList = db.ExecuteQuery<UniteTestScore>(sql).ToList();
            //var result = db.UniteTestScore
            //    .Where(a => a.UtId == Id).Join(db.AspNetUsers, a => a.UserName, b => b.UserName, (a, b) => new { a, b })
            //    .ToList()
            //    .Select(a => new UniteTestScore
            //    {
            //        Id = a.a.Id,
            //        UtId = a.a.UtId,
            //        RealUserName = a.b.RealUserName,
            //        UserName = a.b.UserName,
            //        EndTime = a.a.EndTime,
            //        Score = a.a.Score
            //    });
            //List<UniteTestScore> score = result.ToList();
            //if (string.IsNullOrEmpty(keyword))
            //{
            //    score = result.Where(a => a.RealUserName.Contains(keyword)).ToList();

            //}
            List<UniteTestScore> score = new List<UniteTestScore>();
            foreach (var i in scoreList)
            {
                UniteTestScore a = new UniteTestScore()
                {
                    Id = i.Id,
                    UtId = i.UtId,
                    RealUserName = i.RealUserName,
                    UserName = i.UserName,
                    EndTime = i.EndTime,
                    Score = i.Score
                };
                score.Add(a);
            }
            var totalq = score.Count;
            var rowsq = score.Skip(offset).Take(limit);
            //var result = JsonConvert.SerializeObject(rowsq.ToArray());
            //string s = "{\"rows\":" + result + ",\"total\":" + score.Count + "}";
            //JObject jo = (JObject)JsonConvert.DeserializeObject(s);
            //return Json(jo);
            return Json(new
            {
                total = totalq,
                rows = rowsq
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 根据条件生成sql语句
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="sortName">排序的列名</param>
        /// <param name="sortOrder">排序的方式</param>
        /// <returns>返回要查询的sql语句</returns>
        public static string GetSql(int Id, string keyword, string sortName, string sortOrder)
        {
            string sql = @"SELECT * FROM UniteTestScore";

            //UtId
            sql += @" Where UtId=" + Id + "";
            //搜索
            if (keyword != "")
            {
                //先判断sortName是否多值模糊搜索
                //这里目地是实现，用户想多条件搜索时，在搜索栏中用逗号隔开
                //比如用户搜索同时有A和B的信息，那么在搜索栏中输入“A,B”或者“A，B”
                var keywordList = keyword.Split(new char[2] { ',', '，' });

                //搜索条件
                for (int i = 0; i < keywordList.Count(); i++)
                {
                    sql += i == 0 ? " WHERE " : " AND ";
                    sql += @"CONCAT(Id,UtId,RealUserName,UserName,EndTime,Score) "
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