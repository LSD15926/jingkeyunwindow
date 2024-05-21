using APIOffice.pddApi;
using Pdd_Models;
using Pdd_Models.Models;
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
using jingkeyun.Controls;
using jingkeyun.Data;
using jingkeyun.Class;
using CefSharp.DevTools.Inspector;
using Sunny.UI.Win32;
using CefSharp.WinForms;
using CefSharp;
using Newtonsoft.Json;

namespace jingkeyun.Windows
{
    public partial class ChangeAttr : UIForm
    {

        public  List<Goods_detailModel> DetailModel { get; set; }
       
        public ChangeAttr()
        {
            InitializeComponent();
            InitMyStyle();
        }
        private void EditAttr_Load(object sender, EventArgs e)
        {
            //初始化列表页
            InitBrowser();
        }
        ChromiumWebBrowser Chrome;
        JsObject_Attrs jsObject = new JsObject_Attrs();
        public void InitBrowser()
        {
            Chrome = new ChromiumWebBrowser(InitUser.pageUrl+"jingkeyun/goodAttrs.html");
            Chrome.MenuHandler = new MenuHandler();
            Chrome.KeyboardHandler = new CEFKeyBoardHander();
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled };
            Chrome.Dock = DockStyle.Fill;
            CefSharpSettings.WcfEnabled = true;
            Chrome.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            jsObject._Change=this;
            jsObject._Main=DetailModel;
            Chrome.JavascriptObjectRepository.Register("boundAsync", jsObject, true, BindingOptions.DefaultBinder);
            this.panel1.Controls.Add(Chrome);
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;

            panel2.BackColor = this.TitleColor;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
        }


        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            var task = Chrome.EvaluateScriptAsync("postBack()");
            task.ContinueWith(t => {
                if (!t.IsFaulted)
                {
                    var result = t.Result; // 这里的Result就是JavaScript函数返回的值
                    
                    var json=JsonConvert.SerializeObject(result.Result);
                    List<List<Goods_property_listItem>> AllItem= JsonConvert.DeserializeObject<List<List<Goods_property_listItem>>>(json);
                    List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
                   for (int i = 0;i<DetailModel.Count;i++)
                    {
                        RequstGoodEditModel model = new RequstGoodEditModel();
                        model.ApiType = (int)GoodsEdit.商品属性;
                        model.goods_property_list = AllItem[i];
                        model.goods_id = DetailModel[i].goods_id;
                        model.Malls = DetailModel[i].mall;
                        models.Add(model);
                    }

                    InitUser.RunningTask.Add("商品属性" + stampNow, stampNow.ToString());
                    UIMessageTip.ShowOk("已提交至后台处理");
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += Worker_DoWork;
                    worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                    worker.RunWorkerAsync(models);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        
    BackMsg RetMsg;
    private long stampNow;
    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        InitUser.RunningTask.Remove("商品属性" + stampNow);
        MyMessageBox.showCheck(RetMsg.Mess, "修改商品属性");
    }

    private void Worker_DoWork(object sender, DoWorkEventArgs e)
    {
        List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
        RetMsg = Good_Edit.Edit(models);
    }
    private void EditAttr_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Chrome.CloseDevTools();
                Chrome.GetBrowser().CloseBrowser(true);
            }
            catch { }

            try
            {
                if (Chrome != null)
                {
                    
                }
            }
            catch { }
        }
    }
}
