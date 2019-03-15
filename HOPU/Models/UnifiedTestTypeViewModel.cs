using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Models
{
    /// <summary>
    /// 统测列表的modeloutput model
    /// </summary>
    public class UnifiedTestTypeViewModel
    {
        public IEnumerable<CourseNameViewModel> CourseNames { get; set; }//题目类名称列表
        public IEnumerable<UniteTest> UniteTests { get; set; }//统测列表
    }
}