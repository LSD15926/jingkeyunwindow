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
        public LoadingForm(Form father)
        {
            InitializeComponent();
            this.Size = father.Size;
            UIProgressIndicator uiProgressIndicator1 = new UIProgressIndicator();
            uiProgressIndicator1.Active = true;
            uiProgressIndicator1.BackColor = System.Drawing.Color.White;
            uiProgressIndicator1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            uiProgressIndicator1.Location = new Point(this.Width / 2 - 75, this.Height / 2 - 75);
            uiProgressIndicator1.MinimumSize = new System.Drawing.Size(1, 1);
            uiProgressIndicator1.Name = "uiProgressIndicator1";
            uiProgressIndicator1.Size = new System.Drawing.Size(150, 150);
            uiProgressIndicator1.Style = Sunny.UI.UIStyle.Custom;
            uiProgressIndicator1.TabIndex = 0;
            uiProgressIndicator1.Text = "uiProgressIndicator1";

            this.Controls.Add(uiProgressIndicator1);
            uiProgressIndicator1.Location = new Point(this.Width / 2 - 75, this.Height / 2 - 75);

        }
    }
}
