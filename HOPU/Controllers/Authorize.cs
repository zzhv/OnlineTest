using HOPU.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace HOPU.Controllers
{
    public class Authorize : Controller
    {

        protected override void OnActionExecuted(ActionExecutedContext filterContext)//protected 只能被子类访问  
        {
            base.OnActionExecuted(filterContext);

            if (!GetUserType.GetUserTypeInfo(User.Identity.GetUserId()))//如果不是admin权限
            {
                filterContext.Result = Redirect("/SysHome/Home");//没有返回值， 所以不是return  是filterContexr.Result    
            }
        }

    }
}