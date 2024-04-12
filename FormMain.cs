using jingkeyun.Class;
using jingkeyun.Data;
using jingkeyun.Pinduoduo;
using jingkeyun.Windows;
using Pdd_Models;
using Sunny.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace jingkeyun
{
    public partial class FormMain : UIForm
    {
        LoginForm form1 = null;
        public FormMain(LoginForm form)
        {
            form1 = form;
            InitializeComponent();
            UIStyles.InitColorful(Color.FromArgb(107, 77, 160), Color.White);
            InitMyStyle();
            uiLabel1.Text = InitUser.User.user_name;
            uiLabel3.Text = "版本：V1.0.1";
            ActLabel = this.uiSymbolLabel1;
            ActLabel.ForeColor = Color.Yellow;
            ActLabel.SymbolColor = Color.Yellow;
            ActLabel.BackColor = Color.FromArgb(89, 27, 183);
            txtExpire.Text = MyConvert.StampToTime(InitUser.User.user_expire).ToString("yyyy-MM-dd HH:mm");
            LogEvent($"登录成功！{InitUser.User.user_name}，欢迎使用本系统！");
            //绑定菜单
            InitMenu();
        }

        private void InitMyStyle()
        {
            this.uiSymbolLabel1.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel1.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel2.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel2.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel3.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel3.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel4.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel4.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel5.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel5.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel6.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel6.ForeColor = System.Drawing.Color.White;
            uiLabel1.ForeColor = System.Drawing.Color.White;
        }
        private void InitMenu()
        {
            mailList F1=new mailList();
            F1.UpdLog += LogEvent;
            goodsList F2 =new goodsList();
            F2.UpdLog += LogEvent;
            MainTabControl.AddPage(F1);
            MainTabControl.AddPage(F2);

        }

        private void LogEvent(string message)
        {
            txtLog.AppendText(DateTime.Now.ToString("HH:mm:ss  ")+message + Environment.NewLine);
        }

        private UISymbolLabel ActLabel = null;
        private void uiSymbolLabel1_Click(object sender, EventArgs e)
        {
            //获取点击的区域
            ActLabel.ForeColor = Color.White;
            ActLabel.SymbolColor = Color.White;
            ActLabel.BackColor = System.Drawing.Color.Transparent;
            ActLabel = sender as UISymbolLabel;
            ActLabel.ForeColor = Color.Yellow;
            ActLabel.SymbolColor = Color.Yellow;
            ActLabel.BackColor = Color.FromArgb(89, 27, 183);
            MainTabControl.SelectedIndex = MyConvert.ToInt((sender as UISymbolLabel).Tag)> MainTabControl.TabCount?-1: MyConvert.ToInt((sender as UISymbolLabel).Tag);
        }
        /// <summary>
        /// 续费
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelX2_Click(object sender, EventArgs e)
        {
            TopUp form= new TopUp();
            if (form.ShowDialog() == DialogResult.OK)
            {
                //获取数据库，刷新有效期
                BackMsg backMsg = form1.bsPhp.AppEn_GetUserInfo("UserVipDate");
                try
                {

                    InitUser.User.user_expire = MyConvert.ToTimeStamp(backMsg.Mess).ToString();

                    //数据库二次验证
                    BackData backData = Sys_User.BSPHP_Login(InitUser.User);
                    if (backData.Code == 0)
                    {
                    }
                    txtExpire.Text=backMsg.Mess;
                }
                catch (Exception ex)
                {
                    UIMessageBox.ShowError("获取用户信息失败！" + ex.Message);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            BackMsg backMsg = form1.bsPhp.AppEn_timeout();
            if (backMsg.Mess != "5031")
            {
                UIMessageBox.Show("登录状态已过期，请重新登录！");
                Application.Restart();
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //退出登录
            form1.bsPhp.AppEn_Cancell();

        }
    }
}
