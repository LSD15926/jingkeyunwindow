using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Class;
using jingkeyun.Data;

namespace jingkeyun.Windows
{
    public partial class GroupEdit : UIForm
    {

        private MallGroup _mallGroups=new MallGroup();
        public  MallGroup mallGroups
        {
            get { return _mallGroups; }
            set { 
                _mallGroups = value; 
                txtName.Text=value.group_name;
                txtDesc.Text=value.group_notes;
            }
        }


        public GroupEdit()
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
        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                UIMessageBox.ShowError("请输入分组名称！");
                return;
            }
            _mallGroups.group_name= txtName.Text;
            _mallGroups.group_notes=txtDesc.Text;
            BackMsg backMsg = Mall_Group.Edit(_mallGroups);
            if (backMsg.Code == 0)
            {

                UIMessageBox.ShowSuccess("修改成功！");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                UIMessageBox.ShowError("修改失败!" + backMsg.Mess);
                return;
            }

        }
    }
}
