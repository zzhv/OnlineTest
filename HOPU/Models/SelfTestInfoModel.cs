using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOPU.Models
{
    public class SelfTestInfoModel
    {
        //id, UtId, TopicID, Title, AnswerA, AnswerB, AnswerC, AnswerD, Answer, CourseID
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]//主键和自增                                                                                                                                                                                                                                                       
        public int Id { get; set; }
        public int StId { get; set; }
        public int TopicID { get; set; }
        [MaxLength]
        public string Title { get; set; }
        [MaxLength]
        public string AnswerA { get; set; }
        [MaxLength]
        public string AnswerB { get; set; }
        [MaxLength]
        public string AnswerC { get; set; }
        [MaxLength]
        public string AnswerD { get; set; }
        [MaxLength]
        public string Answer { get; set; }
        [MaxLength]
        public string CourseID { get; set; }
    }
}