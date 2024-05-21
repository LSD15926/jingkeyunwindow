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

namespace jingkeyun.Windows
{
    public partial class ChangeQuantity : UIForm
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
                    jsQuanModel model = new jsQuanModel(); 
                    model.thumb_url = item.thumb_url;
                    model.goods_id=item.goods_id;
                    model.goods_name=item.goods_name;
                    model.Mallinfo=item.Mallinfo;
                    for (int i = 0; i < item.sku_list.Count; i++)
                    {
                        jsQuanSkuModel model2 = new jsQuanSkuModel();
                        model2.sku_quantity = item.sku_list[i].sku_quantity;
                        model2.spec= item.sku_list[i].spec;
                        model2.sku_id = item.sku_list[i].sku_id;
                        try
                        {
                            var d_model = goods_DetailModels.Find(x => x.goods_id == model.goods_id);
                            var d_sku = d_model.sku_list.Find(x => x.sku_id == item.sku_list[i].sku_id);
                            model2.thumb_url = d_sku.thumb_url;
                        }
                        catch
                        {
                            continue;
                        }

                        model.sku_list.Add(model2);
                        cnt++;
                    }
                    goodSkuQuantities.Add(model);
                }
                txtNum.Text = $"商品数:{_GoodsModel.Count}   SKU数:{cnt}";
            }
        }
        public List<jsQuanModel> goodSkuQuantities = new List<jsQuanModel>();
        public ChangeQuantity()
        {
            InitializeComponent();
            InitMyStyle();
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

            uiIntegerUpDown1.StyleCustomMode = true;
            uiIntegerUpDown1.Style = UIStyle.Purple;

            uiIntegerUpDown2.StyleCustomMode = true;
            uiIntegerUpDown2.Style = UIStyle.Purple;

            uiIntegerUpDown3.StyleCustomMode = true;
            uiIntegerUpDown3.Style = UIStyle.Purple;
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {

            jsObject.editQuantity(Chrome, uiIntegerUpDown1.Value, 0);
            uiIntegerUpDown1.Value = 0;
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {

            jsObject.editQuantity(Chrome, uiIntegerUpDown2.Value, 1);
            uiIntegerUpDown2.Value = 0;
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            jsObject.editQuantity(Chrome, uiIntegerUpDown3.Value, 2);
            uiIntegerUpDown3.Value = 0;
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
                        List<jsQuanModel> AllItem = JsonConvert.DeserializeObject<List<jsQuanModel>>(json);
                        List<requestQuantity> models = new List<requestQuantity>();

                        foreach (var item in AllItem)
                        {
                            //获取商品
                            var good_model = _GoodsModel.Find(x => x.goods_id == item.goods_id);
                            long cnt = 0;
                            foreach (var item2 in item.sku_list)
                            {
                                //获取sku
                                var sku_model = good_model.sku_list.Find(x => x.sku_id == item2.sku_id);
                                cnt += item2.sku_quantity;
                                if (sku_model.sku_quantity == item2.sku_quantity)//无修改跳过
                                {
                                    continue;
                                }
                                sku_model.sku_quantity = item2.sku_quantity;
                                requestQuantity model = new requestQuantity();
                                model.quantity = item2.sku_quantity;
                                model.sku_id = item2.sku_id;
                                model.goods_id = item.goods_id;
                                model.Malls = item.Mallinfo;
                                models.Add(model);
                            }
                            good_model.goods_quantity = cnt;
                        }
                        InitUser.RunningTask.Add("修改库存" + stampNow, stampNow.ToString());
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
            InitUser.RunningTask.Remove("修改库存" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改库存");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<requestQuantity> models = e.Argument as List<requestQuantity>;
            RetMsg = Good_Quantity.Update_local(models);
        }

        private void ChangeQuantity_Load(object sender, EventArgs e)
        {
            InitBrowser();
        }
        ChromiumWebBrowser Chrome;
        JsObject_Quantity jsObject = new JsObject_Quantity();
        public void InitBrowser()
        {
            Chrome = new ChromiumWebBrowser(InitUser.pageUrl + "jingkeyun/goodQuantity.html");
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
    }
}
