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
    public partial class skuQuantity : UserControl
    {

        private Sku_listItem _skuList;
        public Sku_listItem sku_ListItem 
        { 
            get { return _skuList; }
            set { 
                _skuList = value; 
                uiTextBox1.Text=value.quantity.ToString();
                foreach (var spec in value.spec)
                {
                    txtName.Text += spec.spec_name + ",";
                }
                if (txtName.Text.Length != 0)
                {
                    txtName.Text=txtName.Text.Substring(0, txtName.Text.Length - 1);
                }
            }
        }
        public int Quantity
        {
            get { return MyConvert.ToInt(uiTextBox1.Text); }
            set { uiTextBox1.Text = value.ToString(); }
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
                    this.BackColor = Color.FromArgb(240,230,249);
                }
            }
        }
        public skuQuantity()
        {
            InitializeComponent();
        }
    }
}
