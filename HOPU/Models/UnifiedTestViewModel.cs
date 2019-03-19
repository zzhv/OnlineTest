using System.Collections.Generic;

namespace HOPU.Models
{
    /// <summary>
    /// 统测考试页面的model
    /// </summary>
    public class UnifiedTestViewModel
    {
        public IEnumerable<UniteTest> TimeInfo { get; set; }//统测时间信息
        public IEnumerable<UniteTestInfo> TopicInfo { get; set; }//题目信息
    }
}