using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APIOffice.Controllers.pddApi
{
    public class PinduoduoSignHelper
    {
        private static string _clientSecret = "cbcd35c51ba36ce0092a559ca2a97985bd49f280"; // 替换为你的client_secret  

        /// <summary>
        /// 生成sign
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GenerateSign(Dictionary<string, string> parameters)
        {
            // 参数名按照字母顺序排序  
            var sortedParams = parameters.OrderBy(p => p.Key).ToList();

            // 拼接参数  
            StringBuilder sb = new StringBuilder();
            foreach (var param in sortedParams)
            {
                sb.Append(param.Key);
                //sb.Append("=");
                sb.Append(param.Value);
                //sb.Append("&");
            }

            // 去除最后一个&字符  
            //if (sb.Length > 0)
            //{
            //    sb.Remove(sb.Length - 1, 1);
            //}

            // 添加client_secret到首尾  
            string stringToSign =  _clientSecret + sb.ToString() + _clientSecret;

            // 使用MD5加密  
            using (MD5 md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));

                // 转换为大写十六进制字符串  
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }

                return builder.ToString().ToUpper();
            }
        }
    
    }
}
