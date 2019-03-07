using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOPU.Models
{
    public class UnifiedTestModel
    {
        public string UserAnswer { get; set; }
        public string RealAnswer { get; set; }
        public bool IsTrue { get; set; }


        public static List<SelectListItem> GetUtId()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            List<UniteTest> BrickTypeId = new List<UniteTest>();
            var result = from a in db.UniteTest
                         select new SelectListItem()
                         {
                             Text = a.UtId.ToString(),
                             Value = a.UtId.ToString(),
                         };
            return result.ToList();

        }
    }
}