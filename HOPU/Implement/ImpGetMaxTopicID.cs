using HOPU.InterFace;
using HOPU.Models;
using System;
using System.Linq;

namespace HOPU.Implement
{
    public class ImpGetMaxTopicID : ITopic
    {
        private HopuDBDataContext db = new HopuDBDataContext();
        /// <summary>
        /// 获取最大TopicID
        /// </summary>
        /// <param name="Number">需要加在TopicID上的数</param>
        /// <returns></returns>
        public int GetMaxTopicID(int Number)
        {
            int realMaxTopicID = 0;
            var maxTopicID = db.Topic.Select(a => a.TopicID).Max();
            if (maxTopicID == 0)
            {
                realMaxTopicID = 1;
            }
            else
            {
                realMaxTopicID = Convert.ToInt32(maxTopicID + Number);
            }
            return realMaxTopicID;

        }

        /// <summary>
        /// 获取最大TopicID
        /// </summary>
        /// <returns></returns>
        public int GetMaxTopicID()
        {
            int realMaxTopicID = 0;
            var maxTopicID = db.Topic.Select(a => a.TopicID).Max();
            if (maxTopicID == 0)
            {
                realMaxTopicID = 1;
            }
            else
            {
                realMaxTopicID = Convert.ToInt32(maxTopicID);
            }
            return realMaxTopicID;
        }
    }
}