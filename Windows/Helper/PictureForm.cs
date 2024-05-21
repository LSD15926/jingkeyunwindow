using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun
{
    public partial class PictureForm : UIForm
    {
        public PictureForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            this.TopMost = true; // 确保窗口总是在最上面  
                                 // 其他初始化代码...  
        }

        public void ShowBigPicture(Image image)
        {
            // 设置PictureBox的图片和位置  
            this.pictureBox1.Image = image;
            this.Show(); // 显示窗口  
        }
    }
}
