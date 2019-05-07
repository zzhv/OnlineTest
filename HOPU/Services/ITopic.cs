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
    }
}
