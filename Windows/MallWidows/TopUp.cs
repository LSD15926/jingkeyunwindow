using BsPhpHelper;
using jingkeyun.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
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
    public partial class TopUp : UIForm
    {
        public string UserName = "";
        public string UserPsw = "";
        public TopUp()
        {
            InitializeComponent();
            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = Color.FromArgb(137, 113, 179);

            uiSymbolLabel1.StyleCustomMode = true;
            uiSymbolLabel1.Style = UIStyle.Custom;
            uiSymbolLabel1.ForeColor = Color.Red;
            uiSymbolLabel1.SymbolColor = Color.Red;

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
            if (string.IsNullOrEmpty(uiTextBox1.Text))
            {
                UIMessageBox.ShowError("请输入充值卡号！");
                return;
            }
            //接口验证卡密有效性

            BackMsg backMsg = new BackMsg();
            if (UserPsw == "")
            {
                backMsg = new BsPhp().AppEn_vipchong(InitUser.User.user_name, "", "0", uiTextBox1.Text.Trim(), "");
            }
            else
            {
                backMsg = new BsPhp().AppEn_vipchong(UserName, UserPsw, "1", uiTextBox1.Text.Trim(), "");
            }
            if (backMsg.Code == 0)
            {
                if (backMsg.Mess.Contains("激活成功"))
                {
                    UIMessageBox.ShowSuccess("充值成功！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    UIMessageBox.ShowError("充值卡激活失败：" + backMsg.Mess);
                }

            }
            else
            {
                UIMessageBox.ShowError("充值失败!" + backMsg.Mess);
                return;
            }

        }
    }
}
