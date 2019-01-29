using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOPU.Controllers
{
    public class SessionController : Controller
    {
        //要过滤的控制器可以继承此Session  

        /// <summary>  
        /// 在控制器执行方法之前执行     暂时弃用 ss
        /// </summary>  
        /// <param name="filterContext"></param>  
        protected override void OnActionExecuted(ActionExecutedContext filterContext)//protected 只能被子类访问  
        {
            base.OnActionExecuted(filterContext);
            if (Session["userState"]==null)
            {
                filterContext.Result = Redirect("~/SysAdmin/AdminLogin");//  没有返回值， 所以不是return   是filterContexr.Result    
            }
        }

    }
}