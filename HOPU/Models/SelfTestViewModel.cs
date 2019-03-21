using System.Collections.Generic;

namespace HOPU.Models
{
    public class SelfTestViewModel
    {
        public IEnumerable<CourseNameViewModel> CourseNames { get; set; }//题目类名称列表
        public IEnumerable<SelfTest> SelfTest { get; set; }
        public IEnumerable<SelfTestInfo> SelfTestInfo { get; set; }
    }
}