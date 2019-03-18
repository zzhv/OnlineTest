using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Models
{
    /// <summary>
    /// 统测考试页面的model
    /// </summary>
    public class UnifiedTestViewModel
    {
        public IEnumerable<UnifiedTestQAViewModel> Answer { get; set; }//校验完后的答案
        public IEnumerable<UniteTest> TimeInfo { get; set; }//统测时间信息
    }
}