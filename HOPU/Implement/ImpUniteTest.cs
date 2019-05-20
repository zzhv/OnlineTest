using HOPU.Models;
using HOPU.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HOPU.Implement
{
    public class ImpUniteTest : IUniteTest
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        /// <summary>
        /// 获取今日统测总数
        /// </summary>
        /// <returns></returns>
        public int GetTodayUnifiedTestCount()
        {
            return db.UniteTest.Where(x => x.StartTime > DateTime.Now.Date).Count();
        }

        /// <summary>
        /// 获取统测总数
        /// </summary>
        /// <returns></returns>
        public int GetunifiedTestCount()
        {
            return db.UniteTest.Count();
        }

        /// <summary>
        /// 获取所有统测信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UniteTest> GetUniteTestInfo()
        {
            return db.UniteTest.Select(a => a).OrderByDescending(a => a.UtId);

        }

        /// <summary>
        /// 根据统测ID获取统测信息
        /// </summary>
        /// <param name="UtId"></param>
        /// <returns></returns>
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