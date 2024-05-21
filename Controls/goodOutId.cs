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
                textBoxX1.Visible = true;
            }
        }
        private bool _BgColor = false;
        public bool BgColor
        {
            get { return _BgColor; }
            set
            {
                _BgColor = value;
                if (value)
                {
                    this.BackColor = Color.FromArgb(220, 210, 231);
                }
            }
        }

        public goodOutId()
        {
            InitializeComponent();
        }

        private Goods_detailModel _Good;
        public Goods_detailModel Good
        {
            get
            {
                _Good.outer_goods_id = textBoxX1.Text;
                foreach (var item in _Good.sku_list)
                {
                    item.out_sku_sn = textBoxX2.Text;
                }
                return _Good;
            }
            set
            {
                _Good = value;
                textBoxX2.Text = _Good.sku_list[0].out_sku_sn;
                textBoxX1.Text = _Good.outer_goods_id;
            }
        }
    }
}
