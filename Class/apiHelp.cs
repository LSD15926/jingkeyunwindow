using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace jingkeyun
{
    public static class apiHelp
    {

        public static string client_id = "2b0c43de6a3e468fbf748bba48b8ee5a";

        public static string requestPost(string strUrl, string param, string Method = "POST")
        {
            //Console.WriteLine("HTTP的POST请求:" + strUrl + ";数据:" + param);

            HttpWebRequest httpWebRequest = WebRequest.Create(strUrl) as HttpWebRequest;
            httpWebRequest.Method = Method;      //指定允许数据发送的请求的一个协议方法
            httpWebRequest.ContentType = "application/json";       //设置 ContentType 属性设置为适当的值
            httpWebRequest.Headers.Add("Cookie", "api_uid=Ck5xaGXukpJJmwBXr/UdAg; _nano_fp=XpmaX0gqXqUon5E8l9_vZFejoXvQqKUtk9~PXlzp; " +
                    "webp=1; dilx=OUmSENY_j_sYwMVHa~YJd; PDDAccessToken=JMVWVPVHFDBWBIQP7IIOWYYFI7ATTTIDTAOAQMLEDKWNWW7Y34ZA1217661; " +
                    "pdd_user_id=2952916144502; pdd_user_uin=2GA3Q7AVCT5ESLXUHJCPNDAFMM_GEXDA; jrpl=fwBGCU2WpGjIQEIbJrM6mZkf42kHc7ye; " +
                    "njrpl=fwBGCU2WpGjIQEIbJrM6mZkf42kHc7ye; rec_list_chat_list_rec_list=rec_list_chat_list_rec_list_0npyrd; " +
                    "pdd_vds=gaFUkVhMZHpHhKcScFZXqkzRrWzRcShUhSCUpWMgHVvUqJMRWkzjvFrKXUHk; api_uid=CiQq/2Xunc5p7wBmrbhKAg==");
            httpWebRequest.Timeout = 90000;
            if (param != "")
            {
                byte[] data = Encoding.UTF8.GetBytes(param);
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);     //写入数据
                    stream.Close();
                }
            }
            WebResponse webResponse = httpWebRequest.GetResponse() as HttpWebResponse;        //发起请求,得到返回对象
            Stream dataStream = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream, Encoding.UTF8);
            string returnStr = reader.ReadToEnd();
            // Clean up the streams and the response.
            reader.Close();
            webResponse.Close();
            return returnStr;
        }
        /// <summary>
        /// 正则截取地址
        /// </summary>
        /// <param name="input"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static List<string> GetCenterString(String input, String left, String right)
        {
            List<string> list = new List<string>();
            Regex reg = new Regex(left + "(((?!" + left + ").)+?)" + right);
            foreach (Match m in reg.Matches(input))
            {
                string value = m.Value;
                value = value.Substring(left.Length, value.Length - left.Length);
                value = value.Substring(0, value.Length - right.Length);
                list.Add(value);
            }

            return list;
        }



        public static string postApi(string url, Dictionary<string, string> parameters,int type=0, object bodyData = null)
        {
            try
            {
                var request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "Post";
                request.ContentType = "application/json"; //x - www - form - urlencoded
                request.Headers.Add("x-Hmac-Auth-origin", "data");
                request.Timeout = 3000000;
                byte[] postData = null;
                if (type == 0)
                {
                    request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    postData = Encoding.UTF8.GetBytes(BuildQuery(parameters, "utf8"));
                }
                else if (type == 1)
                {
                    request.ContentType = "application/json";
                    postData = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(bodyData));
                }
                request.ContentLength = postData.Length;
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(postData, 0, postData.Length);
                    reqStream.Close();
                }
                string result = "";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public static string postApi(string strUrl, string param)
        {
            //Console.WriteLine("HTTP的POST请求:" + strUrl + ";数据:" + param);

            HttpWebRequest httpWebRequest = WebRequest.Create(strUrl) as HttpWebRequest;
            httpWebRequest.Method = "POST";      //指定允许数据发送的请求的一个协议方法
            httpWebRequest.ContentType = "application/json";       //设置 ContentType 属性设置为适当的值
            httpWebRequest.Timeout = 90000;
            if (param != "")
            {
                byte[] data = Encoding.UTF8.GetBytes(param);
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);     //写入数据
                    stream.Close();
                }
            }
            WebResponse webResponse = httpWebRequest.GetResponse() as HttpWebResponse;        //发起请求,得到返回对象
            Stream dataStream = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream, Encoding.UTF8);
            string returnStr = reader.ReadToEnd();
            // Clean up the streams and the response.
            reader.Close();
            webResponse.Close();
            return returnStr;
        }


        public static string BuildQuery(IDictionary<string, string> parameters, string encode)
        {
            StringBuilder postData = new StringBuilder();
            if (parameters != null)
            {
                bool hasParam = false;
                IEnumerator<KeyValuePair<string, string>> dem = parameters.GetEnumerator();
                while (dem.MoveNext())
                {
                    string name = dem.Current.Key;
                    string value = dem.Current.Value;
                    // 忽略参数名或参数值为空的参数
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (hasParam)
                        {
                            postData.Append("&");
                        }
                        postData.Append(name);
                        postData.Append("=");
                        if (encode == "gb2312")
                        {
                            postData.Append(HttpUtility.UrlEncode(value, Encoding.GetEncoding("gb2312")));
                        }
                        else if (encode == "utf8")
                        {
                            postData.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                        }
                        else
                        {
                            postData.Append(value);
                        }
                        hasParam = true;
                    }
                }
            }
            return postData.ToString();
        }

    }


}
