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
    public partial class EditAttr : UIForm
    {

        public Goods_detailModel DetailModel { get; set; }

        private GoodListResponse _GoodsModel = new GoodListResponse();

        public GoodListResponse GoodsModel
        {
            get
            {
                return _GoodsModel;
            }
            set
            {
                _GoodsModel = value;
            }
        }
        public EditAttr()
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
        JsObject_Attr jsObject = new JsObject_Attr();
        public void InitBrowser()
        {
            Chrome = new ChromiumWebBrowser(InitUser.pageUrl + "jingkeyun/goodAttr.html");
            Chrome.MenuHandler = new MenuHandler();
            Chrome.KeyboardHandler = new CEFKeyBoardHander();
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled };
            Chrome.Dock = DockStyle.Fill;
            CefSharpSettings.WcfEnabled = true;
            Chrome.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            jsObject._Main = DetailModel;
            
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
            MyMessageBox.ShowLoading(this);
            var task = Chrome.EvaluateScriptAsync("postBack()");
            task.ContinueWith(t => {
                if (!t.IsFaulted)
                {
                    var result = t.Result; // 这里的Result就是JavaScript函数返回的值
                                           // 处理返回结果
                    
                    var json=JsonConvert.SerializeObject(result.Result);
                    
                    List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
                    RequstGoodEditModel model = new RequstGoodEditModel();
                    model.ApiType = (int)GoodsEdit.商品属性;
                    model.goods_property_list = JsonConvert.DeserializeObject<List<Goods_property_listItem>>(json);
                    model.goods_id= DetailModel.goods_id;
                    model.Malls = DetailModel.mall;
                    models.Add(model);
                    BackMsg RetMsg = Good_Edit.Edit(models);
                    MyMessageBox.IsShowLoading = false;
                    if (RetMsg.Code == 0)
                    {
                        MyMessageBox.ShowSuccess("修改成功!");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MyMessageBox.ShowError(RetMsg.Mess);
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
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
