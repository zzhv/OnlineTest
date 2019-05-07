using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HOPU.Models;
using HOPU.Services;

namespace HOPU.Implement
{
    public class ImpCourse : ICourse
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        /// <summary>
        /// 获得Course表的Name与ID组成SelectListItem
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetSelectListItemOfCourseType()
        {
            return db.Course.Select(a => new SelectListItem
            {
                Value = Convert.ToInt32(a.CourseID).ToString(),
                Text = a.CourseName
            });
        }

        /// <summary>
        /// 获得Course、TypeInfo表组成SelectListItem
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CourseNameViewModel> GetListOfCourseAndTypeInfo()
        {
            return from p in db.TypeInfo
                   join c in db.Course on p.TID equals c.TID
                   orderby p.TID
                   select new CourseNameViewModel
                   {
                       TID = p.TID,
                       TypeName = p.TypeName,
                       CourseID = c.CourseID,
                       CourseName = c.CourseName
                   };

        }

    }
}