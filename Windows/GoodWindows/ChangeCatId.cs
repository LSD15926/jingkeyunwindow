using APIOffice.pddApi;
using Newtonsoft.Json;
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
    public partial class ChangeCatId : UIForm
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
                GetCats(uiTreeView1);
                uiLabel2.Text=value.Count.ToString();
            }
        }

        public ChangeCatId()
        {
            InitializeComponent();
            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = Color.FromArgb(137, 113, 179);

            panel1.BackColor = this.TitleColor;

            uiButton1.StyleCustomMode = true;
            uiButton1.Style = UIStyle.Custom;
            uiButton1.FillColor = Color.FromArgb(119, 40, 245);

            uiButton2.StyleCustomMode = true;
            uiButton2.Style = UIStyle.Custom;
            uiButton2.FillColor = Color.FromArgb(184, 134, 248);

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GoodsCat cat=uiLabel5.Tag as GoodsCat;
            if (!cat.leaf)
            {
                UIMessageBox.Show("请选择完整的分类！");
                return;
            }

            if (UIMessageBox.ShowAsk("是否提交修改？"))
            {
                new UIPage().ShowProcessForm();
                //获取提交请求列表
                List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();

                foreach (var item in GoodsModel)
                {
                    RequstGoodEditModel model = new RequstGoodEditModel();
                    model.ApiType = (int)GoodsEdit.叶子类目;
                    model.goods_id = item.goods_id;
                    model.cat_id = cat.cat_id;
                    model.Malls=item.Mallinfo;
                    models.Add(model);
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

        private void GetCats(UITreeView control, long cat_id = 0)
        {
            RequstCat requstCat = new RequstCat(cat_id, GoodsModel[0].Mallinfo);
            BackMsg backMsg = Good_Cat.Get(requstCat);
            if (backMsg.Code == 0)
            {
                control.Nodes.Clear();

                List<GoodsCat> goodsCats = JsonConvert.DeserializeObject<List<GoodsCat>>(backMsg.Mess);
                if (goodsCats.Count > 0)
                {
                    foreach (var cat in goodsCats)
                    {
                        TreeNode treeNode = new TreeNode();
                        treeNode.Text=cat.cat_name;
                        treeNode.Tag=cat;
                        control.Nodes.Add(treeNode);
                    }
                }
            }
            else
            {
                UIMessageBox.ShowError("获取失败！"+backMsg.Mess);
                return;
            }
        }
        private void uiTreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GoodsCat a = (uiTreeView1.SelectedNode.Tag as GoodsCat);
            uiTreeView2.Nodes.Clear();
            uiTreeView3.Nodes.Clear();
            uiTreeView4.Nodes.Clear();
            if (!a.leaf)
                GetCats(uiTreeView2, a.cat_id);
            uiLabel5.Text= a.cat_name;
            uiLabel5.Tag = a;
        }

        private void uiTreeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GoodsCat a = (uiTreeView2.SelectedNode.Tag as GoodsCat);
            uiTreeView3.Nodes.Clear();
            uiTreeView4.Nodes.Clear();
            if (!a.leaf)
                GetCats(uiTreeView3, a.cat_id);
            uiLabel5.Text = (uiTreeView1.SelectedNode.Tag as GoodsCat).cat_name+">"+ a.cat_name;
            uiLabel5.Tag = a;
        }

        private void uiTreeView3_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GoodsCat a = (uiTreeView3.SelectedNode.Tag as GoodsCat);
            uiTreeView4.Nodes.Clear();
            if (!a.leaf)
                GetCats(uiTreeView4, a.cat_id);
            uiLabel5.Text = (uiTreeView1.SelectedNode.Tag as GoodsCat).cat_name + ">" + (uiTreeView2.SelectedNode.Tag as GoodsCat).cat_name + ">" + a.cat_name;
            uiLabel5.Tag = a;
        }

        private void uiTreeView4_AfterSelect(object sender, TreeViewEventArgs e)
        {
            uiLabel5.Text = (uiTreeView1.SelectedNode.Tag as GoodsCat).cat_name + ">" + (uiTreeView2.SelectedNode.Tag as GoodsCat).cat_name + ">"
                + (uiTreeView3.SelectedNode.Tag as GoodsCat).cat_name + ">" + (uiTreeView4.SelectedNode.Tag as GoodsCat).cat_name;
            uiLabel5.Tag = uiTreeView4.SelectedNode.Tag;
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
