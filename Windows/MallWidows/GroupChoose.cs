using jingkeyun.Class;
using jingkeyun.Data;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace jingkeyun.Windows
{
    public partial class GroupChoose : UIForm
    {

        private Mallinfo _Mall=null;
        public Mallinfo Mall
        {
            get { return  _Mall; }
            set { _Mall = value; 
                txtName.Text=value.mall_name;
            }
        }


        private List<MallGroup> _mallGroups=new List<MallGroup>();
        public List<MallGroup> mallGroups
        {
            get { return _mallGroups; }
            set { 
                _mallGroups = value;
                //绑定下拉列表
                InitGroup();
            }
        }


        public GroupChoose()
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

        private void InitGroup()
        {
            Class.ComboBoxItem item = new Class.ComboBoxItem();
            item.Text = "无分组";
            item.Value = 0;
            ddlGroup.Items.Add(item);
            foreach(var  group in mallGroups)
            {
                item = new Class.ComboBoxItem();
                item.Text = group.group_name;
                item.Value = group.group_id;
                int i= ddlGroup.Items.Add(item);
                //if (group.group_id == _Mall.mall_group)
                //{
                //    ddlGroup.SelectedIndex = i;
                //}
            }
            if(ddlGroup.SelectedIndex==-1)
                ddlGroup.SelectedIndex = 0;

        }
        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click_1(object sender, EventArgs e)
        {
            //_Mall.mall_group=MyConvert.ToInt((ddlGroup.SelectedItem as Class.ComboBoxItem).Value);

            BackMsg backMsg = Mall_Info.Upd(_Mall);
            if (backMsg.Code == 0)
            {

                MyMessageBox.ShowSuccess("修改成功！");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MyMessageBox.ShowError("修改失败!" + backMsg.Mess);
                return;
            }

        }
    }
}
