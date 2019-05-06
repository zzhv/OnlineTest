using System.Collections.Generic;
using System.Web.Mvc;

namespace HOPU.InterFace
{
    public interface ICourse
    {
        IEnumerable<SelectListItem> GetSelectListItemOfCourseType();
    }
}
