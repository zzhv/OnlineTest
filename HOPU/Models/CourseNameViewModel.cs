using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Models
{
    /// <summary>
    /// 题目分类明细
    /// </summary>
    public class CourseNameViewModel
    {
        public double? TId { get; set; }
        public string TypeName { get; set; }
        public double? CourseId { get; set; }
        public string CourseName { get; set; }
    }
}