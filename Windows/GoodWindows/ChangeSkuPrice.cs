using APIOffice.pddApi;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using Sunny.UI.Win32;
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
    public partial class ChangeSkuPrice : UIForm
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
                    var model = goods_DetailModels.FirstOrDefault(x => x.goods_id == item.goods_id);
                    for (int i = 0; i < item.sku_list.Count; i++)
                    {
                        goodSkuPrice User = new goodSkuPrice();
                        if (i == 0)
                        {
                            User.Image = Images[cnt];
                            User.Title = item.goods_name + "\r\nID:" + item.goods_id;
                        }
                        User.Good_id = item.goods_id;
                        User.Sku=item.sku_list[i];
                        string skuName = "";
                        foreach (var spec in item.sku_list[i].spec_details)
                        {
                            skuName += "/" + spec.spec_name;
                        }
                        if (!string.IsNullOrEmpty(skuName))
                        {
                            User.SkuName = skuName.Substring(1);
                        }
                        User.market_price= Convert.ToInt64(model.market_price)/100.00;
                        User.mallinfo=item.Mallinfo;
                        var sku= model.sku_list.FirstOrDefault(x => x.sku_id == item.sku_list[i].sku_id);
                        User.price=sku.price/100.00;
                        User.mPrice=Convert.ToInt64(sku.multi_price)/100.00;
                        uiFlowLayoutPanel1.Controls.Add(User);
                    }
                    cnt++;
                }
            }
        }
        public ChangeSkuPrice()
        {
            InitializeComponent();
            uiComboBox1.SelectedIndex = 0;
            uiComboBox2.SelectedIndex = 0;
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

            uiIntegerUpDown1.StyleCustomMode= true;
            uiIntegerUpDown1.Style = UIStyle.Purple;

            uiDoubleUpDown1.StyleCustomMode=true;
            uiDoubleUpDown1.Style = UIStyle.Purple;

            uiDoubleUpDown2.StyleCustomMode = true;
            uiDoubleUpDown2.Style = UIStyle.Purple;

            uiDoubleUpDown3.StyleCustomMode = true;
            uiDoubleUpDown3.Style = UIStyle.Purple;

            uiDoubleUpDown4.StyleCustomMode = true;
            uiDoubleUpDown4.Style = UIStyle.Purple;

        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            foreach(var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodSkuPrice")
                {
                    continue;
                }

                goodSkuPrice user=item as goodSkuPrice;
                if (uiComboBox1.Text == "加")
                {
                    user.mPrice += uiDoubleUpDown1.Value;
                }
                else
                {
                    user.mPrice -= uiDoubleUpDown1.Value;
                }
                if (uiComboBox2.Text == "加")
                {
                    user.price += uiDoubleUpDown2.Value;
                }
                else
                {
                    user.price -= uiDoubleUpDown2.Value;
                }


            }
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodSkuPrice")
                {
                    continue;
                }

                goodSkuPrice user = item as goodSkuPrice;
                user.mPrice = uiDoubleUpDown3.Value;
                user.price = uiDoubleUpDown4.Value;
            }
        }
        /// <summary>
        /// 倍率修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "goodSkuPrice")
                {
                    continue;
                }

                goodSkuPrice user = item as goodSkuPrice;
                user.mPrice = user.mPrice*uiIntegerUpDown1.Value/100;
                user.price = user.price* uiIntegerUpDown1.Value/100;
                user.market_price=user.market_price* uiIntegerUpDown1.Value/100;
                if(user.price<user.mPrice+1)
                    user.price = user.mPrice + 1;
                if(user.market_price< user.price)
                    user.market_price = user.price+1;
            }
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
                List<requsetSkuPriceModel> models = new List<requsetSkuPriceModel>();
                requsetSkuPriceModel model = new requsetSkuPriceModel();
                foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
                {
                    if (item.GetType().Name != "goodSkuPrice")
                    {
                        continue;
                    }
                    
                    goodSkuPrice User = (item as goodSkuPrice);
                    
                    if (model.goods_id != 0 && model.goods_id != User.Good_id)//非同一商品
                    {
                        models.Add(model);
                        model = new requsetSkuPriceModel();
                    }
                    if(model.market_price==null)
                        model.market_price = Convert.ToInt64(User.market_price * 100);
                    if (model.market_price <= User.price*100)
                    {
                        new UIPage().HideProcessForm();
                        UIMessageBox.ShowError("参考价必须大于单价！");
                        return;
                    }
                    if (User.mPrice > User.price - 1)
                    {
                        new UIPage().HideProcessForm();
                        UIMessageBox.ShowError("拼单价需比单买价低至少1元！");
                        return;
                    }
                    model.goods_id = User.Good_id;
                    SkuPrice price = new SkuPrice();
                    price.single_price = Convert.ToInt64(User.price*100);
                    price.group_price = Convert.ToInt64(User.mPrice*100);
                    price.sku_id=User.Sku.sku_id;
                    model.sku_price_list.Add(price);

                }
                models.Add(model);
                BackMsg backMsg = Good_Price.SkuPrice(models);
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
