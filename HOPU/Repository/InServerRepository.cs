using HOPU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOPU
{
    public class InServerRepository : IRepository<CourseNameViewModel>
    {
        public IEnumerable<CourseNameViewModel> GetUnifiedTestList()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = from p in db.TypeInfo
                         join c in db.Course on p.TID equals c.TID
                         orderby p.TID
                         select new CourseNameViewModel
                         {
                             TId = p.TID,//编号
                             TypeName = p.TypeName,//
                             CourseId = c.CourseID,//题目分类的ID
                             CourseName = c.CourseName//题目分类的名称
                         };
            return result;
        }
    }
}
