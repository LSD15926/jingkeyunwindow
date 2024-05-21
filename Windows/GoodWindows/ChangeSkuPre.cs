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
using System.Diagnostics;
using jingkeyun.Class;
using System.Data.SQLite;
using CefSharp.WinForms;
using CefSharp;
using Newtonsoft.Json;
using CefSharp.DevTools.Network;

namespace jingkeyun.Windows
{
    public partial class ChangeSkuPre : UIForm
    {

        public List<Goods_detailModel> goods_DetailModels= new List<Goods_detailModel>();

        private List<GoodListResponse> _GoodsModel = new List<GoodListResponse>();

        public List<GoodListResponse> GoodsModel
        {
            get
            {
                return _GoodsModel;
            }
            set
            {
                _GoodsModel = value;
                //渲染页面
                int cnt = 0;
                foreach (var item in _GoodsModel)
                {
                    cnt += item.sku_list.Count;
                    jsSkuPreModel model= new jsSkuPreModel();
                    model.thumb_url = item.thumb_url;
                    model.goods_id= item.goods_id;
                    model.goods_name = item.goods_name;
                    model.Mallinfo= item.Mallinfo;
                    for (int i = 0; i < item.sku_list.Count; i++)
                    {
                        jsPre_skuList sku=new jsPre_skuList();
                        sku.spec = item.sku_list[i].spec;
                        sku.sku_id= item.sku_list[i].sku_id;
                        try
                        {
                            var d_model = goods_DetailModels.Find(x => x.goods_id == model.goods_id);
                            var d_sku = d_model.sku_list.Find(x => x.sku_id == item.sku_list[i].sku_id);
                            sku.thumb_url = d_sku.thumb_url;
                            sku.sku_pre_sale_time=d_sku.sku_pre_sale_time==0?"": d_sku.sku_pre_sale_time.ToString()+"000";
                        }
                        catch
                        {
                            continue;
                        }
                        model.sku_list.Add(sku);
                    }
                    jsSkuPreModels.Add(model);
                 }
                txtNum.Text = $"商品数:{_GoodsModel.Count}   SKU数:{cnt}";
            }
        }
        public List<jsSkuPreModel> jsSkuPreModels = new List<jsSkuPreModel>(); 
        public ChangeSkuPre()
        {
            InitializeComponent();
            InitMyStyle();
            DsTime.MinDate = DateTime.Now.AddDays(3);
            DsTime.MaxDate = DateTime.Now.AddDays(30);
            DsTime.Text = "";


        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;

            panel2.BackColor = this.TitleColor;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
            StyleHelper.SetButtonColor(uiButton3, StyleHelper.OkButton);

            StyleHelper.SetButtonColor(uiButton4, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton5, StyleHelper.OkButton);

        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (DsTime.Text == "")
                return;
            jsObject.editQuantity(Chrome, DsTime.Text,0);

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
                    try
                    {
                        var result = t.Result; // 这里的Result就是JavaScript函数返回的值

                        var json = JsonConvert.SerializeObject(result.Result);
                        List<jsSkuPreModel> AllItem = JsonConvert.DeserializeObject<List<jsSkuPreModel>>(json);

                        List<RequstGoodEditModel> models=new List<RequstGoodEditModel>();

                        foreach (var item in AllItem)
                        {
                            Goods_detailModel g_model = goods_DetailModels.Find(x => x.goods_id == item.goods_id);
                            foreach (var item2 in item.sku_list)
                            {
                                var sku_item= g_model.sku_list.Find(x=>x.sku_id == item2.sku_id);
                                long val = 0;
                                if (item2.sku_pre_sale_time.Length > 10)
                                {
                                    val= MyConvert.ToTimeStamp(MyConvert.StampToTime(MyConvert.ToLong(item2.sku_pre_sale_time.Substring(0, 10))).ToString("yyyy-MM-dd 23:59:59"));
                                }
                                if (sku_item.sku_pre_sale_time==val)
                                    continue;
                                sku_item.sku_pre_sale_time = val;
                            }
                            RequstGoodEditModel model = new RequstGoodEditModel();
                            model.ApiType = (int)GoodsEdit.商品预售_4;
                            model.goods_id = item.goods_id;
                            model.Malls = item.Mallinfo;
                            model.sku_list=g_model.sku_list;
                            models.Add(model);
                        }

                        stampNow = MyConvert.ToTimeStamp(DateTime.Now);
                        InitUser.RunningTask.Add("修改规格预售" + stampNow, stampNow.ToString());
                        UIMessageTip.ShowOk("已提交至后台处理");
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += Worker_DoWork;
                        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                        worker.RunWorkerAsync(models);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowError("提交出错！"+ex.Message);
                    }
                    
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        BackMsg RetMsg;
        private long stampNow;
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitUser.RunningTask.Remove("修改规格预售" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改规格预售");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
        }

        private void ChangeQuantity_Load(object sender, EventArgs e)
        {
            InitBrowser();
        }
        ChromiumWebBrowser Chrome;
        JsObject_SkuPre jsObject = new JsObject_SkuPre();
        public void InitBrowser()
        {
            Chrome = new ChromiumWebBrowser(InitUser.pageUrl + "jingkeyun/skuPreSale.html");
            Chrome.MenuHandler = new MenuHandler();
            Chrome.KeyboardHandler = new CEFKeyBoardHander();
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled };
            Chrome.Dock = DockStyle.Fill;
            CefSharpSettings.WcfEnabled = true;
            Chrome.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            jsObject.f=this;
            Chrome.JavascriptObjectRepository.Register("boundAsync", jsObject, true, BindingOptions.DefaultBinder);
            this.panel3.Controls.Add(Chrome);
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            if (DsTime.Text == "")
                return;
            jsObject.editQuantity(Chrome, DsTime.Text, 1);
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            if (DsTime.Text == "")
                return;
            jsObject.editQuantity(Chrome, DsTime.Text, 2);
        }
    }
}
