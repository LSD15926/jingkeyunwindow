using APIOffice.pddApi;
using Newtonsoft.Json;
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
    public partial class ChangeWritePic : UIForm
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
                //渲染页面
                int cnt = 0;
                foreach (var item in _GoodsModel)
                {
                    goodFileSpaceImage User = new goodFileSpaceImage();
                    User.Image = Images[cnt];
                    User.Title = item.goods_name + "\r\nID:" + item.goods_id;
                    User.Good_id = item.goods_id;
                    User.mallinfo = item.Mallinfo;
                    User.imageType = 1;
                    User.BgColor = cnt % 2 == 1;
                    uiFlowLayoutPanel1.Controls.Add(User);
                    cnt++;
                }
            }
        }
        public ChangeWritePic()
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
            MyMessageBox.ShowLoading();
            //获取提交请求列表
            List<FileSpaceUpload> models = new List<FileSpaceUpload>();
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodFileSpaceImage")
                {
                    continue;
                }
                goodFileSpaceImage User = (item as goodFileSpaceImage);
                if (User.spaceResponse == null)
                {
                    continue;
                }
                FileSpaceUpload model = new FileSpaceUpload();
                model.content = User.spaceResponse.url;
                model.file_id = User.spaceResponse.file_id;
                model.goods_id = User.Good_id;
                model.material_type = 1;
                model.access_token = User.mallinfo.mall_token;
                models.Add(model);
            }

            bool flag = true;
            string ErrorMsg = "";
            if (Parallel.For(0, models.Count, i =>
            {
                try
                {
                    BackMsg backMsg = PIcture_Upload.create(models[i]);
                    if (backMsg.Code == 0)
                    {
                    }
                    else
                    {
                        ErrorMsg = backMsg.Mess;
                        flag = false;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg = ex.Message;
                    flag = false;
                }
            }).IsCompleted)
            {
                MyMessageBox.IsShowLoading = false;
                if (flag)
                {
                    MyMessageBox.ShowSuccess("提交成功！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MyMessageBox.ShowError("提交失败！" + ErrorMsg);
                }
            }
        }
    }
}
