using CefSharp;
using CefSharp.WinForms;
using jingkeyun.Class;
using Newtonsoft.Json;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace jingkeyun.Pinduoduo
{
    public class JsObject_Mail
    {
        public mailList form;

        public void getSelection(List<MallTabel> goods)
        {
            form.getCheck(goods);
        }
        public void removeMall(MallTabel tabel)
        {
            form.removeMall(tabel);
        }
        public void showOnPage()
        {
            form.showOnPage();
        }
        public void goMall(MallTabel tabel)
        {
            form.Invoke(new MethodInvoker(delegate
            {
                CefTabPageHelper.showPage("https://mms.pinduoduo.com/home/", "店铺后台");
            }));
        }
        /// <summary>
        /// 渲染数据
        /// </summary>
        /// <param name="chrome"></param>
        /// <param name="mallinfo"></param>
        public void setData(ChromiumWebBrowser chrome, List<Mallinfo> mallinfo)
        {
            if (chrome == null)
                return;
            List<MallTabel> mallTabels = new List<MallTabel>();
            foreach (Mallinfo item in mallinfo)
            {
                MallTabel tabel = new MallTabel();
                tabel.logo= item.logo;
                tabel.id = item.id;
                tabel.mall_id = item.mall_id;
                tabel.mall_name = item.mall_name;
                tabel.mall_token_expire=MyConvert.StampToTime(item.mall_token_expire);
                tabel.mall_type = tabel.mall_token_expire > DateTime.Now ? "已授权" : "授权过期";
                mallTabels.Add(tabel);
            }

            chrome.ExecuteScriptAsync($"setData({JsonConvert.SerializeObject(mallTabels)})");
        }
    }

    public class MallTabel
    {
        public string logo { get; set; } = "";

        public int id { get; set; }=0;

        public string mall_name { get; set; } = "";

        public long mall_id {  get; set; } = 0; 
        public DateTime mall_token_expire {  get; set; } = DateTime.MinValue;

        public string mall_type { get; set; } = "";
        
    }
}