using HOPU.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HOPU.Services
{
    public interface ICourse
    {
        /// <summary>
        /// 获得Course表的Name与ID组成SelectListItem
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetSelectListItemOfCourseType();

        /// <summary>
        /// 获得Course、TypeInfo表ToList
        /// </summary>
        /// <returns></returns>
        IEnumerable<CourseNameViewModel> GetListOfCourseAndTypeInfo();

        /// <summary>
        /// 获得TypeInfo表的Name与ID组成SelectListItem
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetSelectListItemOfTypeInfo();

        /// <summary>
        /// 编辑CourseName和TypeName
        /// </summary>
        /// <param name="course">数据</param>
        /// <returns></returns>
        bool EditCourseNameAndTypeName(CourseNameViewModel course);

        /// <summary>
        /// 删除单个Course
        /// </summary>
        /// <param name="course">数据</param>
        /// <returns></returns>
        bool DeleteCourse(CourseNameViewModel course);

        /// <summary>
        /// 最大二级分类ID
        /// </summary>
        /// <returns></returns>
        int MaxCourseID();

        /// <summary>
        /// 最大一级分类ID
        /// </summary>
        /// <returns></returns>
        int MaxTID();

        /// <summary>
        /// 添加一级与二级目录
        /// </summary>
        /// <param name="course">数据</param>
        /// <returns></returns>
        bool AddTypeAndCourse(CourseNameViewModel course);

        /// <summary>
        /// 添加二级分类
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        bool AddOneCourse(CourseNameViewModel course);


    }
}
