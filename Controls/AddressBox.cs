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
    public partial class AddressBox : UserControl
    {
        public AddressBox()
        {
            InitializeComponent();
        }

        private string _MainMsg = "";
        /// <summary>
        /// 收货人信息
        /// </summary>
        public string MainMsg
        { 
            get { return _MainMsg; } 
            set { _MainMsg = value; 
            label1.Text = _MainMsg;
            } 
        }

        private bool _IsChecked = false;
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsChecked
        { 
            get { return _IsChecked; }
            set { _IsChecked = value;
                if (_IsChecked)
                {
                    checkBox1.Checked = true;
                    checkBox1.ForeColor = Color.Red;
                    checkBox1.Text = "已设默认";
                }
            } 
        }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string DetailedAddress = "";

        private string _addressId = "";
        /// <summary>
        /// 收货信息
        /// </summary>
        public string AddressId
        { get { return _addressId; }
        set { _addressId = value; } }

        private void button1_Click(object sender, EventArgs e)
        {
            //Form1 form = new Form1();
            //form.addressId = _addressId;
            //form.textBox1.Text = DetailedAddress;
            //form.ShowDialog();
            ////刷新信息
            //(this.ParentForm as Form2).GetAddress();


        }
    }
}
