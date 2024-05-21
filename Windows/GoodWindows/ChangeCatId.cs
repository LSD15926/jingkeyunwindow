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
using jingkeyun.Class;

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
                uiLabel2.Text = value.Count.ToString();
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
            this.TitleColor = StyleHelper.Title;

            panel1.BackColor = this.TitleColor;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            GoodsCat cat = uiLabel5.Tag as GoodsCat;
            if (!cat.leaf)
            {
                MyMessageBox.Show("请选择完整的分类！");
                return;
            }

            //获取提交请求列表
            List<RequstGoodEditModel> models = new List<RequstGoodEditModel>();

            foreach (var item in GoodsModel)
            {
                RequstGoodEditModel model = new RequstGoodEditModel();
                model.ApiType = (int)GoodsEdit.叶子类目;
                model.goods_id = item.goods_id;
                model.cat_id = cat.cat_id;
                model.Malls = item.Mallinfo;
                models.Add(model);
            }
            InitUser.RunningTask.Add("商品分类" + stampNow,stampNow.ToString());
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
            InitUser.RunningTask.Remove("商品分类" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改商品分类");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<RequstGoodEditModel> models = e.Argument as List<RequstGoodEditModel>;
            RetMsg = Good_Edit.Edit(models);
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
                        treeNode.Text = cat.cat_name;
                        treeNode.Tag = cat;
                        control.Nodes.Add(treeNode);
                    }
                }
            }
            else
            {
                MyMessageBox.ShowError("获取失败！" + backMsg.Mess);
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
            uiLabel5.Text = a.cat_name;
            uiLabel5.Tag = a;
        }

        private void uiTreeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GoodsCat a = (uiTreeView2.SelectedNode.Tag as GoodsCat);
            uiTreeView3.Nodes.Clear();
            uiTreeView4.Nodes.Clear();
            if (!a.leaf)
                GetCats(uiTreeView3, a.cat_id);
            uiLabel5.Text = (uiTreeView1.SelectedNode.Tag as GoodsCat).cat_name + ">" + a.cat_name;
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
