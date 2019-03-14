using HOPU.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOPU.InterFace
{
    public class IsAdmin : IsAdminInterface
    {
        public bool Isadmins(string Id)
        {
            if (GetUserType.GetUserTypeInfo(Id))//如果是admin权限
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}