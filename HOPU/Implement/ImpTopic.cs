using HOPU.Services;
using HOPU.Models;
using System;
using System.Linq;

namespace HOPU.Implement
{
    public class ImpTopic : ITopic
    {
        private HopuDBDataContext db = new HopuDBDataContext();

        public bool DeleteAllTopic(double[] topics)
        {
            bool flag = true;
            try
            {
                for (int i = 0; i < topics.Length; i++)
                {
                    db.Topic.DeleteOnSubmit(db.Topic.Where(x => x.TopicID == topics[i]).SingleOrDefault());
                    db.SubmitChanges();
                }
            }
            catch (Exception)
            {
                flag = false;
                throw;
            }
            return flag;
        }

        public bool DeleteTopic(Topic topic)
        {
            bool flag = true;
            try
            {
                var delete = db.Topic.Where(z => z.TopicID == topic.TopicID).SingleOrDefault();
                db.Topic.DeleteOnSubmit(delete);
                db.SubmitChanges();
            }
            catch (Exception)
            {
                flag = false;
                throw;
            }
            return flag;
        }

        public bool EditTopic(Topic topic)
        {
            bool flag = true;
            try
            {
                var Edit = db.Topic.Where(x => x.TopicID == topic.TopicID).Select(a => a);
                foreach (var i in Edit)
                {
                    i.TopicID = topic.TopicID;
                    i.Title = topic.Title;
                    i.AnswerA = topic.AnswerA;
                    i.AnswerB = topic.AnswerB;
                    i.AnswerC = topic.AnswerC;
                    i.AnswerD = topic.AnswerD;
                    i.Answer = topic.Answer;
                    i.CourseID = topic.CourseID;
                }
                db.SubmitChanges();
                //if (topic.TopicID > 0)
                //{
                //    flag = true;
                //}
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

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

        public bool InsertOneTopic(Topic topic, string[] Answer)
        {
            bool flag = true;
            string AnswerStr = string.Join("", Answer);
            topic.Answer = AnswerStr;
            try
            {
                db.Topic.InsertOnSubmit(topic);
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }
    }
}