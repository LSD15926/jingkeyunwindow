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
    public partial class ChangeGoodCode : UIForm
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
                        goodOutId User = new goodOutId();
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
                        User.mallinfo=item.Mallinfo;
                        User.outGood=model.outer_goods_id;
                        var sku = model.sku_list.FirstOrDefault(x => x.sku_id == item.sku_list[i].sku_id);
                        User.outSku =sku.out_sku_sn;
                        User.sku_List = sku;
                        uiFlowLayoutPanel1.Controls.Add(User);
                    }
                    cnt++;
                }
            }
        }
        public ChangeGoodCode()
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
                List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
                RequstGoodEditModel model = new RequstGoodEditModel();

                foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
                {                    
                    if (item.GetType().Name != "goodOutId")
                    {
                        continue;
                    }
                    goodOutId User = (item as goodOutId);
                    if (model.goods_id != 0 && model.goods_id != User.Good_id)//非同一商品
                    {
                        models.Add(model);
                        model = new RequstGoodEditModel();
                    }
                    if (model.goods_id == 0)
                    {
                        model.outer_goods_id = User.outGood;
                        model.ApiType = (int)GoodsEdit.商品编码;
                        model.goods_id = User.Good_id;
                        model.Malls = User.mallinfo;
                    }
                    Sku_listItem sku=new Sku_listItem();
                    sku=User.sku_List;
                    sku.out_sku_sn=User.outSku;
                    model.sku_list.Add(sku);
                }
                models.Add(model);
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
                    UIMessageBox.ShowError("出现错误！" + backMsg.Mess);
                    return;
                }
                
            }
        }

       
    }
}
