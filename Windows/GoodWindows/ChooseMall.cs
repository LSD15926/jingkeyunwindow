using jingkeyun.Class;
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
using jingkeyun.Data;
using MoreLinq;
using Pdd_Models.Models;

namespace jingkeyun.Windows
{
    public partial class ChooseMall : UIForm
    {
        public ChooseMall()
        {
            InitializeComponent();
            InitMall();
            //ReadData();
            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;

            panel3.BackColor = this.TitleColor;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
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
            Class.ComboBoxItem item = new Class.ComboBoxItem();
            item.Text = "所有店铺";
            item.Value = 0;
            ddlMall.Items.Add(item);
            foreach (var mallGroup in mallGroups)
            {
                item = new Class.ComboBoxItem();
                item.Text=mallGroup.group_name;
                item.Value=mallGroup.group_id;
                ddlMall.Items.Add(item);
            }
            if(ddlMall.SelectedIndex==-1)
                ddlMall.SelectedIndex = 0;
        }

        private void ReadData()
        {
            列表.Rows.Clear();
            if (InitUser.All_mall.Count > 0)
            {
                List<Mallinfo> GroupMall = new List<Mallinfo>();
                if (ddlMall.SelectedIndex == 0)
                {
                    GroupMall = InitUser.All_mall;
                }
                else
                {
                    GroupMall= InitUser.All_mall.Where(x => x.mall_group.Split(',').ToList().Contains((ddlMall.SelectedItem as ComboBoxItem).Value.ToString())).ToList();
                }
                for (int i = 0; i < GroupMall.Count; i++)
                {
                    bool isChoose = false;
                    isChoose = InitUser.Choose_mall.FindIndex(x => x.mall_id == GroupMall[i].mall_id) >= 0;
                    列表.Rows.Add(isChoose,  GroupMall[i].mall_id,GroupMall[i].mall_name, MyConvert.StampToTime(GroupMall[i].mall_token_expire) , GroupMall[i]);
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

            if (getCheck() == 0)
            {
                MyMessageBox.ShowError("至少需选中一个店铺！");
                return;
            }
            //更新选中店铺
            this.DialogResult = DialogResult.OK;
            this.Close();
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
        private int getCheck()
        {
            List<Mallinfo> mall = new List<Mallinfo>();
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                if (Convert.ToBoolean(列表.Rows[i].Cells["check"].Value))
                {
                    mall.Add(列表.Rows[i].Cells["model"].Value as Mallinfo);
                }
            }
            if (mall.Count > 0)
            {
                InitUser.Choose_mall = mall;
            }
            return mall.Count;
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

        private void ddlMall_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReadData();
        }
    }
}
