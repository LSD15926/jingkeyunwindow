using Newtonsoft.Json;
using Pdd_Models.Models;
using Pdd_Models;
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
using jingkeyun.Data;
using jingkeyun.Controls;
using APIOffice.pddApi;

namespace jingkeyun.Windows
{
    public partial class ChangeTemplate : UIForm
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
                InitTemp();
            }
        }
        public ChangeTemplate()
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

        private void InitTemp()
        {
            TempRequset requset = new TempRequset();
            requset.mall = GoodsModel[0].Mallinfo;
            BackMsg msg = Good_Template.out_Get(requset);
            if (msg.Code != 0)
            {
                return;
            }
            List<GoodsTemplate> templates = JsonConvert.DeserializeObject<List<GoodsTemplate>>(msg.Mess);
            ddlTemp.DataSource = templates;
            ddlTemp.DisplayMember = "template_name";
            ddlTemp.ValueMember = "template_id";

            ddlTemp.SelectedIndex = -1;
        }

        private void ddlTemp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTemp.SelectedIndex >= 0)
            {
                GoodsTemplate template= ddlTemp.SelectedItem as GoodsTemplate;
                uiLabel3.Text="计费方式："+(template.cost_type==0?"按件计费":"按重量计费");
            }
        }

        bool flag=true;
        private void btnOK_Click(object sender, EventArgs e)
        {
            flag=false;
            if (ddlTemp.SelectedIndex == -1)
            {
                UIMessageBox.Show("请选择运费模板后提交!");
                return;
            }
            if (UIMessageBox.ShowAsk("是否提交修改？"))
            {
                new UIPage().ShowProcessForm();
                //获取提交请求列表
                List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();
                GoodsTemplate template = ddlTemp.SelectedItem as GoodsTemplate;
                foreach (var item in GoodsModel)
                {
                    
                    RequstGoodEditModel model = new RequstGoodEditModel();
                    model.ApiType = (int)GoodsEdit.运费模板;
                    model.goods_id = item.goods_id;
                    model.cost_template_id= template.template_id;
                    model.Malls=item.Mallinfo;
                    models.Add(model);
                }
                BackMsg backMsg = Good_Edit.Edit(models);
                if (backMsg.Code == 0)
                {
                    new UIPage().HideProcessForm();
                    UIMessageBox.ShowSuccess("修改成功！");
                    flag=true;
                }
                else
                {
                    new UIPage().HideProcessForm();
                    UIMessageBox.ShowError("出现错误！" + backMsg.Mess);
                    return;
                }
            }
        }

        private void ChangeTemplate_FormClosing(object sender, FormClosingEventArgs e)
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
