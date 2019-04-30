using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOPU.Models
{
    [Table("SelfTest")]
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