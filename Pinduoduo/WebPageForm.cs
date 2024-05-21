using CefSharp;
using CefSharp.WinForms;
using jingkeyun.Class;
using Sunny.UI;
using System.Management.Instrumentation;
using System.Security.Policy;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace jingkeyun.Pinduoduo
{
    public partial class WebPageForm : UIPage
    {
        public WebPageForm()
        {
            InitializeComponent();
            //this.TitleColor = StyleHelper.Title;
            InitBrowser();
        }
        private string _WebUrl = "https://mms.pinduoduo.com/login/";
        public string WebUrl
        { 
            get { return _WebUrl; }
            set { _WebUrl = value;
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
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled};
            this.panel1.Controls.Add(Chrome);
            Chrome.Dock = DockStyle.Fill;
        }

        private void btnSelect_Click(object sender, System.EventArgs e)
        {
            WebUrl=textBox1.Text;
        }

        private void uiSymbolButton1_Click(object sender, System.EventArgs e)
        {
            WebUrl= "https://mms.pinduoduo.com/login/";
        }

        private void uiSymbolLabel1_Click(object sender, System.EventArgs e)
        {
            if(Chrome.CanGoBack)
                Chrome.Back();
        }

        private void uiSymbolLabel2_Click(object sender, System.EventArgs e)
        {
            if(Chrome.CanGoForward)
                Chrome.Forward();
        }

        private void uiSymbolLabel3_Click(object sender, System.EventArgs e)
        {
            Chrome.Reload();
        }


        public void BefClose()
        {
            Chrome.CloseDevTools();//关闭浏览器调试工具
            //此方法会同时关闭窗口，我们真只可以直接调用该方法，同时关闭浏览器及窗口
            Chrome.GetBrowser().CloseBrowser(true);//关闭浏览器
        }
    }
}
