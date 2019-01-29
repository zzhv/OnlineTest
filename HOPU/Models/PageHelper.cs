using HOPU.Models;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using X.PagedList;
namespace HOPU.Controllers
{
    public abstract class PageHelper : Controller
    {
        public static IPagedList<Topic> GetPagedTopic(int? page)
        {
            // return a 404 if user browses to before the first page
            //如果用户在第一页之前浏览，则返回404
            if (page.HasValue && page < 1)
                return null;
            // retrieve list from database/whereverand
            //从数据库/whereverand中检索列表
            var listUnpaged = GetStuffFromDatabase();
            // page the list
            //页面列表
            const int pageSize = 1;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);
            // return a 404 if user browses to pages beyond last page. special case first page if no items exist
            //如果用户浏览到最后一页之外的页面，则返回404。 特殊情况第一页，如果没有项目
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;
            return listPaged;
        }
        protected static IQueryable<Topic> GetStuffFromDatabase()
        {
            HopuDBDataContext c = new HopuDBDataContext();
            var result = from p in c.Topic
                         select p;
            return result;
        }

        /// <summary>
        /// 分类浏览
        /// </summary>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IPagedList<Topic> GetClassifiledTopic(int? page,string type)
        {
            if (page.HasValue && page < 1)
                return null;
            var listUnpaged = GetClassifiedList(type);
            const int pageSize = 5;
            var listPaged = listUnpaged.ToPagedList(page ?? 1, pageSize);
            if (listPaged.PageNumber != 1 && page.HasValue && page > listPaged.PageCount)
                return null;
            return listPaged;
        }
        //获取分类
        public static IQueryable<Topic> GetClassifiedList(string type)
        {
            HopuDBDataContext c = new HopuDBDataContext();
            var result = from p in c.Topic
                         where p.CourseID == int.Parse(type)
                         select p;
            return result;
        }
    }
}
