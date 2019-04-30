using System.Collections.Generic;

namespace HOPU.Models
{
    public class SelfTestViewModel
    {
        public IEnumerable<SelfTest> TimeInfo { get; set; }//独立测时间信息
        public IEnumerable<SelfTestInfo> TopicInfo { get; set; }//题目信息
    }
}