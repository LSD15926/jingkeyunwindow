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
    public partial class EditCode : UIForm
    {


        public Goods_detailModel goods_DetailModels { get; set; }

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
                uiLabel3.Text = value.goods_name;

                bool flag=false;
                foreach (var item in goods_DetailModels.sku_list)
                {
                    goodCode User = new goodCode();
                    User.Sku = item;
                    User.SkuName = value.sku_list.Find(x => x.sku_id == item.sku_id).spec;
                    User.OutSku = item.out_sku_sn;
                    User.BgColor=flag;
                    flag=!flag;
                    Ugood.Add(User);
                }
                uiLabel3.Text = value.goods_name;
                uiTextBox2.Text = goods_DetailModels.outer_goods_id;
                uiFlowLayoutPanel1.Controls.AddRange(Ugood.ToArray());
            }
        }
        List<goodCode> Ugood = new List<goodCode>();
        public EditCode()
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
                RequstGoodEditModel model = new RequstGoodEditModel();
                model.outer_goods_id = uiTextBox2.Text;
                model.ApiType = (int)GoodsEdit.商品编码;
                model.goods_id = goods_DetailModels.goods_id;
                model.Malls = goods_DetailModels.mall;
                foreach (var User in Ugood)
                {
                    User.Sku.out_sku_sn = User.OutSku;
                    model.sku_list.Add(User.Sku);
                }
                models.Add(model);

                InitUser.RunningTask.Add("单商品编码" + stampNow, stampNow.ToString());
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
            InitUser.RunningTask.Remove("单商品编码" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改商品编码与规格编码");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var U in Ugood)
                {

                    U.OutSku = uiTextBox1.Text;
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
                foreach (var U in Ugood)
                {

                    U.OutSku = U.OutSku.Replace(txtOld.Text, txtNew.Text);
                }
            }
            catch
            {
            }
        }



        private void uiButton6_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var U in Ugood)
                {
                    U.OutSku = "";
                }
            }
            catch
            {
            }
        }
    }
}
