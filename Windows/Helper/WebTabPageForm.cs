using jingkeyun.Pinduoduo;
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

namespace jingkeyun.Windows.Helper
{
    public partial class WebTabPageForm : UIForm
    {
        public WebTabPageForm()
        {
            InitializeComponent();
        }
        WebPageForm p1;
        public void AddTabPage(string Url,string PName)
        {
            p1 = new WebPageForm();
            p1.WebUrl = Url;
            p1.Dock = DockStyle.Fill;
            this.Text = PName;
            this.AddPage(p1);
        }

        private void WebTabPageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(p1!=null)
            {
                p1.BefClose();
            }
        }
    }
}
