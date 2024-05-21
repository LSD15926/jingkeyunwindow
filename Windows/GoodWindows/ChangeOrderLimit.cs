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
using jingkeyun.Class;

namespace jingkeyun.Windows
{
    public partial class ChangeOrderLimit : UIForm
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
                uiLabel2.Text = value.Count.ToString();
            }
        }
        public ChangeOrderLimit()
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
        private void btnOK_Click(object sender, EventArgs e)
        {
            //获取提交请求列表
            List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
            foreach (var item in GoodsModel)
            {
                RequstGoodEditModel model = new RequstGoodEditModel();
                model.ApiType = (int)GoodsEdit.限量;
                model.goods_id = item.goods_id;
                model.order_limit = uiIntegerUpDown1.Value;
                model.Malls = item.Mallinfo;
                models.Add(model);
            }
            InitUser.RunningTask.Add("单次限量" + stampNow, stampNow.ToString());
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
            InitUser.RunningTask.Remove("单次限量" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改单次限量");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
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
