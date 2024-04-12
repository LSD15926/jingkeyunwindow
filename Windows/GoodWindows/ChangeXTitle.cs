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

namespace jingkeyun.Windows
{
    public partial class ChangeXTitle : UIForm
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
                    User.Title=item.goods_name+"\r\nID:"+item.goods_id;
                    //todo 接口获取短标题
                    User.NewTitle=goods_DetailModels.FirstOrDefault(x => x.goods_id == item.goods_id).tiny_name;
                    User.Good_id=item.goods_id;
                    User.mallinfo=item.Mallinfo;
                    uiFlowLayoutPanel1.Controls.Add(User);
                    cnt++;
                }
            }
        }
        public ChangeXTitle()
        {
            InitializeComponent();
            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = Color.FromArgb(137, 113, 179);

            panel2.BackColor = this.TitleColor;

            uiButton1.StyleCustomMode = true;
            uiButton1.Style = UIStyle.Custom;
            uiButton1.FillColor = Color.FromArgb(119, 40, 245);

            uiButton2.StyleCustomMode = true;
            uiButton2.Style = UIStyle.Custom;
            uiButton2.FillColor = Color.FromArgb(184, 134, 248);

            uiButton3.StyleCustomMode = true;
            uiButton3.Style = UIStyle.Custom;
            uiButton3.FillColor = Color.FromArgb(119, 40, 245);

            uiButton4.StyleCustomMode = true;
            uiButton4.Style = UIStyle.Custom;
            uiButton4.FillColor = Color.FromArgb(119, 40, 245);

            uiButton5.StyleCustomMode = true;
            uiButton5.Style = UIStyle.Custom;
            uiButton5.FillColor = Color.FromArgb(119, 40, 245);
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
                (item as goodTitle).NewTitle =  (item as goodTitle).NewTitle+ txtEnd.Text;
            }
            txtEnd.Text = "";
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (UIMessageBox.ShowAsk("是否提交修改？"))
            {
                new UIPage().ShowProcessForm();
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
                    model.ApiType = (int)GoodsEdit.短标题;
                    model.goods_id=User.Good_id;
                    model.tiny_name=User.NewTitle;
                    model.Malls=User.mallinfo;
                    if (string.IsNullOrEmpty(User.NewTitle))
                    {
                        new UIPage().HideProcessForm();
                        UIMessageBox.Show("标题不能为空！");
                        return;
                    }
                    Encoding gb2312Encoding = Encoding.GetEncoding("GB2312");
                    if (gb2312Encoding.GetByteCount(User.NewTitle) > 20)
                    {
                        new UIPage().HideProcessForm();
                        UIMessageBox.Show("短标题不能超过20字符！");
                        return;
                    }
                    models.Add(model);
                }
                BackMsg backMsg = Good_Edit.Edit(models);
                if (backMsg.Code == 0)
                {
                    new UIPage().HideProcessForm();
                    UIMessageBox.ShowSuccess("修改成功！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    new UIPage().HideProcessForm();
                    UIMessageBox.ShowError("出现错误！"+backMsg.Mess);
                    return;
                }
            }
        }
    }
}
