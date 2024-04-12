using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace jingkeyun.Data
{
    public class Good_List
    {
        public static BackData Get(requestGoodList model)
        {
            BackData messData = new BackData();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                string Data = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["GoodsList"].ToString(), parameters, 1, model);
                messData = JsonConvert.DeserializeObject<BackData>(data.ToString());
            }
            catch (Exception ex)
            {
                messData.Code = 2001;
                messData.Mess = ex.Message.ToString();
            }
            return messData;

        }
       
    }
}
