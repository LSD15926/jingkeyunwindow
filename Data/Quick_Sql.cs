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
    public class Quick_Sql
    {
        public static BackTable List(string Sql)
        {
            BackTable backMsg = new BackTable();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("Sql", Sql);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["SqlTable"].ToString(), parameters);
                backMsg = JsonConvert.DeserializeObject<BackTable>(data.ToString());
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            
            return backMsg;
        }
        public static BackMsg upd(string Sql)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("Sql", Sql);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["SqlMsg"].ToString(), parameters);
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
