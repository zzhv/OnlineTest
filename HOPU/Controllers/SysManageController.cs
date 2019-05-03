using HOPU.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Facebook;
using System.Reflection;

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
                filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"),
                    gid.ToString() + Path.GetExtension(file.FileName));
                file.SaveAs(filePath);
            }
            ViewBag.data = DataSetToIList<Topic>(ExcelToDS(filePath), 0);
            ViewBag.MtClick = "$('#mtBtn').click()";//让模态框弹出来
            return View();
        }

        //将Excel文件转换为dataset
        public DataSet ExcelToDS(string Path)
        {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string tableName = schemaTable.Rows[0][2].ToString().Trim();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from " + tableName;
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet(); myCommand.Fill(ds, "table1");
            return ds;
        }


        /// <summary> 
        /// DataSet装换为泛型集合 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="p_DataSet">DataSet</param> 
        /// <param name="p_TableIndex">待转换数据表索引</param> 
        /// <returns></returns> 
        /// 2008-08-01 22:46 HPDV2806 
        public static IList<T> DataSetToIList<T>(DataSet p_DataSet, int p_TableIndex)
        {
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return null;
            if (p_TableIndex > p_DataSet.Tables.Count - 1)
                return null;
            if (p_TableIndex < 0)
                p_TableIndex = 0;

            DataTable p_Data = p_DataSet.Tables[p_TableIndex];
            // 返回值初始化 
            IList<T> result = new List<T>();
            for (int j = 0; j < p_Data.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < p_Data.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值 
                        if (pi.Name.Equals(p_Data.Columns[i].ColumnName))
                        {
                            // 数据库NULL值单独处理 
                            if (p_Data.Rows[j][i] != DBNull.Value)
                                pi.SetValue(_t, p_Data.Rows[j][i], null);
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }

        /// <summary> 
        /// DataSet装换为泛型集合 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="p_DataSet">DataSet</param> 
        /// <param name="p_TableName">待转换数据表名称</param> 
        /// <returns></returns> 
        /// 2008-08-01 22:47 HPDV2806 
        public static IList<T> DataSetToIList<T>(DataSet p_DataSet, string p_TableName)
        {
            int _TableIndex = 0;
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return null;
            if (string.IsNullOrEmpty(p_TableName))
                return null;
            for (int i = 0; i < p_DataSet.Tables.Count; i++)
            {
                // 获取Table名称在Tables集合中的索引值 
                if (p_DataSet.Tables[i].TableName.Equals(p_TableName))
                {
                    _TableIndex = i;
                    break;
                }
            }
            return DataSetToIList<T>(p_DataSet, _TableIndex);
        }

        public ActionResult CourseTypeManage()
        {

            return View();
        }
    }
}
