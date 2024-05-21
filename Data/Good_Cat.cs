using APIOffice.Controllers.pddApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Script.Serialization;

namespace jingkeyun.Data
{
    public class Good_Cat
    {
        public static BackMsg Get(RequstCat requstCat)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodsCat"].ToString(), parameters, 1, requstCat);
                backMsg = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }

            return backMsg;
        }

        public static BackMsg getCatRule(Goods_detailModel requstCat)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                long parentId = MyConvert.ToLong(requstCat.cat_id);

                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.cat.rule.get" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", requstCat.mall.mall_token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                };
                parameters.Add("cat_id", parentId.ToString());
                parameters.Add("goods_id", requstCat.goods_id.ToString());
                string sign = PinduoduoSignHelper.GenerateSign(parameters);

                parameters.Add("sign", sign);
                string Url = "https://gw-api.pinduoduo.com/api/router";
                string resultJson = apiHelp.postApi(Url, parameters);

                JToken jToken = JsonConvert.DeserializeObject<JToken>(resultJson);
                if (jToken.ToString().Contains("error_response"))//存在错误信息跳出
                {
                    backMsg.Code = 1;
                    backMsg.Mess = "获取商品分类失败！";
                    return backMsg;
                }
                else
                {
                    backMsg.Mess= jToken["cat_rule_get_response"]["goods_properties_rule"]["properties"].ToString();//商品属性信息
                }
            }
            catch (Exception ex)
            {
                backMsg.Code = 100;
                backMsg.Mess = ex.Message;
            }

            return backMsg;
        }
    }
}
