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
    public partial class goodSkuPrice : UserControl
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
                uiLabel3.Visible = true;
                uiDoubleUpDown3.Visible = true;
            }
        }

        public long Good_id { get; set; }

        public string SkuName
        {
            get { return uiLabel2.Text; }
            set { uiLabel2.Text = value; }
        }
        /// <summary>
        /// 单买价
        /// </summary>
        public double price
        {
            get { return uiDoubleUpDown2.Value; }
            set { uiDoubleUpDown2.Value = value;}
        }
        /// <summary>
        /// 团购价
        /// </summary>
        public double mPrice
        {
            get { return uiDoubleUpDown1.Value;}
            set { uiDoubleUpDown1.Value = value;}
        }
        public double market_price
        {
            get { return uiDoubleUpDown3.Value; }
            set { uiDoubleUpDown3.Value = value; 
            }
        }

        public goodSkuPrice()
        {
            InitializeComponent();
        }

        private skuList _sku;
        public skuList Sku
        { get { return _sku; } set { _sku = value; } }

        private Mallinfo _mallinfo;
        public Mallinfo mallinfo
        {  get { return _mallinfo; } set { _mallinfo = value; } }
    }
}
