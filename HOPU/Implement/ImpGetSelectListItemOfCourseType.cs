using HOPU.InterFace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HOPU.Models;

namespace HOPU.Implement
{
    public class ImpGetSelectListItemOfCourseType : ICourse
    {
        /// <summary>
        /// 获得Course表的CourseName与CourseID组成SelectListItem
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetSelectListItemOfCourseType()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            return db.Course.Select(a => new SelectListItem
            {
                Value = Convert.ToInt32(a.CourseID).ToString(),
                Text = a.CourseName
            });
        }
    }
}