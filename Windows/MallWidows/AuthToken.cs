using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Class;
using jingkeyun.Data;

namespace jingkeyun.Windows
{
    public partial class AuthToken : UIForm
    {
        public AuthToken()
        {
            InitializeComponent();
            InitMyStyle();
        }

        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;


            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);


        }



        public string access_token = "";
        public long expires_at = 0;
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(uiTextBox1.Text))
            {
                MyMessageBox.ShowError("请输入code！");
                return;
            }
            BackMsg backMsg = new BackMsg();
            backMsg=Auth_Token.Get(uiTextBox1.Text.Trim());
            if (backMsg.Code == 0)
            {
                JToken jToken = JsonConvert.DeserializeObject<JToken>(backMsg.Mess);
                if (jToken != null)
                {
                    access_token = jToken["access_token"].ToString();
                    expires_at = MyConvert.ToLong(jToken["expires_at"]);
                    this.DialogResult=DialogResult.OK;
                    this.Close();
                }
            }
            else
            {
                MyMessageBox.ShowError("获取授权失败!"+backMsg.Mess);
                return;
            }
        }
    }
}
