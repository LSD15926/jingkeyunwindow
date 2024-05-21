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
using MoreLinq;
using Newtonsoft.Json;
using jingkeyun.Class;
using Sunny.UI.Win32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using CefSharp.WinForms;
using CefSharp;

namespace jingkeyun.Windows
{
    public partial class ChangeSkuName : UIForm
    {

        public List<Goods_detailModel> goods_DetailModels { get; set; }

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
                foreach (var item in _GoodsModel)
                {
                    jsSkuNameModel model= new jsSkuNameModel();
                    model.goods_id = item.goods_id;
                    model.goods_name = item.goods_name;
                    model.thumb_url = item.thumb_url;
                    model.Mallinfo = item.Mallinfo;
                    Goods_detailModel detailModel = goods_DetailModels.Find(x => x.goods_id == item.goods_id);
                    jsSkuName_spec spec1 = new jsSkuName_spec();
                    jsSkuName_spec spec2 = new jsSkuName_spec();
                    if (detailModel.sku_list[0].spec.Count == 2)
                    {
                        spec1.spec = detailModel.sku_list[0].spec[0].parent_name;
                        spec2.spec = detailModel.sku_list[0].spec[1].parent_name;
                        spec1.spec_id= (long)detailModel.sku_list[0].spec[0].parent_id;
                        spec2.spec_id = (long)detailModel.sku_list[0].spec[1].parent_id;
                        List<SpecItem> _specItems = new List<SpecItem>();
                        foreach (var item2 in detailModel.sku_list)
                        {
                            if (item2.spec != null)
                                _specItems.AddRange(item2.spec);
                        }
                        var NewSpec = _specItems.DistinctBy(x => x.spec_id).ToList();

                        long Pid = (long)detailModel.sku_list[0].spec[0].parent_id;
                        for (int i = 0; i < NewSpec.Count; i++)
                        {
                            jsSkuName_spec_list jsSkuName_Spec_List = new jsSkuName_spec_list();
                            jsSkuName_Spec_List.name= NewSpec[i].spec_name;
                            jsSkuName_Spec_List.id= NewSpec[i].spec_id;
                            if (Pid == (long)NewSpec[i].parent_id)
                                spec1.SpecList.Add(jsSkuName_Spec_List);
                            else
                                spec2.SpecList.Add(jsSkuName_Spec_List);
                        }
                        model.sku_specs.Add(spec1);
                        model.sku_specs.Add(spec2);
                    }
                    else
                    {
                        spec1.spec = detailModel.sku_list[0].spec[0].parent_name;
                        foreach (var sku in detailModel.sku_list)
                        {
                            jsSkuName_spec_list jsSkuName_Spec_List = new jsSkuName_spec_list();
                            jsSkuName_Spec_List.name = sku.spec[0].spec_name;
                            spec1.SpecList.Add(jsSkuName_Spec_List);
                        }
                        model.sku_specs.Add(spec1);
                    }
                    jsSkuNameModels.Add(model);
                }
            }
        }
        public List<jsSkuNameModel> jsSkuNameModels= new List<jsSkuNameModel>();
        public ChangeSkuName()
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

        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            jsObject.editQuantity(Chrome, txtOld.Text, txtNew.Text, 0);
            txtOld.Text = "";
            txtNew.Text = "";
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            jsObject.editQuantity(Chrome, txtBegin.Text, "", 1);
            txtBegin.Text = "";

        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            jsObject.editQuantity(Chrome, txtEnd.Text, "", 2);
            txtEnd.Text = "";
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
                        if (!result.Success)
                        {
                            MyMessageBox.ShowError("提交失败"+result.Message);
                        }
                        var json = JsonConvert.SerializeObject(result.Result);
                        List<jsSkuNameModel> AllItem = JsonConvert.DeserializeObject<List<jsSkuNameModel>>(json);

                        stampNow = MyConvert.ToTimeStamp(DateTime.Now);
                        InitUser.RunningTask.Add("SKU名称" + stampNow, stampNow.ToString());
                        UIMessageTip.ShowOk("已提交至后台处理");
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += Worker_DoWork;
                        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                        worker.RunWorkerAsync(AllItem);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowError("提交出错！" + ex.Message);
                    }

                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        BackMsg RetMsg;
        private long stampNow;
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitUser.RunningTask.Remove("SKU名称" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改SKU名称");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
             
            List<jsSkuNameModel> AllItem = e.Argument as List<jsSkuNameModel>;

            List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();

            for (int i = 0;i<AllItem.Count;i++)
            {
                //获取sku
                Goods_detailModel goods = goods_DetailModels.Find(x => x.goods_id == AllItem[i].goods_id);
                bool flag = false;
                for (int j = 0; j < AllItem[i].sku_specs.Count; j++)
                {
                    for (int k = 0; k < AllItem[i].sku_specs[j].SpecList.Count; k++)
                    {
                        if (jsSkuNameModels[i].sku_specs[j].SpecList[k].name== AllItem[i].sku_specs[j].SpecList[k].name)
                        {
                            continue;
                        }
                        BackMsg msg = Good_Spec.Get(AllItem[i].sku_specs[j].spec_id.ToString(), AllItem[i].sku_specs[j].SpecList[k].name, AllItem[i].Mallinfo.mall_token);
                        if (msg.Code == 0)
                        {
                            SpecItem spec = JsonConvert.DeserializeObject<SpecItem>(msg.Mess);
                            //long specId = ((C as UITextBox).Tag as SpecItem).spec_id;
                            foreach (var item2 in goods.sku_list)
                            {
                                int index = item2.spec.FindIndex(x => x.spec_id == AllItem[i].sku_specs[j].SpecList[k].id);
                                if (index != -1)
                                {
                                    item2.spec[index].spec_id = spec.spec_id;
                                    item2.spec[index].spec_name = spec.spec_name;
                                }
                            }
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    RequstGoodEditModel model= new RequstGoodEditModel();
                    model.goods_id= AllItem[i].goods_id;
                    model.Malls= AllItem[i].Mallinfo;
                    model.ApiType = (int)GoodsEdit.sku名称;
                    model.sku_list=goods.sku_list;
                    models.Add(model);
                }
            }
            RetMsg = Good_Edit.Edit(models);
        }

        private void ChangeSkuName_Load(object sender, EventArgs e)
        {
            InitBrowser();
        }
        ChromiumWebBrowser Chrome;
        JsObject_SkuName jsObject = new JsObject_SkuName();
        public void InitBrowser()
        {
            Chrome = new ChromiumWebBrowser(InitUser.pageUrl + "jingkeyun/goodSkuName.html");
            Chrome.MenuHandler = new MenuHandler();
            Chrome.KeyboardHandler = new CEFKeyBoardHander();
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled };
            Chrome.Dock = DockStyle.Fill;
            CefSharpSettings.WcfEnabled = true;
            Chrome.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            jsObject.f = this;
            Chrome.JavascriptObjectRepository.Register("boundAsync", jsObject, true, BindingOptions.DefaultBinder);
            this.panelMain.Controls.Add(Chrome);
        }
    }
}
