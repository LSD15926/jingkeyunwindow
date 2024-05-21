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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Controls;
using jingkeyun.Data;
using jingkeyun.Class;

namespace jingkeyun.Windows
{
    public partial class ChangeTitle : UIForm
    {

        private List<Image> _images = new List<Image>();

        public List<Image> Images
        {
            get { return _images; }
            set { _images = value; }
        }


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
                    goodTitleOnly User = new goodTitleOnly();
                    User.Image = Images[cnt];
                    User.Title = item.goods_name;
                    User.Good_id = item.goods_id;
                    User.mallinfo = item.Mallinfo;
                    uiFlowLayoutPanel1.Controls.Add(User);
                    cnt++;
                }
            }
        }
        public ChangeTitle()
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
                if (item.GetType().Name != "goodTitleOnly")
                {
                    continue;
                }
                (item as goodTitleOnly).Title = (item as goodTitleOnly).Title.Replace(txtOld.Text, txtNew.Text);

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
                if (item.GetType().Name != "goodTitleOnly")
                {
                    continue;
                }
                (item as goodTitleOnly).Title = txtBegin.Text + (item as goodTitleOnly).Title;
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
                if (item.GetType().Name != "goodTitleOnly")
                {
                    continue;
                }
                (item as goodTitleOnly).Title = (item as goodTitleOnly).Title + txtEnd.Text;
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
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodTitleOnly")
                {
                    continue;
                }
                goodTitleOnly User = (item as goodTitleOnly);
                RequstGoodEditModel model = new RequstGoodEditModel();
                model.ApiType = (int)GoodsEdit.标题;
                model.goods_id = User.Good_id;
                model.goods_name = User.Title;
                model.Malls = User.mallinfo;
                if (string.IsNullOrEmpty(User.Title))
                {
                    MyMessageBox.ShowError("标题不能为空！");
                    return;
                }
                Encoding gb2312Encoding = Encoding.GetEncoding("GB2312");
                if (gb2312Encoding.GetByteCount(User.Title) > 60)
                {
                    MyMessageBox.ShowError("标题不能超过60字符！");
                    return;
                }
                models.Add(model);
                //缓存数据刷新
                GoodsModel.Find(x => x.goods_id == User.Good_id).goods_name = User.Title;
            }

            InitUser.RunningTask.Add("标题" + stampNow, stampNow.ToString());
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
            InitUser.RunningTask.Remove("标题" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改标题");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
        }
    }
}
