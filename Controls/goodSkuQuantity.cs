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
    public partial class goodSkuQuantity : UserControl
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

        public string SkuName
        {
            get { return uiLabel2.Text; }
            set { uiLabel2.Text = value; }
        }

        public int Quantity
        {
            get { return uiIntegerUpDown1.Value; }
            set { uiIntegerUpDown1.Value = value; }
        }

        public goodSkuQuantity()
        {
            InitializeComponent();
        }

        private skuList _sku;
        public skuList Sku
        { get { return _sku; } set { _sku = value; } }

        private Mallinfo _mallinfo;
        public Mallinfo mallinfo
        { get { return _mallinfo; } set { _mallinfo = value; } }
    }
}
