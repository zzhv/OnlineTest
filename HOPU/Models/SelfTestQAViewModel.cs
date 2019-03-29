using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Models
{
    public class SelfTestQAViewModel
    {
        public string UserAnswer { get; set; }
        public string RealAnswer { get; set; }
        public bool IsTrue { get; set; }

    }

    public class SelfTestNewTopicIdViewModel
    {
        public long? StId { get; set; }

        public string TopicID { get; set; }

        public string Title { get; set; }

        public string AnswerA { get; set; }

        public string AnswerB { get; set; }

        public string AnswerC { get; set; }

        public string AnswerD { get; set; }

        public string Answer { get; set; }

        public string CourseID { get; set; }
    }
}