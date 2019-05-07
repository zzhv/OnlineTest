using HOPU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HOPU.Implement
{
    public class ImpTypeInfo : Services.ITypeinfo
    {
        private HopuDBDataContext db = new HopuDBDataContext();

       
        public IEnumerable<SelectListItem> GetSelectListItemOfTypeInfo()
        {
            return db.TypeInfo.Select(a => new SelectListItem
            {
                Value = Convert.ToInt32(a.TID).ToString(),
                Text = a.TypeName
            });
        }
    }
}