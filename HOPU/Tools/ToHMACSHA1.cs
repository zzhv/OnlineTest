using System;
using System.Security.Cryptography;

namespace HOPU.Tools
{
    public class ToHMACSHA1
    {
        /// <summary>
        /// 对传入的数据用指定Key进行加密
        /// </summary>
        /// <param name="encryptText">被加密的数据</param>
        /// <param name="encryptKey">用于加密的Key</param>
        /// <returns></returns>
        public static string toHMACSHA1(string encryptText, string encryptKey)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(encryptKey);
            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(encryptText);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }
    }
}