using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace HOPU.Models
{
    /// <summary>
    /// 通用数据访问类
    /// </summary>
    public class SQLHelper
    {
        private static string sqlCoonectionString = ConfigurationManager.ConnectionStrings["SQLConnString"].ConnectionString;
        //, SqlParameter[] values
        //public static SqlDataReader GetTable(string sql)
        //{
        //    SqlConnection sqlconn = new SqlConnection(sqlCoonectionString);
        //    sqlconn.Open();
        //    SqlCommand sqlcmd = new SqlCommand(sql, sqlconn);
        //    //sqlcmd.Parameters.AddRange(values);
        //    return sqlcmd.ExecuteReader();
        //}



        public static DataTable GetTable(string sql)
        {
            SqlConnection sqlconn = new SqlConnection(sqlCoonectionString);
            //实例化一个空的dt对象
            DataTable dt = new DataTable();
            SqlDataAdapter sqlDA = new SqlDataAdapter(sql, sqlconn);
            //向dt里填充数据
            sqlDA.Fill(dt);
            return dt;
        }




        [HttpPost]
        public static SqlDataReader GetLoginReader(string sql, string userAccount, string Password)
        {
            SqlConnection conn = new SqlConnection(sqlCoonectionString);
            SqlCommand sqlcmd = new SqlCommand(sql, conn);
            SqlParameter[] values = new SqlParameter[]
            {
                new SqlParameter("@userPwd",Password),
                new SqlParameter("@userAccount",userAccount),
            };
            sqlcmd.Parameters.AddRange(values);
            try
            {
                conn.Open();
                return sqlcmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //检查用户名是否重复
        [HttpPost]
        public static SqlDataReader GetRegisterReader(string userAccount)
        {
            string sqll = "select UserAccount from userinfo where userAccount=@userAccount";
            SqlConnection conn = new SqlConnection(sqlCoonectionString);
            SqlCommand sqlcmd = new SqlCommand(sqll, conn);
            sqlcmd.Parameters.AddWithValue("@userAccount", DBNullValueorStringIfNotNull(userAccount));
            try
            {
                conn.Open();
                return sqlcmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        //完成注册动作
        //public static SqlDataReader GetRealRegisterReader(string sql, string userAccount, string userPwd, string userName)
        //{

        //    SqlConnection conn = new SqlConnection(sqlCoonectionString);
        //    SqlCommand sqlcmd = new SqlCommand(sql, conn);
        //    SqlParameter[] values = new SqlParameter[]
        //    {
        //        new SqlParameter("@userAccount", DBNullValueorStringIfNotNull(userAccount)),
        //        new SqlParameter("@userPwd", DBNullValueorStringIfNotNull(userPwd)),
        //        new SqlParameter("@userName",DBNullValueorStringIfNotNull(userName)),
        //    };
        //    sqlcmd.Parameters.AddRange(values);
        //    try
        //    {
        //        conn.Open();
        //        return sqlcmd.ExecuteReader(CommandBehavior.CloseConnection);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}



        public static int sqlcmd(string sql, string userAccount, string password, string userName)
        {
            SqlConnection conn = new SqlConnection(sqlCoonectionString);
            SqlCommand sqlcmdd = new SqlCommand(sql, conn);
            SqlParameter[] values = new SqlParameter[]
           {
                new SqlParameter("@userAccount", DBNullValueorStringIfNotNull(userAccount)),
                new SqlParameter("@password", DBNullValueorStringIfNotNull(password)),
                new SqlParameter("@userName",DBNullValueorStringIfNotNull(userName)),
           };
            sqlcmdd.Parameters.AddRange(values);
            try
            {
                conn.Open();
                return sqlcmdd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static object DBNullValueorStringIfNotNull(string value)
        {
            object obj;
            if (value == null)
            {
                obj = DBNull.Value;
            }
            else
            {
                obj = value;
            }
            return obj;
        }
        //试题
        public static SqlDataAdapter GetDataSet(string sql)
        {
            SqlConnection conn = new SqlConnection(sqlCoonectionString);
            try
            {
                conn.Open();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                return sda;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}