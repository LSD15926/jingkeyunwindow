using Pdd_Models;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Controls
{
    public partial class skuName : UserControl
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

        public SpecItem specItem { get; set; }
        public string NewTitle 
        { 
            get { return uiTextBox1.Text; }
            set { uiTextBox1.Text = value; } 
        }
        public skuName()
        {
            InitializeComponent();
        }

        private Mallinfo _mallinfo;
        public Mallinfo mallinfo
        { get { return _mallinfo; } set { _mallinfo = value; } }
    }
}
