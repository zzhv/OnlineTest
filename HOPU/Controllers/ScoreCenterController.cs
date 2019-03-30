using HOPU.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HOPU.Controllers
{
    [Authorize]
    public class ScoreCenterController : Controller
    {

        #region  成绩列表 Score 

        public ActionResult Score(int? Id)
        {
            if (IsAdmin())
            {
                var pageNumber = Id ?? 1;
                var utList = UnifiedTestListViewModel.GetUtId();
                ViewBag.UtId = pageNumber;
                return View(utList);
            }
            return View();
        }

        #endregion

        #region AdminGetScore

        [HttpPost]
        public JsonResult AdminGetScore(int Id, int limit, int offset, string keyword, string sortOrder, string sortName)
        {

            HopuDBDataContext db = new HopuDBDataContext();
            string sql = AdminGetSql(Id, keyword, sortName, sortOrder);
            //数据请求
            var scoreList = db.ExecuteQuery<UniteTestScore>(sql).ToList();
            List<UniteTestScore> score = new List<UniteTestScore>();
            //检测到循环引用报错，所以只能暂时这样转换一下，看以后有没有更好的办法
            //test
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
            return Json(new
            {
                total = totalq,
                rows = rowsq
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据条件生成sql语句
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="sortName">排序的列名</param>
        /// <param name="sortOrder">排序的方式</param>
        /// <returns>返回要查询的sql语句</returns>
        public static string AdminGetSql(int Id, string keyword, string sortName, string sortOrder)
        {
            string sql = @"SELECT * FROM UniteTestScore";

            if (keyword == "")
            {
                sql += @" Where UtId=" + Id + "";
            }
            else
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
                    + "LIKE '%" + keywordList[i] + "%'" + " AND UtId = " + Id + "";
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

        #region GetScore

        [HttpGet]
        public JsonResult GetScore(int limit, int offset, string keyword, string sortOrder, string sortName)
        {

            HopuDBDataContext db = new HopuDBDataContext();
            string UserName = User.Identity.GetUserName();
            string sql = GetSql(UserName, keyword, sortName, sortOrder);
            //数据请求
            var scoreList = db.ExecuteQuery<UniteTestScore>(sql).ToList();
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
            return Json(new
            {
                total = totalq,
                rows = rowsq
            }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 根据条件生成sql语句
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="sortName">排序的列名</param>
        /// <param name="sortOrder">排序的方式</param>
        /// <returns>返回要查询的sql语句</returns>
        public static string GetSql(string UserName, string keyword, string sortName, string sortOrder)
        {
            string sql = @"SELECT * FROM UniteTestScore";

            if (keyword == "")
            {
                sql += @" Where UserName=" + UserName + "";
            }
            else
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
                    + "LIKE '%" + keywordList[i] + "%'" + " AND UserName = " + UserName + "";
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

        public ActionResult Score_S()
        {

            return View();
        }
    }
}