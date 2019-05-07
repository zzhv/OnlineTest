using HOPU.Models;
using HOPU.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Implement
{
    public class ImpUniteTest : IUniteTest
    {
        private HopuDBDataContext db = new HopuDBDataContext();
        public IEnumerable<UniteTest> GetUniteTestInfo()
        {
            return db.UniteTest.Select(a => a).OrderByDescending(a => a.UtId);

        }

        public IEnumerable<UniteTest> GetUniteTestInfo(int? UtId)
        {
            return db.UniteTest.Where(a => a.UtId == UtId).ToList().Select(x => new UniteTest
            {
                UtId = x.UtId,
                StartTime = x.StartTime,
                TimeLenth = x.TimeLenth,
                TopicCount = x.TopicCount,
            });
        }
    }
}