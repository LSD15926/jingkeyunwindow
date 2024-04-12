﻿using APIOffice.pddApi;
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
    public partial class ChangeLongPic : UIForm
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
                    goodFileSpaceImage User=new goodFileSpaceImage();
                    User.Image = Images[cnt];
                    User.Title = item.goods_name + "\r\nID:" + item.goods_id;
                    User.Good_id=item.goods_id;
                    User.mallinfo=item.Mallinfo;
                    User.imageType = 3;
                    uiFlowLayoutPanel1.Controls.Add(User);
                    cnt++;
                }
            }
        }
        public ChangeLongPic()
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
                    model.material_type = 3;
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
                    if (flag)
                    {
                        new UIPage().HideProcessForm();
                        UIMessageBox.ShowSuccess("提交成功！");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        new UIPage().HideProcessForm();
                        UIMessageBox.ShowError("提交失败！" + ErrorMsg);
                    }
                }
            }
        }
    }
}
