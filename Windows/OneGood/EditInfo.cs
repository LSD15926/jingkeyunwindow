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
using CefSharp.DevTools.Inspector;
using Sunny.UI.Win32;

namespace jingkeyun.Windows
{
    public partial class EditInfo : UIForm
    {
        /// <summary>
        /// 0 标题
        /// 1 短标题
        /// 2 商品描述
        /// </summary>
        public int pageType { get; set; } = 0;
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
                //页面渲染
                switch (pageType)
                {
                    case 0:
                        txtMain.Text = value.goods_name;
                        break;
                    case 1:
                        this.Text = "编辑短标题";
                        LabelType.Text = "短标题";
                        txtMain.Text = DetailModel.tiny_name;
                        break;
                    case 2:
                        this.Text = "";
                        LabelType.Text = "商品描述";
                        txtMain.Text = DetailModel.goods_desc;
                        break;
                }
            }
        }
        public EditInfo()
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
            //获取提交请求列表
            if (string.IsNullOrEmpty(txtMain.Text.Trim()))
            {
                MyMessageBox.ShowError($"请输入{LabelType.Text}!");
                return;
            }
            MyMessageBox.ShowLoading(this);
            RequstGoodEditModel model = new RequstGoodEditModel();
            model.goods_id = DetailModel.goods_id;
            model.Malls = DetailModel.mall;
            Encoding gb2312Encoding = Encoding.GetEncoding("GB2312");
            switch (pageType)
            {
                case 0:
                    model.ApiType=(int)GoodsEdit.标题;
                    if (gb2312Encoding.GetByteCount(txtMain.Text.Trim()) > 60)
                    {
                        MyMessageBox.IsShowLoading=false;
                        MyMessageBox.ShowError("标题不能超过60字符！");
                        return;
                    }
                    model.goods_name = txtMain.Text.Trim();
                    break;
                case 1:
                    model.ApiType = (int)GoodsEdit.短标题;
                    if (gb2312Encoding.GetByteCount(txtMain.Text.Trim()) > 20)
                    {
                        MyMessageBox.IsShowLoading = false;
                        MyMessageBox.ShowError("短标题不能超过20字符！");
                        return;
                    }
                    model.tiny_name = txtMain.Text.Trim();
                    break;
                case 2:
                    model.ApiType = (int)GoodsEdit.商品描述;
                    if (gb2312Encoding.GetByteCount(txtMain.Text.Trim()) > 500 || gb2312Encoding.GetByteCount(txtMain.Text.Trim())<20)
                    {
                        MyMessageBox.IsShowLoading = false;
                        MyMessageBox.ShowError("商品描述字数限制：20-500！");
                        return;
                    }
                    model.goods_desc = txtMain.Text.Trim();
                    break;
            }
            //发送修改请求
            BackMsg RetMsg= Good_Edit.PostGoodEdit(model);
            if (RetMsg.Code == 0)
            {
                MyMessageBox.IsShowLoading = false;
                MyMessageBox.showCheck("修改成功！");
                //修改本地数据
                switch (pageType)
                {
                    case 0:
                        _GoodsModel.goods_name = txtMain.Text.Trim();
                        DetailModel.goods_name=txtMain.Text.Trim();
                        break;
                    case 1:

                        DetailModel.tiny_name = txtMain.Text.Trim();
                        break;
                    case 2:
                        DetailModel.goods_desc = txtMain.Text.Trim();
                        break;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MyMessageBox.IsShowLoading = false;
                MyMessageBox.ShowError($"修改失败：{RetMsg.Mess}");
            }

        }

    }
}
