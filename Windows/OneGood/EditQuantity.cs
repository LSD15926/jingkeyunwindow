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

namespace jingkeyun.Windows
{
    public partial class EditQuantity : UIForm
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

                for (int i = 0; i < DetailModel.sku_list.Count; i++)
                {
                    skuQuantity User = new skuQuantity();
                    User.Quantity = MyConvert.ToInt(value.sku_list[i].sku_quantity);
                    User.sku_ListItem = DetailModel.sku_list[i];
                    User.BgColor = i % 2 == 1;
                    skuQuantities.Add(User);
                    uiFlowLayoutPanel1.Controls.Add(User);
                }
                txtName.Text=value.goods_name;
            }
        }
        private List<skuQuantity> skuQuantities = new List<skuQuantity>();
        public EditQuantity()
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
            foreach (var model in skuQuantities)
            {
                model.Quantity= uiIntegerUpDown1.Value;
            }
            uiIntegerUpDown1.Value = 0;
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {

            foreach (var model in skuQuantities)
            {
                model.Quantity += uiIntegerUpDown2.Value;
            }
            uiIntegerUpDown2.Value = 0;
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {


            foreach (var model in skuQuantities)
            {
                model.Quantity -= uiIntegerUpDown3.Value;
            }
            uiIntegerUpDown3.Value = 0;
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            //获取提交请求列表
            List<requestQuantity> models = new List<requestQuantity>();
            long Quantity = 0;
            foreach (var User in skuQuantities)
            {
                requestQuantity model = new requestQuantity();
                model.quantity = User.Quantity;
                model.goods_id = GoodsModel.goods_id;
                model.sku_id = MyConvert.ToLong(User.sku_ListItem.sku_id);
                //model.outer_id = User.Sku.outer_id;
                model.Malls = GoodsModel.Mallinfo;
                models.Add(model);

                Quantity += model.quantity;
            }
            GoodsModel.goods_quantity = Quantity;

            InitUser.RunningTask.Add("单商品库存" + stampNow, stampNow.ToString());
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
            InitUser.RunningTask.Remove("单商品库存" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改库存");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<requestQuantity> models = e.Argument as List<requestQuantity>;
            RetMsg = Good_Quantity.Update_local(models);
        }
    }
}
