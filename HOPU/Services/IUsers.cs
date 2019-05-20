using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HOPU.Services
{
    public interface IUsers
    {
        /// <summary>
        /// 获取用户总数
        /// </summary>
        /// <returns></returns>
        int GetUsersCount();
    }
}