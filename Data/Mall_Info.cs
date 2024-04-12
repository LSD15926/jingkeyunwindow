using DevComponents.AdvTree;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jingkeyun;

namespace jingkeyun.Data
{
    public class Mall_Info
    {
        /// <summary>
        /// 根据用户id获取名下店铺
        /// </summary>
        /// <returns></returns>
        public static BackData get(int UserId)
        {
            BackData backData = new BackData();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("UserId", UserId.ToString());
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["MallList"].ToString(), parameters);
                backData = JsonConvert.DeserializeObject<BackData>(data.ToString());
            }
            catch (Exception ex)
            {
                backData.Code = 2001;
                backData.Mess = ex.Message.ToString();
            }
            return backData;
        }

        public static BackMsg del(string ids)
        {
            BackMsg backData = new BackMsg();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("ids", ids);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["MallDel"].ToString(), parameters);
                backData = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                backData.Code = 2001;
                backData.Mess = ex.Message.ToString();
            }
            return backData;
        }

        public static BackMsg Upd(Mallinfo model)
        {
            BackMsg backData = new BackMsg();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["MallUpd"].ToString(), parameters,1,model);
                backData = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                backData.Code = 2001;
                backData.Mess = ex.Message.ToString();
            }
            return backData;
        }

        public static BackMsg Info(string Token)
        {
            BackMsg backData = new BackMsg();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                parameters.Add("Token", Token);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["MallInfo"].ToString(), parameters);
                backData = JsonConvert.DeserializeObject<BackMsg>(data.ToString());
            }
            catch (Exception ex)
            {
                backData.Code = 2001;
                backData.Mess = ex.Message.ToString();
            }
            return backData;
        }
    }
}
