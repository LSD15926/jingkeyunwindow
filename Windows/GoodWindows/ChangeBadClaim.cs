using APIOffice.pddApi;
using jingkeyun;
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

namespace jingkeyun.Windows
{
    public partial class ChangeBadClaim : UIForm
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
                uiLabel2.Text=value.Count.ToString();
            }
        }
        public ChangeBadClaim()
        {
            InitializeComponent();
            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = Color.FromArgb(137, 113, 179);

            panel1.BackColor = this.TitleColor;

            uiButton1.StyleCustomMode = true;
            uiButton1.Style = UIStyle.Custom;
            uiButton1.FillColor = Color.FromArgb(119, 40, 245);

            uiButton2.StyleCustomMode = true;
            uiButton2.Style = UIStyle.Custom;
            uiButton2.FillColor = Color.FromArgb(184, 134, 248);
        }
        LoadingForm loading;
        bool succ=false;
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (UIMessageBox.ShowAsk("是否提交修改？"))
            {
                loading = new LoadingForm(this);
                loading.Location = this.Location;
                loading.Show();
                BackgroundWorker worker = new BackgroundWorker(); 
                worker.DoWork += Worker_DoWork;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.RunWorkerAsync();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loading.Close();
            if (succ)
            {
                succ= false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //获取提交请求列表
            List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
            foreach (var item in GoodsModel)
            {
                RequstGoodEditModel model = new RequstGoodEditModel();
                model.ApiType = (int)GoodsEdit.坏果包赔;
                model.goods_id = item.goods_id;
                model.bad_fruit_claim = uiRadioButton1.Checked ? 1 : 0;
                model.Malls = item.Mallinfo;
                models.Add(model);
            }
            BackMsg backMsg = Good_Edit.Edit(models);
            if (backMsg.Code == 0)
            {
                UIMessageBox.ShowSuccess("修改成功！");
                succ=true;
            }
            else
            {
                UIMessageBox.ShowError("出现错误！" + backMsg.Mess);
                return;
            }
        }

        private void ChangeShipmentTime_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
