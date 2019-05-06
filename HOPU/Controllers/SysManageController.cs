using HOPU.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HOPU.Implement;
using Newtonsoft.Json;

namespace HOPU.Controllers
{
    [Authorize]
    public class SysManageController : Authorize
    {
        private ImpGetSelectListItemOfCourseType impGetSelectListItemOfCourseType = new ImpGetSelectListItemOfCourseType();
        private HopuDBDataContext db = new HopuDBDataContext();

        public ActionResult SysManageIndex()
        {
            return View();
        }

        public ActionResult TopicManage()
        {
            IEnumerable<SelectListItem> CourseTypeList = impGetSelectListItemOfCourseType.GetSelectListItemOfCourseType();
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
            string sql = GetSqlStr(limit, offset, keyword, sortOrder, sortName);
            var result = db.ExecuteQuery<Topic>(sql).ToList();
            var totalq = result.Count;
            var rowsq = result.Skip(offset).Take(limit);
            return Json(new
            {
                total = totalq,
                rows = rowsq
            }, JsonRequestBehavior.AllowGet);
        }

        //给GetTopic()态生成Sql语句
        private static string GetSqlStr(int limit, int offset, string keyword, string sortOrder, string sortName)
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

        /// <summary>
        /// 单项删除
        /// </summary>
        /// <param name="topic">topic对象</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteTopic(Topic topic)
        {
            var delete = db.Topic.Where(z => z.TopicID == topic.TopicID).SingleOrDefault();
            db.Topic.DeleteOnSubmit(delete);
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

        /// <summary>
        /// 多选删除
        /// </summary>
        /// <param name="topics">题目题号的数组</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteAllTopic(double[] topics)
        {
            for (int i = 0; i < topics.Length; i++)
            {
                db.Topic.DeleteOnSubmit(db.Topic.Where(x => x.TopicID == topics[i]).SingleOrDefault());
                db.SubmitChanges();
            }
            return Json(true);
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
            var topics = Tools.DataSetToList.DataSetToIList<Topic>(ExcelToDS(filePath), "Topics");
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
            return View(impGetSelectListItemOfCourseType.GetSelectListItemOfCourseType());
        }

        /// <summary>
        /// 将xls文件转换为dataset
        /// </summary>
        /// <param name="Path">xls文件的绝对路径</param>
        /// <returns>dataset</returns>
        public DataSet ExcelToDS(string Path)
        {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string tableName = schemaTable.Rows[0][2].ToString().Trim();
            string strExcel = string.Empty;
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from " + tableName;
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "Topics");
            return ds;
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
            string AnswerStr = string.Join("", Answer);
            topic.Answer = AnswerStr;
            try
            {
                db.Topic.InsertOnSubmit(topic);
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                return Json(new { Success = false, msg = e.Message });
            }
            return Json(new { Success = true, topic });
        }

        /// <summary>
        /// 为添加单题获取最大TopicID+1
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMaxTopicID()
        {
            ImpGetMaxTopicID impGetMaxTopicId = new ImpGetMaxTopicID();
            return Json(impGetMaxTopicId.GetMaxTopicID(1));
        }


        public ActionResult CourseTypeManage()
        {
            return View();
        }
    }
}
