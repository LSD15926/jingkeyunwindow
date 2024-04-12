using BsPhpHelper;
using jingkeyun.Class;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using jingkeyun.Data;
using jingkeyun.Windows;

namespace jingkeyun
{
    public partial class LoginForm : UIForm
    {
        public BsPhp bsPhp;
        public LoginForm()
        {
            InitializeComponent();
            if (SetListConfig.GetConfig("Remember") == "记住密码")
            {
                checkBoxX1.Checked = true;
                textBoxX1.Text = SetListConfig.GetConfig("name");
                textBoxX2.Text = SetListConfig.GetConfig("psw");
            }
            this.panel1.BackColor = Color.FromArgb(85, 67, 153);
            uiSymbolLabel1.StyleCustomMode = true;
            uiSymbolLabel1.Style = Sunny.UI.UIStyle.Custom;
            uiSymbolLabel1.ForeColor = Color.White;
            uiSymbolLabel1.SymbolColor = Color.White;

            uiSymbolLabel2.StyleCustomMode = true;
            uiSymbolLabel2.Style = Sunny.UI.UIStyle.Custom;
            uiSymbolLabel2.ForeColor = Color.White;
            uiSymbolLabel2.SymbolColor = Color.White;

            uiButton1.StyleCustomMode = true;
            uiButton1.Style = Sunny.UI.UIStyle.Custom;
            uiButton1.ForeColor = Color.Purple;

            uiButton2.StyleCustomMode = true;
            uiButton2.Style = Sunny.UI.UIStyle.Purple;
        }
        LoadingForm loading;
        private void uiButton1_Click(object sender, EventArgs e)
        {
            //立即登录
            if (string.IsNullOrEmpty(textBoxX1.Text) || string.IsNullOrEmpty(textBoxX2.Text))
            {
                UIMessageTip.ShowError("账号或密码不能为空!");
                return;
            }
            loading = new LoadingForm(this);
            loading.Location = this.Location;
            loading.Show();
            backgroundWorker1.RunWorkerAsync();
        }

        private void labelX2_Click(object sender, EventArgs e)
        {
            //免费注册
            SignForm form = new SignForm(this);
            form.Show();
            this.Hide();
        }

        private void labelX3_Click(object sender, EventArgs e)
        {
           Application.Exit();  
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            if (!UIMessageBox.ShowAsk("解绑会扣除1天系统使用时间，是否继续？"))
            {
                return ;
            }
            if (string.IsNullOrEmpty(textBoxX1.Text) || string.IsNullOrEmpty(textBoxX2.Text))
            {
                UIMessageTip.ShowError("账号或密码不能为空!");
                return;
            }
            loading = new LoadingForm(this);
            loading.Location = this.Location;
            loading.Show();
            backgroundWorker2.RunWorkerAsync();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            bsPhp = new BsPhp();
        }

        private void labelX4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxX1.Text) || string.IsNullOrEmpty(textBoxX2.Text))
            {
                UIMessageTip.ShowError("账号或密码不能为空!");
                return;
            }
            TopUp form=new TopUp();
            form.UserName = textBoxX1.Text;
            form.UserPsw = textBoxX2.Text;
            form.ShowDialog();
        }
        bool succ=false;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //验证账号登录
            BackMsg backMsg = bsPhp.AppEn_LogIn(textBoxX1.Text.Trim(), textBoxX2.Text.Trim(), "");
            if (backMsg.Code == 0)
            {
                if (backMsg.Mess.Contains("1011"))
                {
                    //获取用户基本信息
                    backMsg = bsPhp.AppEn_GetUserInfo("UserName,UserMobile,UserVipDate");
                    if (backMsg.Code == 0)
                    {
                        try
                        {
                            List<string> info = new List<string>();
                            info = backMsg.Mess.Split(',').ToList();
                            if (info.Count != 3)
                            {
                                //loading.Close();
                                UIMessageBox.ShowError("获取用户信息失败！" + backMsg.Mess);
                                return;
                            }
                            LoginUser loginUser = new LoginUser();
                            loginUser.user_name = info[0];
                            loginUser.user_Phone = info[1];
                            loginUser.user_expire = MyConvert.ToTimeStamp(info[2]).ToString();
                            loginUser.user_psw = textBoxX2.Text.Trim();
                            //数据库二次验证
                            BackData backData = Sys_User.BSPHP_Login(loginUser);
                            if (backData.Code == 0)
                            {
                                var json = JsonConvert.SerializeObject(backData.Data);
                                InitUser.User = JsonConvert.DeserializeObject<List<LoginUser>>(json)[0];
                                //保存密码
                                if (checkBoxX1.Checked)
                                {
                                    SetListConfig.SetConfig("name", textBoxX1.Text.Trim());
                                    SetListConfig.SetConfig("psw", textBoxX2.Text.Trim());
                                    SetListConfig.SetConfig("Remember", "记住密码");
                                }
                                succ=true;
                            }
                        }
                        catch (Exception ex)
                        {
                            UIMessageBox.ShowError("获取用户信息失败！" + ex.Message);
                        }
                    }
                    else
                    {
                        UIMessageBox.ShowError("获取用户信息失败！" + backMsg.Mess);
                    }

                }
                else if (backMsg.Mess.Contains("9908"))
                {
                    UIMessageBox.Show("你的帐号已经到期了哦,请续费");
                    return;
                }
                else
                {
                    UIMessageBox.Show(backMsg.Mess);
                    return;
                }
            }
            else
            {
                UIMessageBox.ShowError("登录失败！" + backMsg.Mess);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loading.Close();
            if (succ)
            {
                succ = false;
                FormMain formMain = new FormMain(this);
                formMain.Show();
                this.Hide();
            }
        }

        //解绑
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackMsg backMsg = bsPhp.AppEn_Unbundling(textBoxX1.Text, textBoxX2.Text);
            if (backMsg.Code == 0)
            {
                //解绑成功！
                if (backMsg.Mess.Contains("解除绑定成功"))
                {
                    UIMessageTip.Show("解绑成功！");
                }
                else
                {
                    UIMessageBox.ShowError("解绑失败！" + backMsg.Mess);
                }
            }
            else
            {
                UIMessageBox.ShowError("解绑失败！" + backMsg.Mess);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loading.Close();
        }
    }
}
