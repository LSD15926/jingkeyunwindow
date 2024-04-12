using DevComponents.AdvTree;
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
    public class Sys_User
    {
        public static BackData Login(string Name, string Psw)
        {
            BackData backMsg = new BackData();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("Name", Name);
                parameters.Add("Psw", Psw);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["UserLogin"].ToString(), parameters);
                backMsg = JsonConvert.DeserializeObject<BackData>(data.ToString());
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }
            
            return backMsg;
        }

        public static BackMsg Sign(LoginUser login)
        {
            BackMsg backMsg = new BackMsg();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表

                object data = apiHelp.postApi(ConfigurationManager.AppSettings["UserSign"].ToString(), parameters,1,login);
                backMsg = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                backMsg.Code = 2001;
                backMsg.Mess = ex.Message.ToString();
            }

            return backMsg;
        }

        public static BackData BSPHP_Login(LoginUser login)
        {
            BackData backMsg = new BackData();
            backMsg.Code = 0;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["BSPHPLogin"].ToString(), parameters,1, login);
                backMsg = JsonConvert.DeserializeObject<BackData>(data.ToString());
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
