using Newtonsoft.Json;
using Pdd_Models.Models;
using Pdd_Models;
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
using jingkeyun.Data;
using jingkeyun.Controls;
using APIOffice.pddApi;
using jingkeyun.Class;

namespace jingkeyun.Windows
{
    public partial class ChangeTemplate : UIForm
    {

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
                uiLabel2.Text = value.Count.ToString();
                InitTemp();
            }
        }
        public ChangeTemplate()
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
        }

        private void InitTemp()
        {
            TempRequset requset = new TempRequset();
            requset.mall = GoodsModel[0].Mallinfo;
            BackMsg msg = Good_Template.out_Get(requset);
            if (msg.Code != 0)
            {
                return;
            }
            List<GoodsTemplate> templates = JsonConvert.DeserializeObject<List<GoodsTemplate>>(msg.Mess);
            ddlTemp.DataSource = templates;
            ddlTemp.DisplayMember = "template_name";
            ddlTemp.ValueMember = "template_id";

            ddlTemp.SelectedIndex = -1;
        }

        private void ddlTemp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTemp.SelectedIndex >= 0)
            {
                GoodsTemplate template = ddlTemp.SelectedItem as GoodsTemplate;
                uiLabel3.Text = "计费方式：" + (template.cost_type == 0 ? "按件计费" : "按重量计费");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ddlTemp.SelectedIndex == -1)
            {
                MyMessageBox.Show("请选择运费模板后提交!");
                return;
            }
            //获取提交请求列表
            List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
            GoodsTemplate template = ddlTemp.SelectedItem as GoodsTemplate;
            foreach (var item in GoodsModel)
            {

                RequstGoodEditModel model = new RequstGoodEditModel();
                model.ApiType = (int)GoodsEdit.运费模板;
                model.goods_id = item.goods_id;
                model.cost_template_id = template.template_id;
                model.Malls = item.Mallinfo;
                models.Add(model);
            }

            InitUser.RunningTask.Add("运费模板" + stampNow, stampNow.ToString());
            UIMessageTip.ShowOk("已提交至后台处理");
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync(models);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        BackMsg RetMsg;
        private long stampNow;
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitUser.RunningTask.Remove("运费模板" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改运费模板");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
        }

        private void ChangeTemplate_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
