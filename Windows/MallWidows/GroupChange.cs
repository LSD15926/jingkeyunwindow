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
    public partial class GroupChange : UIForm
    {
        private string _Group;
        public string Group
        {
            get { return _Group; }
            set {  _Group = value; 
                int i= ddlMall.FindString(value);
                if (i >= 0)
                {
                    ddlMall.SelectedIndex = i;
                }
            }
        }
        public GroupChange()
        {
            InitializeComponent();
            InitInfo();
            InitMall();
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
        List<Mallinfo> mallinfos=new List<Mallinfo>();
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
            foreach (var mallGroup in mallGroups)
            {
                item = new Class.ComboBoxItem();
                item.Text=mallGroup.group_name;
                item.Value=mallGroup.group_id;
                ddlMall.Items.Add(item);
            }
            if(ddlMall.SelectedIndex==-1 && ddlMall.Items.Count>0)
                ddlMall.SelectedIndex = 0;
        }

        private void ReadData()
        {
            列表.Rows.Clear();
            if (mallinfos.Count > 0)
            {
                for (int i = 0; i < mallinfos.Count; i++)
                {
                    bool isChoose = false;
                    isChoose = mallinfos[i].mall_group.Split(',').ToList().Contains((ddlMall.SelectedItem as ComboBoxItem).Value.ToString());
                    列表.Rows.Add(isChoose, mallinfos[i].mall_id, mallinfos[i].mall_name, MyConvert.StampToTime(mallinfos[i].mall_token_expire) , mallinfos[i]);
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
            Mallinfo mallinfo = new Mallinfo();
            string groupId = (ddlMall.SelectedItem as ComboBoxItem).Value.ToString();
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                mallinfo = 列表.Rows[i].Cells["model"].Value as Mallinfo;
                var group = mallinfo.mall_group.Split(',').ToList();
                if (Convert.ToBoolean(列表.Rows[i].Cells["check"].Value))
                {
                    if (group.Contains(groupId))
                        continue;
                    else
                        mallinfo.mall_group = mallinfo.mall_group == "" ? groupId : mallinfo.mall_group + "," + groupId;
                }
                else
                {
                    if (group.Contains(groupId))//存在更新数据
                    {
                        group.Remove(groupId);
                        mallinfo.mall_group = string.Join(",", group);
                    }
                    else
                        continue;
                }
                BackMsg msg=Mall_Info.Upd(mallinfo);
                if (msg.Code != 0)
                {
                    MyMessageBox.ShowError("修改分组出错！"+msg.Mess);
                    return;
                }
            }
            MyMessageBox.ShowSuccess("修改成功！");
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
