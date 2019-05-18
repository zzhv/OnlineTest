using HOPU.Models;
using HOPU.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Implement
{
    public class ImplSelfTest : ISelfTest
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        public int GetTodaySelfTestCount()
        {
            return db.SelfTest.Where(x => x.StartTime > DateTime.Now.Date).Count();
        }
    }
}