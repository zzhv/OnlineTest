using HOPU.Models;
using HOPU.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HOPU.Implement
{
    public class ImpUniteTestScore : IUniteTestScore
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        public IEnumerable<UniteTestScore> GetUniteTestScore(int? UtId, string UserName)
        {
            return db.UniteTestScore.Where(a => a.UtId == UtId && a.UserName == UserName);
        }

        public bool InsertScore(UniteTestScore uniteTestScoreInfo)
        {
            bool flag = false;
            try
            {
                //var uniteTestScore = new UniteTestScore
                //{
                //    UtId = uniteTestScoreInfo.UtId,
                //    RealUserName = uniteTestScoreInfo.RealUserName,
                //    UserName = uniteTestScoreInfo.UserName,
                //    EndTime = DateTime.Now,
                //    Score = uniteTestScoreInfo.Score,
                //};
                db.UniteTestScore.InsertOnSubmit(uniteTestScoreInfo);
                db.SubmitChanges();
                if (uniteTestScoreInfo.UtId > 0)
                {
                    flag = true;
                }
            }
            catch (Exception)
            {
                flag = false;
                throw;
            }
            return flag;
        }
    }
}