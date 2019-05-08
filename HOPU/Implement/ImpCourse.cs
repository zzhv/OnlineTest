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

        public bool EditCourseNameAndTypeName(CourseNameViewModel course)
        {
            bool flag = true;
            try
            {
                var DbCourse = db.Course.Where(x => x.CourseID == course.CourseID).Select(a => a);
                foreach (var i in DbCourse)
                {
                    i.CourseName = course.CourseName;
                };
                var DbType = db.TypeInfo.Where(x => x.TID == course.TID).Select(a => a);
                foreach (var i in DbType)
                {
                    i.TypeName = course.TypeName;
                }
                db.SubmitChanges();
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        public bool DeleteCourse(CourseNameViewModel course)
        {
            bool flag = true;
            try
            {
                var Delete = db.Course.Where(x => x.CourseID == course.CourseID).FirstOrDefault();
                db.Course.DeleteOnSubmit(Delete);
                db.SubmitChanges();
            }
            catch (Exception)
            {
                flag = false;
                throw;
            }
            return flag;
        }


        public int MaxCourseID()
        {

            int realMaxCourseID = 0;
            var maxCourseID = db.Course.Select(a => a.CourseID).Max();
            if (maxCourseID == 0)
            {
                realMaxCourseID = 101;
            }
            else
            {
                realMaxCourseID = Convert.ToInt32(maxCourseID);
            }
            return realMaxCourseID;
        }

        public int MaxTID()
        {
            int realMaxTID = 0;
            var maxTID = db.TypeInfo.Select(a => a.TID).Max();
            if (maxTID == 0)
            {
                realMaxTID = 101;
            }
            else
            {
                realMaxTID = Convert.ToInt32(maxTID);
            }
            return realMaxTID;
        }

        public bool AddTypeAndCourse(CourseNameViewModel course)
        {
            bool flag = true;
            try
            {
                db.TypeInfo.InsertOnSubmit(new TypeInfo { TID = Convert.ToDouble(course.TID), TypeName = course.TypeName });
                db.Course.InsertOnSubmit(new Course { CourseID = Convert.ToDouble(course.CourseID), CourseName = course.CourseName, TID = course.TID });
                db.SubmitChanges();
            }
            catch (Exception)
            {
                flag = false;
                throw;
            }
            return flag;
        }

        public IEnumerable<SelectListItem> GetSelectListItemOfTypeInfo()
        {
            return db.TypeInfo.Select(x => new SelectListItem
            {
                Value = Convert.ToInt32(x.TID).ToString(),
                Text = x.TypeName
            });
        }

        public bool AddOneCourse(CourseNameViewModel course)
        {
            bool flag = true;
            try
            {
                db.Course.InsertOnSubmit(new Course
                {
                    CourseID = Convert.ToDouble(course.CourseID),
                    CourseName = course.CourseName,
                    TID = course.TID
                });
                db.SubmitChanges();
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }
    }
}