using jingkeyun.Controls;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Class
{
    public static class MyMessageBox
    {
        public static DialogForm df;
        public static DialogResult Show(string message,string Title= "消息提示") 
        { 
             df= new DialogForm();
            df.Text = Title;
            df.Msg = message;
            df.Image=global::jingkeyun.Properties.Resources.tishi;
            return df.ShowDialog();
        }
        public static DialogResult ShowSuccess(string message, string Title = "操作成功")
        {
             df = new DialogForm();
            df.Text = Title;
            df.Msg = message;
            df.IsCancel = false;
            df.Image = global::jingkeyun.Properties.Resources.succes;
            return df.ShowDialog();
        }
        public static DialogResult ShowError(string message, string Title = "操作失败")
        {
             df = new DialogForm();
            df.Text = Title;
            df.Msg = message;
            df.IsCancel = false;
            df.Image = global::jingkeyun.Properties.Resources.error;
            return df.ShowDialog();
        }
        public static bool ShowAsk(string message, string Title = "消息提示")
        {
            df = new DialogForm();
            df.Text = Title;
            df.Msg = message;
            df.Image = global::jingkeyun.Properties.Resources.tishi;
            return df.ShowDialog()==DialogResult.OK;
        }
        public static DialogResult showCheck(string message, string Title = "操作完成")
        {
            df = new DialogForm();
            df.Text = Title;
            df.Msg = message;
            df.IsCancel = false;
            df.Image = global::jingkeyun.Properties.Resources.succes;
            return df.ShowDialog();
        }
        private static LoadingForm f = null;

        private static bool flag=false;
        public static void ShowLoading(Form form = null)
        {
            if (IsShowLoading)
            {
                IsShowLoading=false;
                Thread.Sleep(100);
            }
            IsShowLoading = true;
            if (form == null)
                form = InitUser.MainForm;
            Thread thread = new Thread((ThreadStart)delegate
            {
                f = new LoadingForm(form);
                f.ShowInTaskbar = false;
                f.TopMost = true;
                f.DoubleBuffered(true);
                f.Render();
                Application.Run(f);
            });
            thread.Start();
        }
        public static bool IsShowLoading=false;
        

    }
}
