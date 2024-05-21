using jingkeyun;
using Newtonsoft.Json;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APIOffice.Controllers.pddApi
{
    public class PinduoduoSignHelper
    {

        /// <summary>
        /// 生成sign
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GenerateSign(Dictionary<string, string> parameters)
        {
            try
            {
                var json = JsonConvert.SerializeObject(parameters);
                string data = apiHelp.postApi(ConfigurationManager.AppSettings["signGet"].ToString(), parameters, 1, json);
                return data;
            }
            catch {
                return "";
            }   
        }
    
    }
}
