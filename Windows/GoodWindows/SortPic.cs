using jingkeyun.Class;
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
using static Sunny.UI.UIImageListBox;

namespace jingkeyun.Windows
{
    public partial class SortPic : UIForm
    {


        private List<Image> _allImage = new List<Image>();
        public List<Image> AllImage
        {
            get { return _allImage; }
            set { _allImage = value;
                foreach (Image image in _allImage)
                {
                    uiImageListBox1.AddImage(image.Tag.ToString());
                }
            }
        }
        public  List<string> BackImage = new List<string>();
        public SortPic()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            uiImageListBox1.MouseDown += uiImageListBox1_MouseDown;
            uiImageListBox1.MouseMove += uiImageListBox1_MouseMove;
            uiImageListBox1.MouseUp += uiImageListBox1_MouseUp;
            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;

            panel2.BackColor = this.TitleColor;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
        }

        private bool isDragging = false;
        private int dragStartIndex = -1;
        private Point dragStartPoint;
        private Point dragCurrentPoint;
        private void uiImageListBox1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int index = uiImageListBox1.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    dragStartIndex = index;
                    dragStartPoint = e.Location;
                    isDragging = true;
                }
            }
        }

        private void uiImageListBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isDragging)
            {
                dragCurrentPoint = e.Location;

                // 检查是否应该开始拖动排序  
                if (Math.Abs(dragCurrentPoint.Y - dragStartPoint.Y) > SystemInformation.DoubleClickSize.Height)
                {
                    // 这里可以开始实际的拖动排序逻辑  
                    // ...  
                }
            }
        }

        private void uiImageListBox1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;

                // 检查鼠标释放的位置，并更新项的顺序  
                int endIndex = uiImageListBox1.IndexFromPoint(e.Location);
                if (endIndex != ListBox.NoMatches && endIndex != dragStartIndex)
                {
                    object item = uiImageListBox1.Items[dragStartIndex];
                    uiImageListBox1.Items.RemoveAt(dragStartIndex);
                    uiImageListBox1.Items.Insert(endIndex, item);
                    uiImageListBox1.SelectedIndex = endIndex; // 可选：设置选中项为移动后的项  
                }
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < uiImageListBox1.Items.Count; i++)
            {
                BackImage.Add((uiImageListBox1.Items[i] as ImageListItem).ImagePath);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
