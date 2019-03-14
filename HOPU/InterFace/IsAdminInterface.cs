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
    public interface IsAdminInterface
    {
        bool Isadmins(string Id);
    }
}
