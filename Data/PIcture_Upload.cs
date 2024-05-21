using APIOffice.Controllers.pddApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using RestSharp;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Security.Policy;
using Pdd_Models.Models;
using System.Reflection;
using System.Threading;

namespace jingkeyun.Data
{
    public class PIcture_Upload
    {
        public static BackMsg imageUpload(string image, string token)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.image.upload" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                    { "image",image}
                };
                string sign = PinduoduoSignHelper.GenerateSign(parameters);

                parameters.Add("sign", sign);

                string resultJson = apiHelp.postApi("https://gw-api.pinduoduo.com/api/router", parameters);
                JToken jToken = JsonConvert.DeserializeObject<JToken>(resultJson);
                if (jToken.ToString().Contains("error_response"))//存在错误信息跳出
                {
                    throw new Exception(jToken["error_response"].ToString());
                }
                else
                {
                    backMsg.Code = 0;
                    backMsg.Mess = jToken["goods_image_upload_response"]["image_url"].ToString();
                }

            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            return backMsg;
        }

        public static BackMsg create(FileSpaceUpload model)
        {

            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.material.create" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", model.access_token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                };
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    object value = property.GetValue(model);

                    List<string> Names = new List<string>() { "access_token" };
                    if (Names.Contains(property.Name))
                        continue;
                    if (value != null && value.ToString() != "") // 检查值是否为null  
                    {
                        parameters.Add(property.Name, value.ToString());
                    }
                }
                string sign = PinduoduoSignHelper.GenerateSign(parameters);

                parameters.Add("sign", sign);
                string Url = "https://gw-api.pinduoduo.com/api/router";
                string resultJson = apiHelp.postApi(Url, parameters);

                JToken jToken = JsonConvert.DeserializeObject<JToken>(resultJson);
                if (jToken.ToString().Contains("error_response"))//存在错误信息跳出
                {
                    if (!string.IsNullOrEmpty(jToken["error_response"]["error_msg"].ToString()))
                    {
                        backMsg.Mess = "商品id:" + model.goods_id + "修改失败！错误信息：" + jToken["error_response"]["error_msg"].ToString();
                    }
                    else if (!string.IsNullOrEmpty(jToken["error_response"]["sub_msg"].ToString()))
                    {
                        backMsg.Mess = "商品id:" + model.goods_id + "修改失败！错误信息：" + jToken["error_response"]["sub_msg"].ToString();
                    }
                    else
                    {
                        backMsg.Mess = "商品id:" + model.goods_id + "修改失败！错误信息：" + jToken["error_response"].ToString();
                    }
                }
                else
                {
                    backMsg.Code = 0;
                    backMsg.Mess = jToken.ToString();
                }
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            return backMsg;
        }

        public static BackMsg upload(string image, string token)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.img.upload" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                };
                string sign = PinduoduoSignHelper.GenerateSign(parameters);

                parameters.Add("sign", sign);
                string Url = "https://gw-upload.pinduoduo.com/api/upload";//?+ apiHelp.BuildQuery(parameters, "utf8");
                var result = UploadRequest(parameters, image, Encoding.UTF8, Url,"file");

                 JToken jToken = JsonConvert.DeserializeObject<JToken>(result);
                if (jToken.ToString().Contains("error_response"))//存在错误信息跳出
                {
                    throw new Exception(jToken["error_response"].ToString());
                }
                else
                {
                    backMsg.Code = 0;
                    backMsg.Mess = jToken["goods_img_upload_response"]["url"].ToString();
                }
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            return backMsg;
        }


        /// <summary>
        /// 上传至文件空间
        /// </summary>
        /// <param name="image"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static BackMsg uploadToFileSpace(string image, string token)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.filespace.image.upload" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                };
                string sign = PinduoduoSignHelper.GenerateSign(parameters);

                parameters.Add("sign", sign);
                string Url = "https://gw-upload.pinduoduo.com/api/upload";//?+ apiHelp.BuildQuery(parameters, "utf8");
                var result = UploadRequest(parameters, image, Encoding.UTF8, Url, "file");

                JToken jToken = JsonConvert.DeserializeObject<JToken>(result);
                if (jToken.ToString().Contains("error_response"))//存在错误信息跳出
                {
                    throw new Exception(jToken["error_response"].ToString());
                }
                else
                {
                    backMsg.Code = 0;
                    backMsg.Mess = jToken["goods_filespace_image_upload_response"].ToString();
                }
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            return backMsg;
        }


        /// <summary>
        /// 上传文件接口
        /// </summary>
        /// <param name="dictParam">上传参数</param>
        /// <param name="fileUrl">文件的地址</param>
        /// <param name="url">请求接口地址</param>
        /// <param name="encoding">字符编码格式</param>
        /// <param name="keyName">文件参数的参数名称</param>
        /// <returns></returns>
        public static string UploadRequest(Dictionary<string, string> dictParam, string fileUrl, Encoding encoding, string url, string keyName)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebHeaderCollection header = request.Headers;
            request.Method = "post";

            //读取文件信息
            FileStream fileStream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            byte[] UpdateFile = bytes;//转换为二进制
            if (UpdateFile.Length == 0)
            {
                return "上传文件无效";
            }
            string Boundary = "--WebKitFormBoundary39B5a5e2FWoGbphs";
            //构造POST请求体
            StringBuilder PostContent = new StringBuilder("");

            //获取文件名称
            string filename = "";
            filename = Path.GetFileName(fileUrl);

            //组成普通参数信息
            foreach (KeyValuePair<string, string> item in dictParam)
            {
                PostContent.Append("--" + Boundary + "\r\n")
                           .Append("Content-Disposition: form-data; name=\"" + item.Key + "\"" + "\r\n\r\n" + (string)item.Value + "\r\n");
            }
            byte[] PostContentByte = encoding.GetBytes(PostContent.ToString());

            //处理文件参数信息
            StringBuilder FileContent = new StringBuilder();
            FileContent.Append("--" + Boundary + "\r\n")
                    .Append("Content-Disposition:form-data; name=\"" + keyName + "\";filename=\"" + filename + "\"" + "\r\n\r\n");
            byte[] FileContentByte = encoding.GetBytes(FileContent.ToString());
            request.ContentType = "multipart/form-data;boundary=" + Boundary;
            byte[] ContentEnd = encoding.GetBytes("\r\n--" + Boundary + "--\r\n");//请求体末尾，后面会用到
            //定义请求流
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(PostContentByte, 0, PostContentByte.Length);//写入参数
            myRequestStream.Write(FileContentByte, 0, FileContentByte.Length);//写入文件信息
            myRequestStream.Write(UpdateFile, 0, UpdateFile.Length);//文件写入请求流中
            myRequestStream.Write(ContentEnd, 0, ContentEnd.Length);//写入结尾   
            myRequestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encodingth = response.ContentEncoding;
            if (encodingth == null || encodingth.Length < 1)
            {
                encodingth = "GBK"; //默认编码,根据需求自己指定 
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encodingth));
            string retString = reader.ReadToEnd();
            return retString;
        }

    }
}
