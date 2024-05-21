namespace jingkeyun.Controls
{
    partial class goodAttr
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtName = new Sunny.UI.UILabel();
            this.ddlSelect = new Sunny.UI.UIComboBox();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.txtName.Location = new System.Drawing.Point(72, 15);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(90, 23);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "品牌";
            this.txtName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ddlSelect
            // 
            this.ddlSelect.DataSource = null;
            this.ddlSelect.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            this.ddlSelect.FillColor = System.Drawing.Color.White;
            this.ddlSelect.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ddlSelect.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ddlSelect.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(216)))), ((int)(((byte)(241)))));
            this.ddlSelect.ItemRectColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(58)))), ((int)(((byte)(183)))));
            this.ddlSelect.ItemSelectBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(58)))), ((int)(((byte)(183)))));
            this.ddlSelect.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ddlSelect.Location = new System.Drawing.Point(169, 8);
            this.ddlSelect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ddlSelect.MinimumSize = new System.Drawing.Size(63, 0);
            this.ddlSelect.Name = "ddlSelect";
            this.ddlSelect.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.ddlSelect.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(58)))), ((int)(((byte)(183)))));
            this.ddlSelect.ScrollBarBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(242)))), ((int)(((byte)(251)))));
            this.ddlSelect.ScrollBarColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(58)))), ((int)(((byte)(183)))));
            this.ddlSelect.ScrollBarStyleInherited = false;
            this.ddlSelect.Size = new System.Drawing.Size(212, 32);
            this.ddlSelect.Style = Sunny.UI.UIStyle.Custom;
            this.ddlSelect.SymbolSize = 24;
            this.ddlSelect.TabIndex = 12;
            this.ddlSelect.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.ddlSelect.Watermark = "请选择";
            // 
            // goodAttr
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ddlSelect);
            this.Controls.Add(this.txtName);
            this.Name = "goodAttr";
            this.Size = new System.Drawing.Size(455, 50);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UILabel txtName;
        private Sunny.UI.UIComboBox ddlSelect;
    }
}
