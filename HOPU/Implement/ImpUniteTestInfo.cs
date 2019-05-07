using HOPU.Models;
using HOPU.Services;
using System.Collections.Generic;
using System.Linq;

namespace HOPU.Implement
{
    public class ImpUniteTestInfo : IUniteTestInfo
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        public IEnumerable<UnifiedTestNewTopicIdViewModel> GetUnifiedTestTopics(int? UtId, string UserName)
        {
            return db.UniteTestInfo.Where(a => a.UtId == UtId).ToList().Select(c =>
                  new UnifiedTestNewTopicIdViewModel
                  {
                      UtId = c.UtId,
                      Title = c.Title,
                      AnswerA = c.AnswerA,
                      AnswerB = c.AnswerB,
                      AnswerC = c.AnswerC,
                      AnswerD = c.AnswerD,
                      Answer = c.Answer,
                      CourseID = c.CourseID,
                      TopicID = Tools.ToHMACSHA1.toHMACSHA1(c.TopicID.ToString(), UserName)
                  });
        }
    }
}