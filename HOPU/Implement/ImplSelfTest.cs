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

        /// <summary>
        /// 获取今日独测总数
        /// </summary>
        /// <returns></returns>
        public int GetTodaySelfTestCount()
        {
            return db.SelfTest.Where(x => x.StartTime > DateTime.Now.Date).Count();
        }
    }
}