using CefSharp;
using jingkeyun.Class;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace jingkeyun.Windows
{
    public class JsObject_SkuPre
    {
        public ChangeSkuPre f;
        public string getData()
        {
            try {
                return JsonConvert.SerializeObject(f.jsSkuPreModels);
            }
            catch {
                return "[]";
            }
        }
        public void editQuantity(CefSharp.WinForms.ChromiumWebBrowser chrome, string value,int type)
        {
            try
            {
                if (chrome == null)
                    return;

                string stamp = "";
                if(value!="")
                    stamp= MyConvert.ToFullTimeStamp(value).ToString();
                chrome.ExecuteScriptAsync($"editData({stamp},{type})");
            }
            catch
            {
                MyMessageBox.ShowError("出现异常！请稍后再试！");
            }
        }

    }

    public class jsSkuPreModel
    {
        public string thumb_url { get; set; } = "";
        public string goods_name { get; set; } = "";

        public long goods_id { get; set; } = 0;

        public List<jsPre_skuList> sku_list = new List<jsPre_skuList>();

        public Mallinfo Mallinfo { get; set; }
    }

    public class jsPre_skuList
    {
        public string sku_pre_sale_time { get; set; } = "";

        public string spec { get; set; } = "";

        public string thumb_url { get; set; } = "";

        public long sku_id { get; set; } = 0;
    }
}