using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;

namespace jingkeyun.Data
{
    public class Auth_Token
    {
        public static BackMsg Get(string code)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("code", code);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["AuthToken"].ToString(), parameters);
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
