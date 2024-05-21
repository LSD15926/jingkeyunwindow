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
                int cnt = 0;
                foreach (var item in _GoodsModel)
                {
                    SkuBoxs skuBoxs = new SkuBoxs();
                    skuBoxs.DetailModel = goods_DetailModels.FirstOrDefault(x=>x.goods_id==item.goods_id);
                    skuBoxs.BgColor = cnt % 2 == 1;
                    uiFlowLayoutPanel1.Controls.Add(skuBoxs);
                    cnt++;
                }
            }
        }
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
            if (string.IsNullOrEmpty(txtOld.Text))
            {
                return;
            }
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "SkuBoxs")
                {
                    continue;
                }
                (item as SkuBoxs).Change(txtOld.Text, txtNew.Text,0);

            }
            txtOld.Text = "";
            txtNew.Text = "";
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBegin.Text))
            {
                return;
            }

            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "SkuBoxs")
                {
                    continue;
                }
                (item as SkuBoxs).Change(txtBegin.Text, "", 1);
            }
            txtBegin.Text = "";

        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEnd.Text))
            {
                return;
            }
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "SkuBoxs")
                {
                    continue;
                }
                (item as SkuBoxs).Change(txtEnd.Text, "", 2);
            }
            txtEnd.Text = "";
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {

            //获取提交请求列表
            List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
            List<SpecItem> OldSpec = new List<SpecItem>();
            List<SpecItem> NewSpec = new List<SpecItem>();
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "SkuBoxs")
                {
                    continue;
                }
                SkuBoxs User = (item as SkuBoxs);

                
                RequstGoodEditModel requst = new RequstGoodEditModel();
                requst.ApiType = (int)GoodsEdit.sku名称;
                requst.goods_id = User.DetailModel.goods_id;
                requst.sku_list = User.getNewSku();
                requst.Malls = User.DetailModel.mall;
                models.Add(requst);
            }
            InitUser.RunningTask.Add("SKU名称" + stampNow, stampNow.ToString());
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
            InitUser.RunningTask.Remove("SKU名称" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改SKU名称");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
        }
    }
}
