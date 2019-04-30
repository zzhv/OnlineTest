using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HOPU.Models
{
    [Table("SelfTestScore")]
    public class SelfTestScoreModel
    {
        //Id, UtId, RealUserName, UserName, EndTime, Score
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]//主键和自增                                                                                                                                                                                                                                                       
        public int Id { get; set; }
        public int StId { get; set; }//独测号
        [MaxLength]
        public string RealUserName { get; set; }
        [MaxLength]
        public string UserName { get; set; }
        public DateTime EndTime { get; set; }
        public int Score { get; set; }
    }
}