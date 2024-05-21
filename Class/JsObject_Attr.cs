using CefSharp;
using jingkeyun.Data;
using jingkeyun.Pinduoduo;
using jingkeyun.Windows;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace jingkeyun.Class
{
    public class JsObject_Attr
    {
        public Goods_detailModel _Main;

        public string getPageData()
        {
            //获取规则
            BackMsg backMsg = Good_Cat.getCatRule(_Main);
            if (backMsg.Code == 0)
            {
                return backMsg.Mess;
            }
            return "[]";
        }
        public string getChoose()
        {
            return JsonConvert.SerializeObject(_Main.goods_property_list);
        }


    }


    public class JsObject_Attrs
    {
        public List<Goods_detailModel> _Main;
        public ChangeAttr _Change;
        public string getPageData()
        {
            //获取规则
            string jsonData = "[";
            List<BackMsg> backMsgs = new BackMsg[_Main.Count].ToList();
            Parallel.For(0, _Main.Count, i =>
            {
                BackMsg backMsg = Good_Cat.getCatRule(_Main[i]);
                backMsgs[i]=backMsg;
            });
            foreach(var item in backMsgs)
            {
                if (item.Code != 0)
                {
                    MyMessageBox.ShowError("获取发布规则失败！"+item.Mess);
                    InitUser.MainForm.LogEvent($"获取发布规则失败!" + item.Mess);
                    return "[]";

                }
                jsonData += item.Mess+",";
            }
            if(jsonData.EndsWith(","))
                jsonData=jsonData.Substring(0, jsonData.Length-1);
            return jsonData + "]";
        }

        public string getGoodModel()
        {
            return JsonConvert.SerializeObject(_Main);
        }

        public void showLoading()
        {
            MyMessageBox.ShowLoading(_Change);
        }
        public void hideLoading()
        {
            MyMessageBox.IsShowLoading = false;
        }
    }
}
