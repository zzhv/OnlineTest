using System.Collections.Generic;

namespace HOPU.Services
{
    public interface IBTTable
    {
        /// <summary>
        /// 获取标准的Bootstrap table的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="SqlStr">Sql语句</param>
        /// <returns></returns>
        IEnumerable<T> GetTableInfo<T>(string SqlStr);
    }
}
