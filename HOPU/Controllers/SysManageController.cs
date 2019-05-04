using HOPU.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            HopuDBDataContext db = new HopuDBDataContext();
            var courseType = db.Course.Select(a => new SelectListItem
            {
                Text = a.CourseName,
                Value = a.CourseID.ToString()
            });
            return View(courseType);
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

        //动态生成Sql
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

        //单项编辑
        [HttpPost]
        public JsonResult EditTopic(Topic topic)
        {
            char[] Answer = topic.Answer.ToUpper().ToCharArray();
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

        //单项删除
        [HttpPost]
        public JsonResult DeleteTopic(Topic topic)
        {
            HopuDBDataContext db = new HopuDBDataContext();
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

        //多选删除
        [HttpPost]
        public JsonResult DeleteAllTopic(double[] topics)
        {
            HopuDBDataContext db = new HopuDBDataContext();
            for (int i = 0; i < topics.Length; i++)
            {
                db.Topic.DeleteOnSubmit(db.Topic.Where(x => x.TopicID == topics[i]).SingleOrDefault());
                db.SubmitChanges();
            }

            return Json(true);
        }

        //从文件导入题目
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
            HopuDBDataContext db = new HopuDBDataContext();
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
            return View();
        }

        //将xls文件转换为dataset
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

        public ActionResult CourseTypeManage()
        {
            return View();
        }
    }
}
