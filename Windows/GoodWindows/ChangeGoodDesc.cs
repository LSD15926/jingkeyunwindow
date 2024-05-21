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
using jingkeyun.Class;

namespace jingkeyun.Windows
{
    public partial class ChangeGoodDesc : UIForm
    {

        private List<Image> _images=new List<Image>();

        public List<Image> Images
        {
            get { return _images; }
            set { _images = value; }
        }
        public List<Goods_detailModel> goods_DetailModels { get; set; }

        private List<GoodListResponse> _GoodsModel=new List<GoodListResponse>();

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
                    goodTitle User=new goodTitle();
                    User.Image = Images[cnt];
                    User.Title = item.goods_name + "\r\nID:" + item.goods_id;
                    User.NewTitle= goods_DetailModels.FirstOrDefault(x => x.goods_id == item.goods_id).goods_desc;
                    User.Good_id=item.goods_id;
                    User.mallinfo=item.Mallinfo;
                    User.BgColor = cnt % 2 == 1;
                    uiFlowLayoutPanel1.Controls.Add(User);
                    cnt++;
                }
            }
        }
        public ChangeGoodDesc()
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

            uiButton1.FillColor = Color.FromArgb(119, 40, 245);

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
            foreach(var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodTitle")
                {
                    continue;
                }
                (item as goodTitle).NewTitle= (item as goodTitle).NewTitle.Replace(txtOld.Text, txtNew.Text);
                
            }
            txtOld.Text = "";
            txtOld.Text = "";
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBegin.Text))
            {
                return;
            }

            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodTitle")
                {
                    continue;
                }
                (item as goodTitle).NewTitle = txtBegin.Text+(item as goodTitle).NewTitle;
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
                if (item.GetType().Name != "goodTitle")
                {
                    continue;
                }
                (item as goodTitle).NewTitle =   (item as goodTitle).NewTitle+ txtEnd.Text;
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
                if (item.GetType().Name != "goodTitle")
                {
                    continue;
                }
                goodTitle User= (item as goodTitle);
                RequstGoodEditModel model = new RequstGoodEditModel();
                model.ApiType = (int)GoodsEdit.商品描述;
                model.goods_id=User.Good_id;
                model.goods_desc=User.NewTitle;
                model.Malls=User.mallinfo;
                if (string.IsNullOrEmpty(User.NewTitle))
                {
                    MyMessageBox.ShowError("商品描述不能为空！");
                    return;
                }
                Encoding gb2312Encoding = Encoding.GetEncoding("GB2312");
                if (gb2312Encoding.GetByteCount(User.NewTitle) > 500|| gb2312Encoding.GetByteCount(User.NewTitle)<20)
                {
                    MyMessageBox.ShowError("商品描述字数限制：20-500！");
                    return;
                }
                models.Add(model);
                goods_DetailModels.Find(x=>x.goods_id==User.Good_id).goods_desc= model.goods_desc;
            }
            InitUser.RunningTask.Add("商品描述" + stampNow, stampNow.ToString());
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
            InitUser.RunningTask.Remove("商品描述" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改商品描述");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
        }

    }
}
