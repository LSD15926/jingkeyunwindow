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
    public partial class ChangeShipmentTime : UIForm
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

        public ChangeShipmentTime()
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
        bool flag=true;
        private void btnOK_Click(object sender, EventArgs e)
        {
            flag=false;
            if (UIMessageBox.ShowAsk("是否提交修改？"))
            {
                new UIPage().ShowProcessForm();
                //获取提交请求列表
                List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();

                int shipment_limit_second = 0;
                foreach (var radio in this.Controls)
                {
                    if (radio.GetType().Name == "UIRadioButton")
                    {
                        if (((UIRadioButton)radio).Checked)
                        {
                            shipment_limit_second= Convert.ToInt32(((UIRadioButton)radio).Tag.ToString());
                        }
                    }
                }

                foreach (var item in GoodsModel)
                {
                    RequstGoodEditModel model = new RequstGoodEditModel();
                    model.ApiType = (int)GoodsEdit.发货时间;
                    model.goods_id = item.goods_id;
                    model.shipment_limit_second = shipment_limit_second;
                    model.Malls = item.Mallinfo;
                    models.Add(model);
                }
                BackMsg backMsg = Good_Edit.Edit(models);
                if (backMsg.Code == 0)
                {
                    new UIPage().HideProcessForm();
                    UIMessageBox.ShowSuccess("修改成功！");
                    flag = true;
                    this.DialogResult=DialogResult.OK;
                    this.Close();
                }
                else
                {
                    new UIPage().HideProcessForm();
                    UIMessageBox.ShowError("出现错误！" + backMsg.Mess);
                    return;
                }
            }
        }

        private void ChangeShipmentTime_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!flag)
            {
                e.Cancel = true;
                flag = true;
            }
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
