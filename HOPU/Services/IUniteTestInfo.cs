using HOPU.Models;
using System.Collections.Generic;

namespace HOPU.Services
{
    public interface IUniteTestInfo
    {
        /// <summary>
        /// 根据UtID获得依据UserName来打乱Topic后的统测题目
        /// </summary>
        /// <param name="UtId">统测ID</param>
        /// <param name="UserName">用户名</param>
        /// <returns></returns>
        IEnumerable<UnifiedTestNewTopicIdViewModel> GetUnifiedTestTopics(int? UtId, string UserName);

        /// <summary>
        /// 添加统测
        /// </summary>
        /// <param name="submitCheckbox">所选分类</param>
        /// <param name="topicCount">题目数量</param>
        /// <param name="timeLenth">考试时间</param>
        /// <returns></returns>
        string AddTest(string[] submitCheckbox, int topicCount, int timeLenth);

        /// <summary>
        /// 通过UtId删除统测
        /// </summary>
        /// <param name="UtId">统测ID</param>
        /// <returns></returns>
        bool DeleteTest(int UtId);
    }
}
