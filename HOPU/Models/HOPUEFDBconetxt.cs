using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HOPU.Models
{
    public class HOPUEFDBconetxt : DbContext
    {
        public DbSet<SelfTestModel> SelfTest { get; set; }
        public DbSet<SelfTestScore> SelfTestScore { get; set; }
        public DbSet<SelfTestInfoModel> SelfTestInfo { get; set; }
    }
}