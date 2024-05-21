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
    public partial class goodTitleOnly : UserControl
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
            get { return textBox1.Text; }
            set
            {
                textBox1.Text = value;
            }
        }

        private long _Good_id;
        public long Good_id { 
            get
            {
                return _Good_id;
            } 
            set 
            { 
                _Good_id = value;
                uiLabel2.Text = "ID:" + value;
            } 
        }

        public goodTitleOnly()
        {
            InitializeComponent();
        }

        private Mallinfo _mallinfo;
        public Mallinfo mallinfo
        { 
            get { return _mallinfo; } 
            set { _mallinfo = value; } 
        }
    }
}
