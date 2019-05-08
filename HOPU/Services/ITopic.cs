using HOPU.Models;

namespace HOPU.Services
{
    public interface ITopic
    {
        /// <summary>
        /// 获得最大的TopicID
        /// </summary>
        /// <returns></returns>
        int GetMaxTopicID();

        /// <summary>
        /// 获得最大的TopicID并加上Number
        /// </summary>
        /// <param name="Number">需要加减的数</param>
        /// <returns></returns>
        int GetMaxTopicID(int Number);

        /// <summary>
        /// 单项题目的编辑
        /// </summary>
        /// <param name="topic">Topic对象</param>
        /// <returns></returns>
        bool EditTopic(Topic topic);

        /// <summary>
        /// 单项删除
        /// </summary>
        /// <param name="topic">topic对象</param>
        /// <returns></returns>
        bool DeleteTopic(Topic topic);

        /// <summary>
        /// 多选删除
        /// </summary>
        /// <param name="topics">题目题号的数组</param>                               
        /// <returns></returns>
        bool DeleteAllTopic(double[] topics);

        /// <summary>
        /// 添加单个题目
        /// </summary>
        /// <param name="topic">Topic对象</param>
        /// <param name="Answer">题目答案，因为是数组的问题，单独获取</param>
        /// <returns></returns>
        bool InsertOneTopic(Topic topic, string[] Answer);

    }
}
