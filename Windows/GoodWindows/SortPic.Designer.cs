namespace jingkeyun.Windows
{
    partial class SortPic
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uiImageListBox1 = new Sunny.UI.UIImageListBox();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.uiButton2 = new Sunny.UI.UIButton();
            this.uiButton1 = new Sunny.UI.UIButton();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiImageListBox1
            // 
            this.uiImageListBox1.FillColor = System.Drawing.Color.White;
            this.uiImageListBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiImageListBox1.ItemHeight = 50;
            this.uiImageListBox1.Location = new System.Drawing.Point(15, 74);
            this.uiImageListBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiImageListBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiImageListBox1.Name = "uiImageListBox1";
            this.uiImageListBox1.Padding = new System.Windows.Forms.Padding(2);
            this.uiImageListBox1.ShowDescription = false;
            this.uiImageListBox1.ShowText = false;
            this.uiImageListBox1.Size = new System.Drawing.Size(270, 571);
            this.uiImageListBox1.TabIndex = 0;
            this.uiImageListBox1.Text = "uiImageListBox1";
            this.uiImageListBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.uiImageListBox1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // uiLabel1
            // 
            this.uiLabel1.AutoSize = true;
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.ForeColor = System.Drawing.Color.Red;
            this.uiLabel1.Location = new System.Drawing.Point(23, 48);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(138, 21);
            this.uiLabel1.Style = Sunny.UI.UIStyle.Custom;
            this.uiLabel1.StyleCustomMode = true;
            this.uiLabel1.TabIndex = 2;
            this.uiLabel1.Text = "拖拽图片进行排序";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiLabel1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.uiButton2);
            this.panel2.Controls.Add(this.uiButton1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 659);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(299, 47);
            this.panel2.TabIndex = 39;
            // 
            // uiButton2
            // 
            this.uiButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uiButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton2.Location = new System.Drawing.Point(103, 7);
            this.uiButton2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton2.Name = "uiButton2";
            this.uiButton2.Size = new System.Drawing.Size(88, 34);
            this.uiButton2.TabIndex = 6;
            this.uiButton2.Text = "取 消";
            this.uiButton2.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton2.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton2.Click += new System.EventHandler(this.uiButton2_Click);
            // 
            // uiButton1
            // 
            this.uiButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uiButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.uiButton1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.Location = new System.Drawing.Point(197, 7);
            this.uiButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiButton1.Name = "uiButton1";
            this.uiButton1.Size = new System.Drawing.Size(88, 34);
            this.uiButton1.TabIndex = 5;
            this.uiButton1.Text = "提 交";
            this.uiButton1.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiButton1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.uiButton1.Click += new System.EventHandler(this.uiButton1_Click);
            // 
            // SortPic
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(299, 706);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.uiLabel1);
            this.Controls.Add(this.uiImageListBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SortPic";
            this.Text = "图片排序";
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Sunny.UI.UIImageListBox uiImageListBox1;
        private Sunny.UI.UILabel uiLabel1;
        private System.Windows.Forms.Panel panel2;
        private Sunny.UI.UIButton uiButton2;
        private Sunny.UI.UIButton uiButton1;
    }
}