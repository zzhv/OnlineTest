using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HOPU.Models
{
    public class ClassifiedBrowseModel
    {
        public string coursename { get; set; }
        public static List<SelectListItem> ListItem()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            //Models.Course stu = db.Course.Where(c => c.S_ID == id).FirstOrDefault();
            //2、将数据绑定到实体集合，然后再将实体集合中的数据绑定到selectedItem集合中
            List<SelectListItem> listclass = db.Course.ToList().Select(c => new SelectListItem() { Text = c.CourseName, Value = c.CourseID.ToString() }).ToList();
            return listclass;
        }
    }
}