using BsPhpHelper;
using jingkeyun.Class;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Data;

namespace jingkeyun
{
    public partial class DialogForm : UIForm
    {
        public DialogForm()
        {
            InitializeComponent();

            this.TitleColor = StyleHelper.Title;
            uiLabel1.Font = new Font(FontHelper.pfc.Families[0], 12);
            this.panel1.BackColor=StyleHelper.Title;
            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
        }

        private string _Msg = "";
        public string Msg
        {
            get { return _Msg; }
            set { _Msg = value; 
               uiLabel1.Text= _Msg;
            }
        }

        private bool _IsCancel=false;
        public bool IsCancel
        {
            get { return _IsCancel; }
            set { 
                _IsCancel = value; 
                uiButton2.Visible=value;
            }
        }

        private Image _image= global::jingkeyun.Properties.Resources.tishi;
        public Image Image
        {
            get { return _image; }
            set { _image = value;
                this.pictureBox1.Image = value;
            }
        }


        private void uiButton1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
