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
    public partial class LoadingForm : UIForm
    {
        public LoadingForm(Form father=null)
        {
            InitializeComponent();
            this.Location = father == null ? InitUser.MainForm.Location : father.Location;
            this.Size = father == null ? InitUser.MainForm.Size : father.Size;
            pictureBox1.Location = new Point(this.Width / 2 - 75, this.Height / 2 - 75);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!MyMessageBox.IsShowLoading)
            {
                timer1.Stop();
                this.Close();
                //CloseMm();
            }
        }

        private void CloseMm()
        {
            // 设置窗体的动画样式
            //this.Opacity = 1.0; // 重置透明度为不透明
            this.TopMost = true; // 将窗体置于最顶层以确保动画可见

            // 播放淡出动画
            Timer timer = new Timer();
            timer.Interval = 100; // 动画间隔（以毫秒为单位）
            timer.Tick += (s, args) =>
            {
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.1; // 每次间隔减少5%的透明度
                }
                else
                {
                    timer.Stop(); // 透明度达到0时停止动画
                    this.TopMost = false; // 停止窗体顶层特性
                    this.Close(); // 关闭窗体
                }
            };
            timer.Start();
        }
    }
}
