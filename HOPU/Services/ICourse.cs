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
        /// 获得Course、TypeInfo表组成SelectListItem
        /// </summary>
        /// <returns></returns>
        IEnumerable<CourseNameViewModel> GetListOfCourseAndTypeInfo();
    }
}
