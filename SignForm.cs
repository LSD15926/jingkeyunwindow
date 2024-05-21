using BsPhpHelper;
using jingkeyun.Class;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Data;

namespace jingkeyun
{
    public partial class SignForm : UIForm
    {
        LoginForm loginForm1;
        public SignForm(LoginForm form)
        {
            loginForm1 = form;  
            InitializeComponent();
            StyleHelper.SetButtonColor(uiButton1, StyleHelper.HighLigtPurple);
        }
        private bool getModel()
        {
            bool flag=false;
            if (textBoxX4.Text != textBoxX3.Text)
            {
                MyMessageBox.Show("两次输入的密码不一致！");
                return flag;
            }
            if (string.IsNullOrEmpty(textBoxX5.Text))
            {
                MyMessageBox.Show("请输入登录账号！");
                return flag;
            }
            if (string.IsNullOrEmpty(textBoxX4.Text))
            {
                MyMessageBox.Show("请输入登录密码！");
                return flag;
            }
            if (string.IsNullOrEmpty(textBoxX1.Text))
            {
                MyMessageBox.Show("请输入充值卡号！");
                return flag;
            }
            if (!IsMobilePhone(textBoxX2.Text))
            {
                MyMessageBox.Show("请输入正确的手机号码！");
                return flag;
            }

            flag = true;
            return flag;
        }

        private bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1(3\\d|4[5-9]|5[0-35-9]|6[567]|7[0-8]|8\\d|9[0-35-9])\\d{8}$");
            return regex.IsMatch(input);
            
        }
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if(!getModel())
            {
                return;
            }
            MyMessageBox.ShowLoading(this);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MyMessageBox.IsShowLoading=false;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Sign();
        }

        private void Sign()
        {
            BackMsg backMsg = loginForm1.bsPhp.AppEn_registration(textBoxX5.Text.Trim(), textBoxX4.Text.Trim(), textBoxX3.Text.Trim(), "", "", "", "", textBoxX2.Text, "");
            if (backMsg.Code == 0)
            {
                try
                {
                    if (backMsg.Mess.Contains("注册成功"))//注册成功！
                    {
                        //充值卡充值
                        backMsg = loginForm1.bsPhp.AppEn_vipchong(textBoxX5.Text.Trim(), textBoxX4.Text.Trim(), "1", textBoxX1.Text.Trim(), "");
                        if (backMsg.Code == 0)
                        {
                            if (backMsg.Mess.Contains("激活成功"))
                            {
                                UIMessageTip.ShowOk("注册成功！");
                            }
                            else
                            {
                                MyMessageBox.ShowError("账号注册成功，但充值卡激活失败：" + backMsg.Mess);
                            }
                        }
                        else
                        {
                            MyMessageBox.ShowError("账号注册成功，但充值卡激活失败：" + backMsg.Mess);
                        }

                    }
                    else
                    {
                        MyMessageBox.ShowError("注册失败！" + backMsg.Mess);
                    }
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowError("注册失败！" + ex.Message);
                }
            }
            else
            {
                MyMessageBox.ShowError("注册失败：" + backMsg.Mess);
            }
        }
        private void labelX2_Click(object sender, EventArgs e)
        {
            loginForm1.Show();
            this.Close();   
        }

        private void labelX3_Click(object sender, EventArgs e)
        {
            loginForm1.Close();
            this.Close();
        }

        private void textBoxX2_Leave(object sender, EventArgs e)
        {
            if(!IsMobilePhone(textBoxX2.Text))
                UIMessageTip.ShowError("不是有效的手机号！");
        }
    }
}
