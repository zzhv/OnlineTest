using System;
using System.ComponentModel.DataAnnotations;

namespace HOPU.Models
{
    public class SelfTestModel
    {
        [Key]
        public int StId { get; set; }
        public DateTime StartTime { get; set; }
        public int TimeLenth { get; set; }
        public int TopicCount { get; set; }
        [MaxLength]
        public string CourseId { get; set; }
        [MaxLength]
        public string CourseName { get; set; }
    }
}