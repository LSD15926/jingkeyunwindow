using CefSharp;
using jingkeyun.Class;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace jingkeyun.Windows
{
    public class JsObject_SkuName
    {
        public ChangeSkuName f;
        public string getData()
        {
            try {
                return JsonConvert.SerializeObject(f.jsSkuNameModels);
            }
            catch {
                return "[]";
            }
        }
        public void editQuantity(CefSharp.WinForms.ChromiumWebBrowser chrome, string value1,string value2,int type)
        {
            try
            {
                if (chrome == null)
                    return;
                chrome.ExecuteScriptAsync($"editData('{value1}','{value2}',{type})");
            }
            catch
            {
                MyMessageBox.ShowError("出现异常！请稍后再试！");
            }
        }

    }

    public class jsSkuNameModel
    {
        public string thumb_url { get; set; } = "";
        public string goods_name { get; set; } = "";
        public long goods_id { get; set; } = 0;
        public Mallinfo Mallinfo { get; set; }

        public List<jsSkuName_spec> sku_specs { get; set;}=new List<jsSkuName_spec>();
    }
    public class jsSkuName_spec
    {
        public string spec { get; set; }

        public long spec_id { get; set; }

        public List<jsSkuName_spec_list> SpecList { get; set; } = new List<jsSkuName_spec_list> { };
    }

    public class jsSkuName_spec_list
    {
        public string name { get; set; } = "";
        public long id {  get; set; } = 0;
    }

}