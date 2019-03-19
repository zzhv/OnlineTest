using System.Collections.Generic;

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