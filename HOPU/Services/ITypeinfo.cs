using System.Collections.Generic;
using System.Web.Mvc;

namespace HOPU.Services
{
    public interface ITypeinfo
    {
        /// <summary>
        /// 获得Typeinfo表组成SelectListItem
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetSelectListItemOfTypeInfo();
    }
}
