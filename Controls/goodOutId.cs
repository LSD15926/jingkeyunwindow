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
    public partial class goodOutId : UserControl
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
                textBoxX1.Visible = true;
            }
        }

        public long Good_id { get; set; }

        public string SkuName
        {
            get { return uiLabel2.Text; }
            set { uiLabel2.Text = value; }
        }

        public string outGood
        {
            get { return textBoxX1.Text; }
            set { textBoxX1.Text = value; }
        }

        public string outSku
        {
            get { return textBoxX2.Text; }
            set { textBoxX2.Text = value;}
        }
        public goodOutId()
        {
            InitializeComponent();
        }

        private skuList _sku;
        public skuList Sku
        { get { return _sku; } set { _sku = value; } }

        public Sku_listItem sku_List;

        private Mallinfo _mallinfo;
        public Mallinfo mallinfo
        {  get { return _mallinfo; } set { _mallinfo = value; } }
    }
}
