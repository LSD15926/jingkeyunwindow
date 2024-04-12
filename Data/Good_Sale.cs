using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
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
            foreach (BackMsg msg in messData)
            {
                backMsg = msg;
                if (msg.Code != 0)
                {
                    break;
                }
            }
            return backMsg;
        }
    }
}
