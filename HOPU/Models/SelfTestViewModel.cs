using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Models
{
    public class SelfTestViewModel
    {
        public IEnumerable<SelfTest> TimeInfo { get; set; }//独立测时间信息
        public IEnumerable<SelfTestInfo> TopicInfo { get; set; }//题目信息
    }
}