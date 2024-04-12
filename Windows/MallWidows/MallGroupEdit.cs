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
using jingkeyun.Class;
using jingkeyun.Data;
using MoreLinq;
using Pdd_Models.Models;
using APIOffice.pddApi;
using jingkeyun.Data;

namespace jingkeyun.Windows
{
    public partial class MallGroupEdit : UIForm
    {


        public string Type = "Add";
        public MallGroupEdit()
        {
            InitializeComponent();
            InitMyStyle();
            ReadData();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = Color.FromArgb(137, 113, 179);


            uiPanel1.StyleCustomMode = true;
            uiPanel1.Style= Sunny.UI.UIStyle.Custom;
            uiPanel1.FillColor = this.TitleColor;

            uiButton1.StyleCustomMode = true;
            uiButton1.Style = UIStyle.Custom;
            uiButton1.FillColor = Color.FromArgb(119, 40, 245);

            uiButton2.StyleCustomMode = true;
            uiButton2.Style = UIStyle.Custom;
            uiButton2.FillColor = Color.FromArgb(184, 134, 248);

        }
        private void ReadData()
        {
            BackData ReadServerData = Mall_Info.get((int)InitUser.User.UserId);
            if (ReadServerData.Code != 0)
            {
                UIMessageBox.Show("获取店铺失败！");
                return;
            }
            var json=JsonConvert.SerializeObject(ReadServerData.Data);
            List<Mallinfo> mallinfos = JsonConvert.DeserializeObject<List<Mallinfo>>(json);
            列表.Rows.Clear();
            if (mallinfos.Count > 0)
            {
                List<string> picUrl = new List<string>();
                for (int i = 0; i < mallinfos.Count; i++)
                {
                    列表.Rows.Add();
                    int NewRow = 列表.Rows.Count - 1;

                    列表.Rows[NewRow].Cells["Mall_Name"].Value = mallinfos[i].mall_name;
                    列表.Rows[NewRow].Cells["Mall_Id"].Value = mallinfos[i].mall_id;

                    DateTime dt = MyConvert.StampToTime(mallinfos[i].mall_token_expire);
                    列表.Rows[NewRow].Cells["Expire"].Value = dt.ToString("yyyy-MM-dd HH:mm");
                    if (dt > DateTime.Now)
                    {

                    }
                    else
                    {
                        列表.Rows[NewRow].Cells["Expire"].Value = dt.ToString("yyyy-MM-dd HH:mm") + "(已过期)";
                        列表.Rows[NewRow].Cells["Expire"].Style.ForeColor = Color.Red;
                        列表.Rows[NewRow].Cells["Expire"].Style.SelectionForeColor = Color.Red;
                    }

                    列表.Rows[NewRow].Cells["model"].Value = mallinfos[i];
                    picUrl.Add(mallinfos[i].logo);

                }
                Parallel.For(0, picUrl.Count, i =>
                {
                    try
                    {
                        Image image = Image.FromStream(System.Net.WebRequest.Create(picUrl[i]).GetResponse().GetResponseStream());
                        列表.Rows[i].Cells["pic"].Value = image;
                    }
                    catch
                    {
                    }
                });
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
            if (string.IsNullOrEmpty(txtName.Text))
            {
                UIMessageBox.Show("请输入分组名称！");
                return;
            }
            if (UIMessageBox.ShowAsk("是否确认提交？"))
            {
                BackMsg backMsg = new BackMsg();
                //新增一条数据
                if (Type == "Add")
                {
                    MallGroup group = new MallGroup();
                    group.group_user = InitUser.User.UserId;
                    group.group_name = txtName.Text;
                    group.group_notes = txtNotes.Text;
                    backMsg = Mall_Group.Add(group);
                    if (backMsg.Code != 0)
                    {
                        UIMessageBox.ShowError("新增分组失败！");
                        return;
                    }
                    //获取选中数据
                    if (getCheck() > 0)
                    {
                        //分配店铺分组
                        string Sql = $"update u_mall set mall_group=(case mall_group when '' then  '{backMsg.Mess}' else mall_group+',{backMsg.Mess}' end)  where id in (" + string.Join(",", mallId) + ")";
                        backMsg = Quick_Sql.upd(Sql);
                        if (backMsg.Code != 0)
                        {
                            UIMessageBox.ShowError("分配分组失败！");
                            return;
                        }
                        else
                        {
                            UIMessageBox.ShowSuccess("新增成功！");
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            return;
                        }
                    }
                    UIMessageBox.ShowSuccess("新增成功！");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;

                }
                else
                {

                }
            }
        }

        List<string> mallId = new List<string>();
        private int getCheck()
        {

            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                if (Convert.ToBoolean(列表.Rows[i].Cells["check"].Value))
                {
                    mallId.Add((列表.Rows[i].Cells["model"].Value as Mallinfo).id.ToString());
                }
            }
            return mallId.Count;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                列表.Rows[i].Cells["check"].Value = checkBox1.Checked;
            }
        }

        private void 列表_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1) return;
            DataGridViewCheckBoxCell ifcheck = (DataGridViewCheckBoxCell)列表.Rows[e.RowIndex].Cells["check"];
            ifcheck.Value = !Convert.ToBoolean(ifcheck.Value);
        }
    }
}
