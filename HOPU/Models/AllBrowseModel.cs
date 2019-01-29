using System.Web.Mvc;
using HOPU.Models;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using X.PagedList;
using Microsoft.AspNet.Identity;
using System.Linq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HOPU.Models
{
    public class AllBrowseModel
    {
        public static string GetTopicCount()
        {
            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.Topic.Count();
            return result.ToString();
        }
    }

    public class GetRealUserName
    {
        public static string GetRealName(string Id)
        {
            string realUserName = "";
            HopuDBDataContext db = new HopuDBDataContext();
            var result = from a in db.AspNetUsers
                         where a.Id == Id
                         select a;
            foreach (var item in result)
            {
                realUserName = item.RealUserName;
            }
            return realUserName;
        }
    }


    public class GetUserType
    {
        public static bool GetUserTypeInfo(string id)
        {
            string userType = "";
            HopuDBDataContext db = new HopuDBDataContext();
            var result = db.UserClaims.Where(a => a.UserId == id).Select(a=>a.ClaimType).ToArray();
            if (result.Count() >= 1)
            {
                userType = result[0];
            }
            switch (userType)
            {
                case "admin":
                    return true;
                default:
                    return false;
            }
        }

        //[DisplayName("时长")]
        //[Required(ErrorMessage = "新密码不能为空")]
        //public int TimeLenth { get; set; }
    }
}