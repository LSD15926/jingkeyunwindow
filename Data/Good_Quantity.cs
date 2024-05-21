using APIOffice.Controllers.pddApi;
using jingkeyun.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace jingkeyun.Data
{
    public class Good_Quantity
    {
        public static BackMsg Update(List<requestQuantity> model)
        {
            List<BackMsg> messData = new List<BackMsg>();
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                string Data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodsQuantity"].ToString(), parameters, 1, model);
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
                    InitUser.MainForm.LogEvent("出错商品id：【" + model[i].goods_id + "】错误原因：" + messData[i].Mess);
                    cnt++;
                }
            }
            backMsg.Mess = $"修改完成：SKu总数{messData.Count},成功{messData.Count - cnt}个,失败{cnt}个！\n失败原因请查看操作日志栏目";

            InitUser.MainForm.LogEvent($"修改完成：SKu总数{messData.Count},成功{messData.Count - cnt}个,失败{cnt}个！");
            return backMsg;
        }

        public static BackMsg Update_local(List<requestQuantity> model)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            List<BackMsg> results = new BackMsg[model.Count].ToList();
            Parallel.For(0, model.Count, i =>
            {
                //数据正常请求接口
                Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.quantity.update" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", model[i].Malls.mall_token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                    { "goods_id", model[i].goods_id.ToString() },
                    { "quantity", model[i].quantity.ToString() },
                    { "sku_id", model[i].sku_id.ToString() },
                    { "outer_id", model[i].outer_id.ToString() },
                    { "update_type", model[i].update_type.ToString() },
                };
                string sign = PinduoduoSignHelper.GenerateSign(parameters);

                parameters.Add("sign", sign);
                string Url = "https://gw-api.pinduoduo.com/api/router";
                string resultJson = apiHelp.postApi(Url, parameters);
                JToken jToken = JsonConvert.DeserializeObject<JToken>(resultJson);
                if (jToken.ToString().Contains("error_response"))//存在错误信息跳出
                {
                    backMsg.Code = 101;
                    try
                    {
                        if (!string.IsNullOrEmpty(jToken["error_response"]["error_msg"].ToString()))
                        {
                            InitUser.MainForm.LogEvent($"库存修改失败：商品id【{model[i].goods_id}】 SKU【{model[i].sku_id}】。失败原因：{jToken["error_response"]["error_msg"].ToString()}");
                        }
                        else if (!string.IsNullOrEmpty(jToken["error_response"]["sub_msg"].ToString()))
                        {
                            InitUser.MainForm.LogEvent($"库存修改失败：商品id【{model[i].goods_id}】 SKU【{model[i].sku_id}】。失败原因：{jToken["error_response"]["sub_msg"].ToString()}");
                        }
                        else
                        {
                            InitUser.MainForm.LogEvent($"库存修改失败：商品id【{model[i].goods_id}】 SKU【{model[i].sku_id}】。失败原因：{jToken["error_response"].ToString()}");
                        }
                    }
                    catch
                    {
                        InitUser.MainForm.LogEvent($"库存修改失败：商品id【{model[i].goods_id}】 SKU【{model[i].sku_id}】。失败原因：{jToken["error_response"].ToString()}");
                    }
                }
                else
                {
                    backMsg.Code = 0;
                    backMsg.Mess = jToken.ToString();
                }
                results[i] = backMsg;
            });
            int cnt = 0;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].Code != 0)
                {
                    cnt++;
                }
            }
            backMsg.Mess = $"修改完成：SKu总数{results.Count},成功{results.Count - cnt}个,失败{cnt}个！\n失败原因请查看操作日志栏目";
            InitUser.MainForm.LogEvent($"修改完成：SKu总数{results.Count},成功{results.Count - cnt}个,失败{cnt}个！");
            return backMsg;
        }
    }
}
