using BsPhpHelper;
using jingkeyun.Class;
using Pdd_Models;
using Sunny.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

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
            this.TitleColor = StyleHelper.Title;

            uiSymbolLabel1.StyleCustomMode = true;
            uiSymbolLabel1.Style = UIStyle.Custom;
            uiSymbolLabel1.ForeColor = Color.Red;
            uiSymbolLabel1.SymbolColor = Color.Red;

            panel1.BackColor = this.TitleColor;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);

        }
        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(uiTextBox1.Text))
            {
                MyMessageBox.ShowError("请输入充值卡号！");
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
                    UIMessageTip.ShowOk("充值成功！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MyMessageBox.ShowError("充值卡激活失败：" + backMsg.Mess);
                }

            }
            else
            {
                MyMessageBox.ShowError("充值失败!" + backMsg.Mess);
                return;
            }

        }
    }
}
