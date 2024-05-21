using APIOffice.Controllers.pddApi;
using jingkeyun.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;

namespace jingkeyun.Data
{
    public class Good_Sale
    {
        public static BackMsg Status(List<requsetSaleBody> model)
        {
            List<BackMsg> messData = new List<BackMsg>();
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                string Data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodsSale"].ToString(), parameters, 1, model);
                messData = JsonConvert.DeserializeObject<List<BackMsg>>(data.ToString());
            }
            catch (Exception ex)
            {
                
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            int cnt = 0;
            for (int i = 0; i < messData.Count; i++)
            {
               
                if (messData[i].Code != 0)
                {
                    InitUser.MainForm.LogEvent("出错商品id：【" + model[i].goods_id+"】错误原因："+ messData[i].Mess);
                    cnt++;
                }
            }
            backMsg.Mess = $"修改完成：商品总数{messData.Count},成功{messData.Count - cnt}个,失败{cnt}个！\n失败原因请查看操作日志栏目";

            InitUser.MainForm.LogEvent($"修改完成：商品总数{messData.Count},成功{messData.Count - cnt}个,失败{cnt}个！");
            return backMsg;
        }


        public static BackMsg SetOne(requsetSaleBody model)
        {
            List<BackMsg> messData = new List<BackMsg>();
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.sale.status.set" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", model.Malls.mall_token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                    { "goods_id", model.goods_id.ToString() },
                    { "is_onsale", model.is_onsale.ToString() },

                };
                string sign = PinduoduoSignHelper.GenerateSign(parameters);

                parameters.Add("sign", sign);
                string Url = "https://gw-api.pinduoduo.com/api/router";
                string resultJson = apiHelp.postApi(Url, parameters);
                JToken jToken = JsonConvert.DeserializeObject<JToken>(resultJson);
                if (jToken.ToString().Contains("error_response"))//存在错误信息跳出
                {
                    backMsg.Code = 101;
                    if (!string.IsNullOrEmpty(jToken["error_response"]["error_msg"].ToString()))
                    {
                        backMsg.Mess = "错误信息：" + jToken["error_response"]["error_msg"].ToString();
                    }
                    else if (!string.IsNullOrEmpty(jToken["error_response"]["sub_msg"].ToString()))
                    {
                        backMsg.Mess = "错误信息：" + jToken["error_response"]["sub_msg"].ToString();
                    }
                    else
                    {
                        backMsg.Mess = "错误信息：" + jToken["error_response"].ToString();
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
    }
}
