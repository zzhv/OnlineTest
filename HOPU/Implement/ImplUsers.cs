using HOPU.Models;
using HOPU.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Implement
{
    public class ImplUsers : IUsers
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        public int GetUsersCount()
        {
            return db.AspNetUsers.Count();
        }
    }
}