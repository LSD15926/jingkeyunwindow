using APIOffice.pddApi;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using Sunny.UI.Win32;
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

namespace jingkeyun.Windows
{
    public partial class ChangeGoodCode : UIForm
    {

        private List<Image> _images = new List<Image>();

        public List<Image> Images
        {
            get { return _images; }
            set { _images = value; }
        }
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
                uiLabel3.Text = value.Count.ToString();
                //渲染页面
                int cnt = 0;
                foreach (var item in _GoodsModel)
                {
                    var model = goods_DetailModels.FirstOrDefault(x => x.goods_id == item.goods_id);

                    goodOutId User = new goodOutId();
                    User.Image = Images[cnt];
                    User.Title = item.goods_name + "\r\nID:" + item.goods_id;
                    User.Good = model;
                    User.BgColor = cnt % 2 == 1;
                    Ugood.Add(User);
                    cnt++;
                }
                uiFlowLayoutPanel1.SuspendLayout();
                uiFlowLayoutPanel1.Controls.AddRange(Ugood.ToArray());
                uiFlowLayoutPanel1.ResumeLayout();
            }
        }
        List<goodOutId> Ugood = new List<goodOutId>();
        public ChangeGoodCode()
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

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (MyMessageBox.ShowAsk("是否提交修改？"))
            {
                List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
                foreach (var User in Ugood)
                {
                    RequstGoodEditModel model = new RequstGoodEditModel();
                    model.outer_goods_id = User.Good.outer_goods_id.ToString();
                    model.ApiType = (int)GoodsEdit.商品编码;
                    model.goods_id = User.Good.goods_id;
                    model.Malls = User.Good.mall;
                    model.sku_list = User.Good.sku_list;
                    models.Add(model);
                }
                InitUser.RunningTask.Add("详情图" + stampNow, stampNow.ToString());
                UIMessageTip.ShowOk("已提交至后台处理");
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += Worker_DoWork;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.RunWorkerAsync(models);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        BackMsg RetMsg;
        private long stampNow;
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitUser.RunningTask.Remove("详情图" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改商品编码与规格编码");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            Goods_detailModel model = new Goods_detailModel();
            foreach (var U in Ugood)
            {
                model = U.Good;
                model.outer_goods_id = txtBegin.Text;
                U.Good = model;
            }
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            try
            {
                Goods_detailModel model = new Goods_detailModel();
                foreach (var U in Ugood)
                {
                    model = U.Good;
                    model.sku_list[0].out_sku_sn = uiTextBox1.Text;
                    U.Good = model;
                }
            }
            catch
            {
            }
        }

        private void uiButton7_Click(object sender, EventArgs e)
        {
            try
            {
                Goods_detailModel model = new Goods_detailModel();
                foreach (var U in Ugood)
                {
                    model = U.Good;
                    model.sku_list[0].out_sku_sn = model.sku_list[0].out_sku_sn.Replace(txtOld.Text, txtNew.Text);
                    U.Good = model;
                }
            }
            catch
            {
            }
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            Goods_detailModel model = new Goods_detailModel();
            foreach (var U in Ugood)
            {
                model = U.Good;
                model.outer_goods_id = "";
                U.Good = model;
            }
        }

        private void uiButton6_Click(object sender, EventArgs e)
        {
            try
            {
                Goods_detailModel model = new Goods_detailModel();
                foreach (var U in Ugood)
                {
                    model = U.Good;
                    model.sku_list[0].out_sku_sn = "";
                    U.Good = model;
                }
            }
            catch
            {
            }
        }
    }
}
