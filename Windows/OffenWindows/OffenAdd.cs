using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Class;
using jingkeyun.Data;
using System.Data.SQLite;

namespace jingkeyun.Windows
{
    public partial class OffenAdd : UIForm
    {

        public OffenAdd()
        {
            InitializeComponent();
            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;

            panel1.BackColor = this.TitleColor;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
        }
        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click_1(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MyMessageBox.ShowError("违规词不能为空");
                return;
            }
            List<string> list = new List<string>();
            list=txtName.Text.Split("\n").ToList();
            int cnt = 0;
            foreach(string s in list)
            {
                if(s.Trim()=="")
                    continue;
                string sql = "insert into Offend(word) values(@word)";
                SQLiteParameter[] paras = new SQLiteParameter[]
                {
                         new SQLiteParameter("@word",s)
                };
                int i = SQLiteHelper.ExecuteNonQuery(sql, paras);
                if (i != -99)
                    cnt += i;
            }
            UIMessageTip.ShowOk($"成功添加{cnt}条数据");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
