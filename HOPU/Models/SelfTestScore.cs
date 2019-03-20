using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HOPU.Models
{
    public class SelfTestScore
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