using jingkeyun.Class;
using jingkeyun.Data;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace jingkeyun.Windows
{
    public partial class GroupDelete : UIForm
    {
        public GroupDelete()
        {
            InitializeComponent();
            InitInfo();
            InitMall();
            InitMyStyle();
        }
        List<Mallinfo> mallinfos = new List<Mallinfo>();
        private void InitInfo()
        {
            BackData ReadServerData = Mall_Info.get((int)InitUser.User.UserId);
            if (ReadServerData.Code != 0)
            {
                MyMessageBox.Show("获取店铺失败！");
                return;
            }
            var json = JsonConvert.SerializeObject(ReadServerData.Data);
            mallinfos = JsonConvert.DeserializeObject<List<Mallinfo>>(json);
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = Color.FromArgb(137, 113, 179);

            panel3.BackColor = this.TitleColor;

            uiButton1.StyleCustomMode = true;
            uiButton1.Style = UIStyle.Custom;
            uiButton1.FillColor = Color.FromArgb(119, 40, 245);

            uiButton2.StyleCustomMode = true;
            uiButton2.Style = UIStyle.Custom;
            uiButton2.FillColor = Color.FromArgb(184, 134, 248);
        }

        private void InitMall()
        {
            BackData backData = new BackData();
            backData = Mall_Group.List(InitUser.User.UserId);
            if (backData.Code != 0)
            {
                MyMessageBox.ShowError("获取店铺分组出错！");
                return;
            }
            var json = JsonConvert.SerializeObject(backData.Data);
            List<MallGroup> mallGroups = JsonConvert.DeserializeObject<List<MallGroup>>(json);
            列表.Rows.Clear();
            if (mallGroups.Count > 0)
            {
                for (int i = 0; i < mallGroups.Count; i++)
                {
                    列表.Rows.Add(false, mallGroups[i].group_name, mallGroups[i]);
                }
            }
            列表.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_RowPostPaint);
        }

        private void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            try
            {
                DataGridView dgv = sender as DataGridView;
                Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                                                    e.RowBounds.Location.Y,
                                                    dgv.RowHeadersWidth - 4,
                                                    e.RowBounds.Height);


                TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                                        dgv.RowHeadersDefaultCellStyle.Font,
                                        rectangle,
                                        dgv.RowHeadersDefaultCellStyle.ForeColor,
                                        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
            catch (Exception ex)
            {

            }
        }
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if(!MyMessageBox.ShowAsk("是否删除选中的分组？"))
                return;
            List<string> Ids=getCheck();
            string Sql = "delete u_mall_group where group_id in ("+string.Join(",",Ids)+")";
                        
            BackMsg backMsg = Quick_Sql.upd(Sql);
            if (backMsg.Code == 0)
            {
                UIMessageTip.ShowOk("删除成功！");
                //更新选中店铺
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MyMessageBox.ShowError("删除失败！" + backMsg.Mess);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                列表.Rows[i].Cells["check"].Value = checkBox1.Checked;
            }
        }
        /// <summary>
        /// 获取选中
        /// </summary>
        private List<string> getCheck()
        {
            List<string> mallId = new List<string>();
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                if (Convert.ToBoolean(列表.Rows[i].Cells["check"].Value))
                {
                    mallId.Add((列表.Rows[i].Cells["model"].Value as MallGroup).group_id.ToString());
                }
            }
            return mallId;
        }

        private void 列表_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;
            DataGridViewCheckBoxCell ifcheck = (DataGridViewCheckBoxCell)列表.Rows[e.RowIndex].Cells["check"];
            ifcheck.Value = !Convert.ToBoolean(ifcheck.Value);
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
    }
}
