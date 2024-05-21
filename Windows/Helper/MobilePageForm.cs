using CefSharp.WinForms;
using CefSharp;
using jingkeyun.Class;
using jingkeyun.Pinduoduo;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Windows.Helper
{
    public partial class MobilePageForm : UIForm
    {
        public MobilePageForm()
        {
            InitializeComponent();
            InitBrowser();
        }
        private string _WebUrl = "https://mms.pinduoduo.com/login/";
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
            Chrome.KeyboardHandler=new CEFKeyBoardHander();
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled };
            this.Controls.Add(Chrome);
            Chrome.Dock = DockStyle.Fill;
        }

        private void MobilePageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Chrome.CloseDevTools();//关闭浏览器调试工具
            //此方法会同时关闭窗口，我们真只可以直接调用该方法，同时关闭浏览器及窗口
            Chrome.GetBrowser().CloseBrowser(true);//关闭浏览器
            ////释放浏览器对象
            //if (Chrome != null && !Chrome.Disposing)
            //{
            //    try
            //    {
            //    Chrome.Dispose();

            //    }
            //    catch (Exception ex) {
            //        var a = ex.Message;
            //    }
            //}
        }
    }
}
