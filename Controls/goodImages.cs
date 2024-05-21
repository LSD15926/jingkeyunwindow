using jingkeyun.Class;
using jingkeyun.Data;
using jingkeyun.Windows;
using Pdd_Models;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Controls
{
    public partial class goodImages : UserControl
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
        private bool _BgColor = false;
        public bool BgColor
        {
            get { return _BgColor; }
            set
            {
                _BgColor = value;
                if (value)
                {
                    this.BackColor = Color.FromArgb(220, 210, 231);
                }
            }
        }
        public long Good_id { get; set; }
        public goodImages()
        {
            InitializeComponent();
        }

        private Mallinfo _mallinfo;
        public Mallinfo mallinfo
        { get { return _mallinfo; } set { _mallinfo = value; } }

        private  void uiButton1_Click(object sender, EventArgs e)
        {
            //本地上传图片
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true; //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";//窗体标题
            
            dialog.Filter = "图片文件(*.jpg, *.png, *jpeg)| *.jpg;*.png;*jpeg"; //文件筛选.jpg;*.png
                                                               //黑认路径设置为我的电脑文件夹
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                List<string> paths = new List<string>();
                foreach (string FileName in dialog.FileNames)
                {
                    FileInfo fileInfo = new FileInfo(FileName);
                    long fileSize = fileInfo.Length; // 获取文件大小（字节）
                    if (fileSize < 1024 * 1024)
                    {
                        paths.Add(FileName);
                    }
                }

                foreach (string FileName in paths)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Size = new Size(50, 50);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.Image = LoadResizedImage(FileName,50,50);
                    pictureBox.Cursor = Cursors.Hand;
                    pictureBox.Image.Tag = FileName;
                    pictureBox.MouseDown += PictureBox_MouseDown;
                    uiFlowLayoutPanel1.Controls.Add(pictureBox);
                }
                Parallel.For(0, paths.Count, i =>
                {
                    try {
                        BackMsg backMsg = PIcture_Upload.upload(paths[i], _mallinfo.mall_token);
                        if (backMsg.Code == 0)
                        {
                            //pictureBox[i].Tag = backMsg.Mess;
                            uiFlowLayoutPanel1.Controls[2].Controls[i].Tag = backMsg.Mess;
                        }
                        else
                        {
                            MyMessageBox.ShowError( "第"+(i+1)+"张图片上传失败！" + backMsg.Mess);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MyMessageBox.ShowError("第" + (i + 1) + "张图片上传失败！" + ex.Message);
                        return;
                    }
                });
                getPath();
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
                this.uiContextMenuStrip1.Show(MousePosition.X,MousePosition.Y);
                activePic=sender as PictureBox;  
            }
        }

        PictureBox activePic = null;
        
        private void uiButton2_Click(object sender, EventArgs e)
        {
            if (MyMessageBox.ShowAsk("是否清空已上传的图片？"))
            {
                uiFlowLayoutPanel1.Controls[2].Controls.Clear();
                getPath();
                GC.Collect();
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activePic != null)
            {
                uiFlowLayoutPanel1.Remove(activePic);
                getPath();
                GC.Collect();
            }
        }

        public List<string> ImagePath = new List<string>(); 
        /// <summary>
        /// 获取上传的图片的拼多多地址
        /// </summary>
        private void getPath()
        {
            ImagePath.Clear();
            foreach (Control control in uiFlowLayoutPanel1.Controls[2].Controls)
            {
                if (control.GetType().Name == "PictureBox")
                {
                    ImagePath.Add(((PictureBox)control).Tag.ToString());
                }
            }
        }
        private void uiButton3_Click(object sender, EventArgs e)
        {
            //获取images传给
            List<Image> images = new List<Image>();
            foreach (Control control in uiFlowLayoutPanel1.Controls[2].Controls)
            {
                if (control.GetType().Name == "PictureBox")
                {
                    images.Add(((PictureBox)control).Image);
                }
            }
            if (images.Count == 0)
            {
                MyMessageBox.Show("请先上传图片！");
                return;
            }
            SortPic sort = new SortPic();
            sort.AllImage = images;
            
            if (sort.ShowDialog() == DialogResult.OK)
            {
                for(int i=0;i<sort.BackImage.Count;i++)
                {
                    foreach (Control control in uiFlowLayoutPanel1.Controls[2].Controls)
                    {
                        if (((PictureBox)control).Image.Tag.ToString() == sort.BackImage[i])
                        {
                            uiFlowLayoutPanel1.Controls[2].Controls.SetChildIndex(control, i);
                        }
                        
                    }
                }
                getPath();
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
