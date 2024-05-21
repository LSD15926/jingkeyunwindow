using CefSharp;
using jingkeyun.Pinduoduo;
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
    public class JsObject_Good
    {

        public goodsList form;

        /// <summary>
        /// 跳转商品详情页
        /// </summary>
        /// <param name="goods_id"></param>
        public void Redirection(string goods_id)
        {

            form.Invoke(new MethodInvoker(delegate
            {
                CefTabPageHelper.showPage("https://mms.pinduoduo.com/goods/goods_detail?goods_id=" + goods_id,"商品详情");
            }));
            //InitUser.MainForm.F3.WebUrl = "https://mms.pinduoduo.com/goods/goods_detail?goods_id=" + goods_id;
            //InitUser.MainForm.uiSymbolLabel1_Click(InitUser.MainForm.uiSymbolLabel7, null);
        }

        /// <summary>
        /// 显示右键菜单
        /// </summary>
        public void showMenuStrip(GoodListResponse SelectModel)
        {
            form.showMenu(SelectModel);
        }

        public void getSelection(List<GoodListResponse> goods)
        {
            form.getSelect(goods);
        }

        public void setSelectiom(int begin, int end, CefSharp.WinForms.ChromiumWebBrowser chrome)
        {
            if(chrome==null)
                return;
            chrome.ExecuteScriptAsync($"setSelection({begin},{end})");
        }

        public void setData(List<GoodListResponse> detailModels, CefSharp.WinForms.ChromiumWebBrowser chrome)
        {
            if (chrome == null)
                return;
            chrome.ExecuteScriptAsync($"setData({JsonConvert.SerializeObject(detailModels)})");
        }

        public void copyMsg(string msg)
        {
            form.Invoke(new MethodInvoker(delegate
            {
                Clipboard.SetText(msg);
            }));
            
        }

        public void saleChange(GoodListResponse SelectModel)
        {
            form.DoSale(SelectModel);
        }

        public void openForm(string url)
        {
            form.Invoke(new MethodInvoker(delegate
            {
                CefTabPageHelper.showMobilePage(url);
            }));
        }

        public string getQrcode(string id)
        {
            try
            {
                string url = "https://mobile.yangkeduo.com/goods1.html?goods_id=" + id;
                // 使用MemoryStream来保存Bitmap对象  
                using (MemoryStream memory = new MemoryStream())
                {
                    Bitmap bitmap = QrCodeHelper.getCode(url);
                    // 将Bitmap对象保存到MemoryStream中  
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png); // 或者使用ImageFormat.Jpeg等  
                    memory.Position = 0; // 重置MemoryStream的位置  

                    // 读取MemoryStream中的数据并转换为Base64字符串  
                    byte[] byteImage = memory.ToArray();
                    string base64String = Convert.ToBase64String(byteImage);

                    return base64String;
                }

            }
            catch (Exception ex)
            {
                return "";
            }
            
        }
    }
}
