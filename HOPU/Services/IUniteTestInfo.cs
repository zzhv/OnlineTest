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
    }
}
