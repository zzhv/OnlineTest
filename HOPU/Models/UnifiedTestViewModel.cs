using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Models
{
    /// <summary>
    /// 统测考试时候的model
    /// </summary>
    public class UnifiedTestViewModel
    {
        public string UserAnswer { get; set; }
        public string RealAnswer { get; set; }
        public bool IsTrue { get; set; }

    }
}