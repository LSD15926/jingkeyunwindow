using jingkeyun.Class;
using jingkeyun.Windows;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Class;
using jingkeyun.Data;

namespace jingkeyun.Controls
{
    public partial class goodFileSpaceImage : UserControl
    {


        public Image Image
        {
            set
            {
                pictureBox1.Image = value;
            }
        }

        public string Title
        {
            set
            {
                uiLabel1.Text = value;
            }
        }

        public long Good_id { get; set; }
        public goodFileSpaceImage()
        {
            InitializeComponent();
        }
        public FileSpaceResponse spaceResponse=null;

        /// <summary>
        /// 1白底图 3长图
        /// </summary>
        public int imageType = 1;

        private Mallinfo _mallinfo;
        public Mallinfo mallinfo
        { get { return _mallinfo; } set { _mallinfo = value; } }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            //本地上传图片
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false; //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";//窗体标题

            dialog.Filter = "图片文件(*.jpg, *.png, *jpeg)| *.jpg;*.png;*jpeg"; //文件筛选.jpg;*.png
                                                                            //黑认路径设置为我的电脑文件夹
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                string FileName = dialog.FileNames[0];
                PictureBox pictureBox = new PictureBox();
                pictureBox.Size = new Size(100, 100);
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Image = LoadResizedImage(FileName, 100, 100);
                pictureBox.Cursor = Cursors.Hand;
                pictureBox.Image.Tag = FileName;
                pictureBox.MouseDown += PictureBox_MouseDown;
                uiFlowLayoutPanel1.Controls.Add(pictureBox);
                Parallel.For(0, 1, i =>
                {
                    try
                    {
                        BackMsg backMsg = PIcture_Upload.uploadToFileSpace(FileName, _mallinfo.mall_token);
                        if (backMsg.Code == 0)
                        {
                            spaceResponse = JsonConvert.DeserializeObject<FileSpaceResponse>(backMsg.Mess);
                        }
                        else
                        {
                            UIMessageBox.ShowError("图片上传失败！" + backMsg.Mess);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        UIMessageBox.ShowError("图片上传失败！" + ex.Message);
                        return;
                    }
                });
            }
        }

        private Image LoadResizedImage(string fileName, int width, int height)
        {
            using (Image originalImage = Image.FromFile(fileName))
            {
                // 创建一个新的Bitmap对象来存储调整尺寸后的图片  
                Bitmap resizedImage = new Bitmap(width, height);

                // 使用Graphics对象来绘制调整尺寸后的图片  
                using (Graphics graphics = Graphics.FromImage(resizedImage))
                {
                    // 设置高质量插值法  
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    // 绘制图片到新的Bitmap对象中，并调整其大小  
                    graphics.DrawImage(originalImage, 0, 0, width, height);
                }

                return resizedImage;
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.uiContextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                activePic = sender as PictureBox;
            }
        }

        PictureBox activePic = null;

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activePic != null)
            {
                uiFlowLayoutPanel1.Remove(activePic);
                spaceResponse = null;
                GC.Collect();
            }
        }

        private void 查看ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activePic != null)
            {
                BrowserHelper.OpenBrowserUrl(activePic.Tag.ToString());
            }
        }
    }
}
