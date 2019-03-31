using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOPU.Models
{
    public class UnifiedTestListViewModel
    {
        public static List<SelectListItem> GetUtId()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            List<UniteTest> BrickTypeId = new List<UniteTest>();
            var result = from a in db.UniteTest
                         select new SelectListItem()
                         {
                             Text = "第" + a.UtId.ToString() + "号统测",
                             Value = a.UtId.ToString(),
                         };
            return result.ToList();

        }

        public static List<SelectListItem> GetStId()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = from a in db.SelfTest
                         select new SelectListItem()
                         {
                             Text = "第" + a.StId.ToString() + "号统测",
                             Value = a.StId.ToString(),
                         };
            return result.ToList();

        }
    }
}