using System.Collections.Generic;

namespace HOPU.Models
{
    /// <summary>
    /// 题目分类明细
    /// </summary>
    public class CourseNameViewModel
    {
        public double? TID { get; set; }
        public string TypeName { get; set; }
        public double? CourseID { get; set; }
        public string CourseName { get; set; }
    }

    public class CourseNameListViewModel
    {
        public IEnumerable<CourseNameViewModel> CourseName { get; set; }
        public IEnumerable<TypeInfo> TypeName { get; set; }
    }
}