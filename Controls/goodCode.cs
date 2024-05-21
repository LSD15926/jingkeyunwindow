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
    public partial class goodCode : UserControl
    {

        private Sku_listItem _sku;

        public Sku_listItem Sku
        {
            get { return _sku; }
            set { _sku = value; 
            }
        }
        private string _SkuName;
        public string SkuName
        {
            get { return _SkuName; }
            set { 
                _SkuName = value;
                txtName.Text = value;   
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
        private string _OutSku;
        public string OutSku
        {
            get => _OutSku;
            set { _OutSku = value;
                uiTextBox1.Text = value;
            }
        }

        public goodCode()
        {
            InitializeComponent();
        }
    }
}
