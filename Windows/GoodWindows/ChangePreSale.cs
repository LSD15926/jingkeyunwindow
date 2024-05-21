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
using DevComponents.DotNetBar;
using APIOffice.pddApi;

namespace jingkeyun.Windows
{
    public partial class ChangePreSale : UIForm
    {

        private List<Goods_detailModel> _GoodsModel = new List<Goods_detailModel>();

        public List<Goods_detailModel> GoodsModel
        {
            get
            {
                return _GoodsModel;
            }
            set
            {
                _GoodsModel = value;
                uiLabel2.Text = value.Count.ToString();
            }
        }

        private List<RequstGoodEditModel> requsets = new List<RequstGoodEditModel>();
        public ChangePreSale()
        {
            InitializeComponent();
            InitMyStyle();
            //初始化数据
            DsTime.MinDate = DateTime.Now.AddDays(3);
            DsTime.MaxDate = DateTime.Now.AddDays(30);


        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;

            panel2.BackColor = StyleHelper.Title;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (uiRadioButton3.Checked && ddlquantity.Text == "")
            {
                MyMessageBox.ShowError("预售时间不能为空!");
                return;
            }


            if (uiRadioButton1.Checked)
            {
                foreach (var item in _GoodsModel)
                {
                    //生成请求包
                    RequstGoodEditModel model= new RequstGoodEditModel();
                    model.ApiType = (int)GoodsEdit.商品预售_1;
                    model.Malls = item.mall;
                    model.goods_id = item.goods_id;
                    requsets.Add(model);
                }
            }
            else if (uiRadioButton2.Checked)
            {
                foreach (var item in _GoodsModel)
                {
                    //生成请求包
                    RequstGoodEditModel model = new RequstGoodEditModel();
                    model.ApiType = (int)GoodsEdit.商品预售_2;
                    model.pre_sale_time = MyConvert.ToTimeStamp(DsTime.Value.ToString("yyyy-MM-dd 23:59:59"));
                    model.Malls = item.mall;
                    model.goods_id = item.goods_id;
                    requsets.Add(model);
                }
            }
            else {
                foreach (var item in _GoodsModel)
                {
                    //生成请求包
                    RequstGoodEditModel model = new RequstGoodEditModel();
                    model.ApiType = (int)GoodsEdit.商品预售_3;

                    model.shipment_limit_second = int.Parse(ddlquantity.Text) * 86400;
                    model.Malls = item.mall;
                    model.goods_id = item.goods_id;
                    requsets.Add(model);
                }
            }

            InitUser.RunningTask.Add("商品预售" + stampNow, stampNow.ToString());
            UIMessageTip.ShowOk("已提交至后台处理");
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync(requsets);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        BackMsg RetMsg;
        private long stampNow;
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitUser.RunningTask.Remove("商品预售" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改商品预售");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
        }
        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (uiRadioButton2.Checked)
            {
                panelDs.Visible = true;
                panelSd.Visible=false;
            }
        }

        private void uiRadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (uiRadioButton3.Checked)
            {
                panelSd.Visible = true;
                panelDs.Visible = false;

            }
        }

        private void uiRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (uiRadioButton1.Checked)
            {
                panelSd.Visible = false;
                panelDs.Visible = false;

            }
        }
    }
}
