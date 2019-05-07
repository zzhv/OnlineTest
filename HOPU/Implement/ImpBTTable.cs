using HOPU.Models;
using HOPU.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HOPU.Implement
{
    public class ImpBTTable : IBTTable
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        public IEnumerable<T> GetTableInfo<T>(string SqlStr)
        {
            return db.ExecuteQuery<T>(SqlStr);
        }
    }
}