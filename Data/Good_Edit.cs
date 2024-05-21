using APIOffice.Controllers.pddApi;
using DevComponents.AdvTree;
using jingkeyun.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace jingkeyun.Data
{
    public class Good_Edit
    {
        //private static BackMsg Edit(List<RequstGoodEditModel> model)
        //{
        //    List<BackMsg> messData = new List<BackMsg>();
        //    BackMsg backMsg = new BackMsg();
        //    backMsg.Code = 0;
        //    try
        //    {
        //        Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
        //        string Data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
        //        object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodsEdit"].ToString(), parameters, 1, model);
        //        messData = JsonConvert.DeserializeObject<List<BackMsg>>(data.ToString());
        //    }
        //    catch (Exception ex)
        //    {
                
        //        backMsg.Code = 2001;
        //        backMsg.Mess = ex.Message.ToString();
        //    }
        //    foreach (BackMsg msg in messData)
        //    {
        //        backMsg = msg;
        //        if (msg.Code != 0)
        //        {
        //            break;
        //        }
        //    }
        //    return backMsg;
        //}

        public static BackMsg List(List<RequstGoodDetail> model)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                string Data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodsDetail"].ToString(), parameters, 1, model);
                backMsg = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            return backMsg;
        }


        public static BackMsg Edit(List<RequstGoodEditModel> BodyList)
        {
            BackMsg msg = new BackMsg();
            msg.Code = 0;
            List<BackMsg> results = new BackMsg[BodyList.Count].ToList();
            FormMain main = InitUser.MainForm as FormMain;
            try
            {
                Parallel.For(0, BodyList.Count, i => results[i]= PostGoodEdit(BodyList[i]));
                int cnt = 0;
                foreach (var item in results)
                {
                    if (item is BackMsg)
                    {
                        if (item.Code != 0)
                        {
                            main.LogEvent(item.Mess);
                            cnt++;
                        }
                    }
                }
                msg.Mess = $"修改完成：商品总数{results.Count},成功{results.Count - cnt}个,失败{cnt}个！\n失败原因请查看操作日志栏目";

                main.LogEvent($"修改完成：商品总数{results.Count},成功{results.Count - cnt}个,失败{cnt}个！");
            }
            catch (Exception ex)
            {
                msg.Code = 200;
                msg.Mess = ex.Message;
                main.LogEvent(ex.Message);
            }
            
            return msg;
        }

        public static BackMsg PostGoodEdit(RequstGoodEditModel item)
        {
            BackMsg backMsg = new BackMsg();
            Goods_detailModel detaiModel = getmodel(item.goods_id, item.Malls.mall_token);

            switch (item.ApiType)
            {
                case 0:
                    detaiModel.goods_name = item.goods_name;
                    break;
                case 1:
                    detaiModel.tiny_name = item.tiny_name;
                    break;
                case 2:
                    if (item.shipment_limit_second == 0)
                    {
                        detaiModel.shipment_limit_second = 86400;
                        detaiModel.delivery_one_day = 1;
                    }
                    else
                        detaiModel.shipment_limit_second = item.shipment_limit_second;
                    break;
                case 3:
                    detaiModel.sku_list = item.sku_list;
                    break;
                case 4:
                    detaiModel.cat_id = item.cat_id;
                    break;
                case 5:
                    detaiModel.cost_template_id = item.cost_template_id;
                    break;
                case 6:
                    detaiModel.two_pieces_discount = item.two_pieces_discount;
                    break;
                case 7:
                    detaiModel.buy_limit = item.buy_limit;
                    break;
                case 8:
                    detaiModel.order_limit = item.order_limit;
                    break;
                case 9:
                    detaiModel.is_folt = item.is_folt;
                    break;
                case 10:
                    detaiModel.bad_fruit_claim = item.bad_fruit_claim;
                    break;
                case 11:
                    detaiModel.goods_desc = item.goods_desc;
                    break;
                case 12:// 商品轮播图
                    detaiModel.carousel_gallery_list = item.carousel_gallery;
                    break;
                case 13:// 商品详情图
                    detaiModel.detail_gallery_list = item.detail_gallery;
                    break;
                case 14:// 
                    detaiModel.sku_list = item.sku_list;
                    detaiModel.outer_goods_id = item.outer_goods_id;
                    break;
                case 15:
                    detaiModel.goods_property_list=item.goods_property_list;
                    break;
                case 16://非预售
                    detaiModel.is_pre_sale = 0;
                    detaiModel.is_group_pre_sale = 0;
                    detaiModel.pre_sale_time = 0;
                    detaiModel.is_sku_pre_sale = 0;
                    if (detaiModel.shipment_limit_second > 172800)
                        detaiModel.shipment_limit_second = 172800;
                    break;
                case 17:
                    detaiModel.is_pre_sale=1;
                    detaiModel.is_sku_pre_sale = 0;
                    detaiModel.pre_sale_time = item.pre_sale_time;
                    detaiModel.is_group_pre_sale = 0;
                    break;
                case 18:
                    detaiModel.is_pre_sale = 0;
                    detaiModel.is_sku_pre_sale = 0;
                    detaiModel.shipment_limit_second = item.shipment_limit_second;
                    detaiModel.pre_sale_time = 0;
                    detaiModel.is_group_pre_sale = 1;
                    break;
                case 19://规格预售
                    detaiModel.is_pre_sale = 0;
                    detaiModel.pre_sale_time = 0;
                    detaiModel.is_group_pre_sale = 0;
                    detaiModel.is_sku_pre_sale = 1;
                    detaiModel.sku_list = item.sku_list;

                    if (detaiModel.shipment_limit_second > 172800)
                        detaiModel.shipment_limit_second = 172800;
                    break;
                default:
                    break;
            }

            //数据正常请求接口
            Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.information.update" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", item.Malls.mall_token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                    //{ "goods_id", item.goods_id.ToString() },

                };

            parameters.Add("carousel_gallery", JsonConvert.SerializeObject(detaiModel.carousel_gallery_list).ToString());
            parameters.Add("detail_gallery", JsonConvert.SerializeObject(detaiModel.detail_gallery_list).ToString());
            if (item.ApiType == 15)
            {
                parameters.Add("goods_properties", JsonConvert.SerializeObject(detaiModel.goods_property_list).ToString());
            }
            foreach (PropertyInfo property in detaiModel.GetType().GetProperties())
            {
                object value = property.GetValue(detaiModel);

                List<string> Names = new List<string>() { "oversea_goods", "goods_travel_attr", "goods_trade_attr", "goods_property_list", "elec_goods_attributes", "carousel_video", "carousel_gallery_list", "detail_gallery_list" };
                if (Names.Contains(property.Name))
                    continue;

                if (property.Name == "sku_list")
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    foreach (var sku in detaiModel.sku_list)
                    {
                        sku.quantity = 0;
                        List<long> specIds = new List<long>();
                        foreach (var spe in sku.spec)
                            specIds.Add(spe.spec_id);
                        sku.spec_id_list = JsonConvert.SerializeObject(specIds.ToArray()).ToString();
                    }

                    value = JsonConvert.SerializeObject(detaiModel.sku_list, settings);
                }

                
                if (value != null && value.ToString() != "") // 检查值是否为null  
                {
                    //某些参数类型变化
                    List<string> TypeChanged = new List<string>() { "is_customs", "is_folt", "is_pre_sale", "is_refundable", "second_hand" };
                    if (TypeChanged.Contains(property.Name))
                        parameters.Add(property.Name, value.ToString() == "1" ? "true" : "false");
                    else
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
                backMsg.Code = 101;
                try
                {
                    if (!string.IsNullOrEmpty(jToken["error_response"]["error_msg"].ToString()))
                    {
                        backMsg.Mess = "商品【" + detaiModel.goods_name + "】修改失败！错误信息：" + jToken["error_response"]["error_msg"].ToString();
                    }
                    else if (!string.IsNullOrEmpty(jToken["error_response"]["sub_msg"].ToString()))
                    {
                        backMsg.Mess = "商品【" + detaiModel.goods_name + "】修改失败！错误信息：" + jToken["error_response"]["sub_msg"].ToString();
                    }
                    else
                    {
                        backMsg.Mess = "商品【" + detaiModel.goods_name + "】修改失败！错误信息：" + jToken["error_response"].ToString();
                    }
                }
                catch 
                {
                    backMsg.Mess = "商品【" + detaiModel.goods_name + "】修改失败！错误信息：" + jToken["error_response"].ToString();
                }
            }
            else
            {
                backMsg.Code = 0;
                backMsg.Mess = jToken.ToString();
            }

            //修改成功后提交溯源接口

            if (backMsg.Code == 0)
            {
                //detaiModel.access_token = item.Malls.mall_token;
                //string backMsg1 = apiHelp.postApi("http://112.124.0.204:2356/jky/uploadTraceability", new Dictionary<string, string>(), 1, detaiModel);
                ////apiHelp.setError("溯源失败:" + backMsg1+JsonConvert.SerializeObject(detaiModel));
                //JToken jToken1 = JsonConvert.DeserializeObject<JToken>(backMsg1);
                //if (jToken1["code"].ToString() != "200")
                //{
                //    backMsg.Code = MyConvert.ToInt(jToken1["code"].ToString());
                //    backMsg.Mess =  "溯源失败！商品【" + detaiModel.goods_name +"】错误信息："+ jToken1["msg"].ToString();
                //    //apiHelp.setError("溯源失败:" + backMsg1);
                //    Dictionary<string, string> parameter = new Dictionary<string, string>();
                //    parameter.Add("Error", "溯源失败:" + backMsg1);
                //    apiHelp.postApi(ConfigurationManager.AppSettings["SysError"].ToString(), parameter);
                //}
            }
            return backMsg;
        }

        public static Goods_detailModel getmodel(long goodsID, string access_token)
        {
            BackMsg backMsg=new BackMsg();
            Goods_detailModel model = new Goods_detailModel();
            //根据商品id获取商品明细
            Dictionary<string, string> parameters = new Dictionary<string, string>
                {
                    { "type", "pdd.goods.detail.get" },
                    { "client_id", apiHelp.client_id },
                    { "access_token", access_token },
                    { "timestamp", ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000/1000).ToString() },
                    { "data_type", "JSON" },
                    { "goods_id", goodsID.ToString() },

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
            if (backMsg.Code == 0)
            {
                JToken token = JsonConvert.DeserializeObject<JToken>(backMsg.Mess);
                var json = JsonConvert.SerializeObject(token["goods_detail_get_response"]);
                model = JsonConvert.DeserializeObject<Goods_detailModel>(json);
            }

            return model;
        }
    }
}
