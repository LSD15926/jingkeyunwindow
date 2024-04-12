using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;

namespace jingkeyun.Data
{
    public class Good_Spec
    {
        public static BackMsg Get(string Parent_Spec_Id, string Spec_Name ,string access_token)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("Parent_Spec_Id", Parent_Spec_Id.ToString());
                parameters.Add("Spec_Name", Spec_Name.ToString());
                parameters.Add("access_token", access_token.ToString());
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodSpec"].ToString(), parameters);
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
