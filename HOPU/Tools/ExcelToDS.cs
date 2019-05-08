using System.Data;
using System.Data.OleDb;

namespace HOPU.Tools
{
    public class ExcelToDS
    {

        /// <summary>
        /// 将xls文件转换为dataset
        /// </summary>
        /// <param name="Path">xls文件的绝对路径</param>
        /// <returns>dataset</returns>
        public static DataSet excelToDS(string Path)
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
    }
}