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
    public partial class ChangeQuantity : UIForm
    {

        private List<Image> _images=new List<Image>();

        public List<Image> Images
        {
            get { return _images; }
            set { _images = value; }
        }


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
                    for (int i = 0; i < item.sku_list.Count; i++)
                    {
                        goodSkuQuantity User = new goodSkuQuantity();
                        if (i == 0)
                        {
                            User.Image = Images[cnt];
                            User.Title = item.goods_name + "\r\nID:" + item.goods_id;
                        }
                        User.Good_id = item.goods_id;
                        User.Quantity =Convert.ToInt32(item.sku_list[i].sku_quantity);
                        User.Sku=item.sku_list[i];
                        string skuName = "";
                        foreach (var spec in item.sku_list[i].spec_details)
                        {
                            skuName += "/"+ spec.spec_name;
                        }
                        if (!string.IsNullOrEmpty(skuName))
                        {
                            User.SkuName = skuName.Substring(1);
                        }
                        User.mallinfo=item.Mallinfo;
                        uiFlowLayoutPanel1.Controls.Add(User);
                    }
                    cnt++;
                }
            }
        }
        public ChangeQuantity()
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

            uiIntegerUpDown1.StyleCustomMode = true;
            uiIntegerUpDown1.Style = UIStyle.Purple;

            uiIntegerUpDown2.StyleCustomMode = true;
            uiIntegerUpDown2.Style = UIStyle.Purple;

            uiIntegerUpDown3.StyleCustomMode = true;
            uiIntegerUpDown3.Style = UIStyle.Purple;
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            foreach(var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodSkuQuantity")
                {
                    continue;
                }
                (item as goodSkuQuantity).Quantity=uiIntegerUpDown1.Value;
            }
            uiIntegerUpDown1.Value = 0;
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {

            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodSkuQuantity")
                {
                    continue;
                }
                (item as goodSkuQuantity).Quantity += uiIntegerUpDown2.Value;
            }
            uiIntegerUpDown2.Value = 0;
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodSkuQuantity")
                {
                    continue;
                }
                (item as goodSkuQuantity).Quantity -= uiIntegerUpDown3.Value;
            }
            uiIntegerUpDown3.Value = 0;
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
                List<requestQuantity> models = new List<requestQuantity>();
                long NowGood = 0;
                long Quantity = 0;
                foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
                {
                    if (item.GetType().Name != "goodSkuQuantity")
                    {
                        continue;
                    }
                    goodSkuQuantity User = (item as goodSkuQuantity);

                    requestQuantity model = new requestQuantity();
                    model.quantity=User.Quantity;
                    model.goods_id=User.Good_id;
                    model.sku_id=User.Sku.sku_id;
                    model.outer_id=User.Sku.outer_id;
                    model.Malls=User.mallinfo;
                    models.Add(model);
                    if (NowGood != User.Good_id)
                    {
                        //切换商品
                        if (NowGood != 0)
                        {
                            GoodsModel.Find(x=>x.goods_id==NowGood).goods_quantity=Quantity;
                        }
                        NowGood = User.Good_id;
                        Quantity = 0;
                    }
                    Quantity += model.quantity;

                }
                GoodsModel.Find(x => x.goods_id == NowGood).goods_quantity = Quantity;

                BackMsg backMsg = Good_Quantity.Update(models);
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
                    UIMessageBox.ShowError("出现错误！" + backMsg.Mess);
                    return;
                }
            }
        }
    }
}
