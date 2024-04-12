using APIOffice.Controllers.pddApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Script.Serialization;

namespace jingkeyun.Data
{
    public class Good_Template
    {
        /// <summary>
        /// 云内调接口，会存在十几秒卡顿
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static BackMsg Get(TempRequset model)
        {
            BackMsg messData = new BackMsg();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                string Data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodsTemplate"].ToString(), parameters, 1, model);
                object jo = (JObject)JsonConvert.DeserializeObject(data.ToString());
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                messData = Serializer.Deserialize<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                messData.Code = 2001;
                messData.Mess = ex.Message.ToString();
            }
            return messData;

        }
        /// <summary>
        /// 云外调接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static BackMsg out_Get(TempRequset model)
        {
            BackMsg backMsg = new BackMsg();
            BackData backData = new BackData();
            //数据正常请求接口
            Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                { "type", "pdd.goods.logistics.template.get" },
                { "client_id", apiHelp.client_id },
                { "access_token", model.mall.mall_token },
                { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                { "data_type", "JSON" },
                };

            string sign = PinduoduoSignHelper.GenerateSign(parameters);

            parameters.Add("sign", sign);
            string resultJson = apiHelp.postApi("http://gw-api.pinduoduo.com/api/router", parameters);
            try
            {
                JToken jToken = JsonConvert.DeserializeObject<JToken>(resultJson);
                if (jToken.ToString().Contains("error_response"))//存在错误信息跳出
                {
                    throw new Exception(jToken["error_response"].ToString());
                }
                else
                {
                    backMsg.Code = 0;
                    backMsg.Mess = jToken["goods_logistics_template_get_response"]["logistics_template_list"].ToString();
                    return backMsg;
                }
            }
            catch (Exception ex)
            {
                backMsg.Code = 1002;
                backMsg.Mess = ex.Message;
                return backMsg;
            }
        }
    }
}
