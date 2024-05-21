using jingkeyun.Class;
using jingkeyun.Controls;
using jingkeyun.Pinduoduo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun
{
    public partial class Form2 : FormBase
    {
        public Form2()
        {
            InitializeComponent();
        }

        private async void uiButton1_Click(object sender, EventArgs e)
        {
            MyMessageBox.ShowLoading(this);

        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            Form1 form3 = new Form1();
            form3.Show();
        }
    }
}
