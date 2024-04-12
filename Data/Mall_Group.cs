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
    public class Mall_Group
    {
        public static BackData List(int user)
        {
            BackData backMsg = new BackData();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("UserId", user.ToString());
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["MallGroupList"].ToString(), parameters);
                backMsg = JsonConvert.DeserializeObject<BackData>(data.ToString());
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            
            return backMsg;
        }
        public static BackMsg Add(MallGroup model)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表

                object data = apiHelp.postApi(ConfigurationManager.AppSettings["MallGroupAdd"].ToString(), parameters,1,model);
                backMsg = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }

            return backMsg;
        }

        public static BackMsg Edit(MallGroup model)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["MallGroupEdit"].ToString(), parameters, 1, model);
                backMsg = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }

            return backMsg;
        }

        public static BackMsg Del(MallGroup model)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["MallGroupDel"].ToString(), parameters,1,model);
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
