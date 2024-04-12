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
    public partial class goodTitle : UserControl
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
        public string NewTitle 
        { 
            get { return uiTextBox1.Text; }
            set { uiTextBox1.Text = value; } 
        }
        public goodTitle()
        {
            InitializeComponent();
        }

        private Mallinfo _mallinfo;
        public Mallinfo mallinfo
        { get { return _mallinfo; } set { _mallinfo = value; } }
    }
}
