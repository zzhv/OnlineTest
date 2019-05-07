using HOPU.Models;
using System.Collections.Generic;

namespace HOPU.Services
{
    public interface IUniteTest
    {
        /// <summary>
        /// 获取统一测试的信息
        /// </summary>
        /// <returns></returns>
        IEnumerable<UniteTest> GetUniteTestInfo();
        /// <summary>
        /// 根据UtId获取统一测试的时间等信息
        /// </summary>
        /// <param name="UtId">统测ID</param>
        /// <returns></returns>
        IEnumerable<UniteTest> GetUniteTestInfo(int? UtId);
    }
}
