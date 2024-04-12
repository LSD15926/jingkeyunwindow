using Newtonsoft.Json;
using System.IO;

namespace jingkeyun.Class
{
    internal class SetListConfig
    {
        static string path = Path.Combine(Directory.GetCurrentDirectory(),"account.ini");
        public static void SetConfig(string Name,string value)
        {
            object OutData = value;
            string Data = Newtonsoft.Json.JsonConvert.SerializeObject(OutData);
            string strJson = JsonConvert.SerializeObject(OutData);
            Config.OperateIniFile.WriteIniData("token", Name, Config.Secret.加密(Data), path);
        }
        public static string GetConfig(string Name)
        {
            string ConfigData = "";

            ConfigData = Config.OperateIniFile.ReadIniData("token", Name, "", path);
            if (ConfigData == "")
            {
                return ConfigData;
            }
            string ConfigDatajm = Config.Secret.解密(ConfigData).TrimStart('"').TrimEnd('"');

            return ConfigDatajm;

        }
    }
}

