using APIOffice.Controllers.pddApi;
using jingkeyun.Class;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows.Forms;
using jingkeyun.Controls;
using jingkeyun.Pinduoduo;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Threading;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using jingkeyun;
using System.Drawing.Text;
using System.Drawing;
using System.Data.SqlClient;
using jingkeyun.Windows;
using static System.Windows.Forms.ListViewItem;
using DevComponents.DotNetBar.Controls;
using System.Data;
using System.Diagnostics;
using Pdd_Models;
using System.Linq;
using System.Text;
using System.Net;
using CefSharp;
using CefSharp.WinForms;

namespace jingkeyun
{
    public partial class Form3 : Form
    {
        public Form3()
        {

            InitializeComponent();
            InitBrowser();
        }
        private string _WebUrl = "http://127.0.0.1:8848/jingkeyun/goodTable.html";
        public string WebUrl
        {
            get { return _WebUrl; }
            set
            {
                _WebUrl = value;
                Chrome.LoadUrl(value);
            }
        }
        ChromiumWebBrowser Chrome;

        public void InitBrowser()
        {

            Chrome = new ChromiumWebBrowser(WebUrl);

            // 在这里重新赋值，重写了 LifeSpanHandler ，阻止打开新的窗口
            Chrome.LifeSpanHandler = new CefLifeSpanHandler();
            Chrome.MenuHandler = new MenuHandler();
            Chrome.KeyboardHandler = new CEFKeyBoardHander();
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled };
            this.Controls.Add(Chrome);
            Chrome.JavascriptObjectRepository.Register("boundAsync", new JsObject_Attrs(), true, BindingOptions.DefaultBinder);
            Chrome.Dock = DockStyle.Fill;
        }

        private void MobilePageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Chrome.CloseDevTools();//关闭浏览器调试工具
            //此方法会同时关闭窗口，我们真只可以直接调用该方法，同时关闭浏览器及窗口
            Chrome.GetBrowser().CloseBrowser(true);//关闭浏览器
            //释放浏览器对象
            if (Chrome != null && !Chrome.Disposing)
            {
                Chrome.Dispose();
            }
        }


    }
}
