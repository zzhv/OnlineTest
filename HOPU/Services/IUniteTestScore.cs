using System.Collections.Generic;
using HOPU.Models;
namespace HOPU.Services
{
    public interface IUniteTestScore
    {
        /// <summary>
        /// 获取指定用户的指定统测号的统测成绩信息
        /// </summary>
        /// <param name="UtId">统测ID</param>
        /// <param name="UserName">用户名</param>
        /// <returns></returns>
        IEnumerable<UniteTestScore> GetUniteTestScore(int? UtId, string UserName);

        /// <summary>
        /// 将统测考试结果存入数据库
        /// </summary>
        /// <param name="uniteTestScoreInfo">统测成绩信息</param>
        /// <returns></returns>
        bool InsertScore(UniteTestScore uniteTestScoreInfo);
    }
}
