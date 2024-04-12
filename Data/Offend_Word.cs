using Newtonsoft.Json;
using Pdd_Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jingkeyun;

namespace jingkeyun.Data
{
    public class Offend_Word
    {
        public static BackData get()
        {
            BackData backData = new BackData();
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>(); //参数列表
                object data = apiHelp.postApi(ConfigurationManager.AppSettings["OffendList"].ToString(), parameters);
                backData = JsonConvert.DeserializeObject<BackData>(data.ToString());
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
