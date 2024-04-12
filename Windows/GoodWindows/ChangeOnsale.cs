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
    public partial class ChangeOnsale : UIForm
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

        private List<requsetSaleBody> requsets = new List<requsetSaleBody>();  
        public ChangeOnsale()
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (UIMessageBox.ShowAsk("是否提交修改？"))
            {
                new UIPage().ShowProcessForm();
                foreach (var item in _GoodsModel)
                {
                    requsetSaleBody body = new requsetSaleBody();
                    body.goods_id = item.goods_id;
                    body.is_onsale=uiRadioButton1.Checked?1:0;
                    body.Malls=item.Mallinfo;
                    requsets.Add(body);

                    item.is_onsale=body.is_onsale;
                }

                //发送请求
                BackMsg backMsg = Good_Sale.Status(requsets);
                if (backMsg.Code == 0)
                {
                    new UIPage().HideProcessForm();
                    UIMessageBox.ShowSuccess("修改成功!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    new UIPage().HideProcessForm();
                    UIMessageBox.ShowError("修改失败！"+backMsg.Mess);
                }
            }
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
