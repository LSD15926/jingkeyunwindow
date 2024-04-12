using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodsCat"].ToString(), parameters,1,requstCat);
                backMsg = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
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
