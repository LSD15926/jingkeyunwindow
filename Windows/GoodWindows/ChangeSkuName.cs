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
using MoreLinq;
using Newtonsoft.Json;

namespace jingkeyun.Windows
{
    public partial class ChangeSkuName : UIForm
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
                    List<SpecItem> _specItems = new List<SpecItem>();

                    foreach (var item2 in item.sku_list)
                    {
                        if(item2.spec_details!=null)
                            _specItems.AddRange(item2.spec_details);
                    }
                    var NewSpec = _specItems.DistinctBy(x => x.spec_id).ToList();

                    foreach (var item3 in NewSpec)
                    {
                        skuName User = new skuName();
                        User.Image = Images[cnt];
                        User.Title = item.goods_name + "\r\nID:" + item.goods_id;
                        User.NewTitle = item3.spec_name;
                        User.specItem = item3;
                        User.mallinfo=item.Mallinfo;
                        uiFlowLayoutPanel1.Controls.Add(User);
                    }
                    
                    cnt++;
                }
            }
        }
        public ChangeSkuName()
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
            foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
            {
                if (item.GetType().Name != "skuName")
                {
                    continue;
                }
                (item as skuName).NewTitle = (item as skuName).NewTitle.Replace(txtOld.Text, txtNew.Text);

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
                if (item.GetType().Name != "skuName")
                {
                    continue;
                }
                (item as skuName).NewTitle = txtBegin.Text + (item as skuName).NewTitle;
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
                if (item.GetType().Name != "skuName")
                {
                    continue;
                }
                (item as skuName).NewTitle = (item as skuName).NewTitle + txtEnd.Text;
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
                List<SpecItem> OldSpec=new List<SpecItem>();
                List<SpecItem> NewSpec=new List<SpecItem>();
                foreach (var item in uiFlowLayoutPanel1.Panel.Controls)
                {
                    if (item.GetType().Name != "skuName")
                    {
                        continue;
                    }
                    skuName User = (item as skuName);
                    //判断是否发生修改
                    if (User.specItem.spec_name != User.NewTitle)
                    {

                        OldSpec.Add(User.specItem);
                        //请求生成新的spec
                        BackMsg msg = Good_Spec.Get(User.specItem.parent_id.ToString(),User.NewTitle,User.mallinfo.mall_token);
                        if (msg.Code == 0)
                        {
                            SpecItem spec = JsonConvert.DeserializeObject<SpecItem>(msg.Mess);
                            NewSpec.Add(spec);
                        }
                    }
                }
                //获取修改对象
                foreach (var model in GoodsModel)
                {
                    var Detail = goods_DetailModels.Find(x => x.goods_id == model.goods_id);
                    RequstGoodEditModel requst=new RequstGoodEditModel();
                    requst.ApiType = (int)GoodsEdit.sku名称;
                    requst.goods_id= model.goods_id;
                    requst.sku_list = Detail.sku_list;
                    requst.Malls = Detail.mall;
                    //替换sku
                    for (int i = 0; i < requst.sku_list.Count; i++)
                    {
                        for (int j = 0; j < requst.sku_list[i].spec.Count; j++)
                        {
                            int number = OldSpec.FindIndex(x => x.spec_id == requst.sku_list[i].spec[j].spec_id);
                            if (number > -1)
                            {
                                requst.sku_list[i].spec[j].spec_id = NewSpec[number].spec_id;
                                requst.sku_list[i].spec[j].spec_name = NewSpec[number].spec_name;
                            }
                        }
                    }
                    Detail.sku_list=requst.sku_list;
                    models.Add(requst);
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
                    UIMessageBox.ShowError("出现错误！" + backMsg.Mess);
                    return;
                }
            }
        }
    }
}
