using CefSharp;
using jingkeyun.Pinduoduo;
using jingkeyun.Windows;
using Newtonsoft.Json;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Class
{
    public class JsObject_Quantity
    {
        public ChangeQuantity f;
        public string getData()
        {
            return JsonConvert.SerializeObject(f.goodSkuQuantities);
        }
        public void editQuantity(CefSharp.WinForms.ChromiumWebBrowser chrome,int value,int type)
        {
            try
            {
                if (chrome == null)
                    return;
                chrome.ExecuteScriptAsync($"editData({value},{type})");
            }
            catch
            {
                MyMessageBox.ShowError("出现异常！请稍后再试！");
            }
        }

        public void setData(List<jsQuanModel> detailModels, CefSharp.WinForms.ChromiumWebBrowser chrome)
        {
            try
            {
                if (chrome == null)
                    return;
                chrome.ExecuteScriptAsync($"setData({JsonConvert.SerializeObject(detailModels)})");
            }
            catch 
            {
                MyMessageBox.ShowError("出现异常！请稍后再试！");
            }
            
        }

    }

    public class jsQuanModel
    {
        public string thumb_url { get; set; } = "";
        public string goods_name { get; set; } = "";

        public long goods_id {  get; set; }=0;

        public List<jsQuanSkuModel> sku_list=new List<jsQuanSkuModel>();

        public Mallinfo Mallinfo { get; set; }
    }

    public class jsQuanSkuModel
    {
        public long sku_quantity { get; set; } = 0;

        public string spec { get; set; } = "";

        public string thumb_url { get; set; } = "";

        public long sku_id {  get; set; } = 0;  
    }
}
