using APIOffice.pddApi;
using jingkeyun.Class;
using jingkeyun.Data;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Class;
using jingkeyun.Data;
using jingkeyun.Windows;

namespace jingkeyun.Pinduoduo
{
    public partial class mailList : UIPage
    {
        
        public event UpdateLogDelegate UpdLog;
        Thread List = null;
        public mailList()
        {
            InitializeComponent();


        }
        private void mailList_Load(object sender, EventArgs e)
        {
            //ReadData();

            InitGroup();
        }
        List<MallGroup> mallGroups = new List<MallGroup>();
        List<Mallinfo> mallAll=new List<Mallinfo>();
        private void InitGroup()
        {
            //获取所有店铺分组
            BackData backData = new BackData();
            backData = Mall_Group.List(InitUser.User.UserId);
            if (backData.Code != 0)
            {
                UIMessageBox.ShowError("获取店铺分组出错！");
                return;
            }
            else
            {
                menuPanel.Controls.Clear();
                mallGroups.Clear();
                var json = JsonConvert.SerializeObject(backData.Data);
                mallGroups = JsonConvert.DeserializeObject<List<MallGroup>>(json);
                if (mallGroups != null)
                {
                    foreach (MallGroup group in mallGroups)
                    {

                        int cnt = mallAll.Where(x => x.mall_group.Split(',').ToList().Contains(group.group_id.ToString())).ToList().Count();

                        UILabel label1 = new UILabel();
                        label1.Text = group.group_name + "(" + cnt + ")";
                        label1.Dock = DockStyle.Top;
                        label1.BackColor = System.Drawing.Color.Transparent;
                        label1.Cursor = System.Windows.Forms.Cursors.Hand;
                        label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        label1.ForeColor = System.Drawing.Color.Black;
                        label1.MinimumSize = new System.Drawing.Size(1, 1);
                        label1.Size = new System.Drawing.Size(159, 50);
                        label1.Style = Sunny.UI.UIStyle.Custom;
                        label1.StyleCustomMode = true;
                        label1.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
                        label1.Click += Label1_Click;
                        //label1.MouseDown += Label1_MouseDown;
                        label1.Tag = group.group_id;
                        menuPanel.Controls.Add(label1);
                    }
                }
                //首先添加所有店铺
                UILabel label = new UILabel();
                label.Text = "所有店铺(" + mallAll.Count + ")";
                label.Dock = DockStyle.Top;
                label.BackColor = System.Drawing.Color.Transparent;
                label.Cursor = System.Windows.Forms.Cursors.Hand;
                label.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                label.ForeColor = System.Drawing.Color.Black;
                label.MinimumSize = new System.Drawing.Size(1, 1);
                label.Size = new System.Drawing.Size(159, 50);
                label.Style = Sunny.UI.UIStyle.Custom;
                label.StyleCustomMode = true;
                label.ZoomScaleRect = new System.Drawing.Rectangle(0, 0, 0, 0);
                label.Click += Label1_Click;
                menuPanel.Controls.Add(label);
                actLabel = label;
                actLabel.ForeColor = Color.Yellow;
                actLabel.BackColor = Color.FromArgb(143, 80, 244);
                Label1_Click(label, null);
            }
        }

        //选中操作的分组
        UILabel changeLabel = null;
        private void Label1_MouseDown(object sender, MouseEventArgs e)
        {
            //if (MouseButtons.Right == e.Button)//右键显示右键菜单
            //{
            //    changeLabel = sender as UILabel;
            //    分组菜单.Show(MousePosition.X, MousePosition.Y);
            //}
        }

        UILabel actLabel = null;
        //点击切换店铺类型
        private void Label1_Click(object sender, EventArgs e)
        {
            UILabel label = sender as UILabel;
            if (label != actLabel)
            {
                actLabel.BackColor = System.Drawing.Color.Transparent;
                actLabel.ForeColor = System.Drawing.Color.Black;
                actLabel = label;
                actLabel.ForeColor = Color.Yellow;
                actLabel.BackColor = Color.FromArgb(143, 80, 244);
            }
            ReadData();
        }

        bool ReadServerDataState = false;
        BackData ReadServerData = new BackData();
        bool flag = true;
        private void ReadData()
        {
            UpdLog("开始读取店铺！");
            timer1.Start();
            ReadServerDataState = false;
            uiProgressIndicator1.Visible = true;
            List = new Thread(new ThreadStart(ListData));
            List.IsBackground = true;
            List.Start();
        }

        private void ListData()
        {
            BackData pageListData = new BackData();
            //获取店铺列表数据
            ReadServerData = Mall_Info.get((int)InitUser.User.UserId);

            ReadServerDataState = true;

        }
        private void ShowData()
        {
            if (ReadServerData.Code != 0)
            {
                timer1.Stop();
                UIMessageBox.ShowError(ReadServerData.Mess);
                uiProgressIndicator1.Visible = false;
                return;
            }
            mallAll.Clear();
            var json = JsonConvert.SerializeObject(ReadServerData.Data);
            List<Mallinfo> Mallinfo = JsonConvert.DeserializeObject<List<Mallinfo>>(json);
            if (!string.IsNullOrEmpty(textBoxX1.Text))
            {
                Mallinfo = Mallinfo.Where(x => x.mall_name.Contains(textBoxX1.Text.Trim())).ToList();
            }

            mallAll.AddRange(Mallinfo);
            foreach (var control in menuPanel.Controls)
            {
                if (control.GetType().Name != "UILabel")
                {
                    continue;
                }
                UILabel label = control as UILabel;
                int cnt = 0; 
                if (label.Tag == null)
                {
                    cnt = mallAll.Count();
                }
                else
                    cnt= mallAll.Where(x => x.mall_group.Split(',').ToList().Contains(label.Tag.ToString())).ToList().Count();
                label.Text = label.Text.Substring(0, label.Text.IndexOf("(")) + "(" + cnt + ")";
            }

            bool IsAll=false;
            if (actLabel.Tag != null)
            {
                Mallinfo = Mallinfo.Where(x => x.mall_group.Split(',').ToList().Contains(actLabel.Tag.ToString())).ToList();
            }
            else
            {
                InitUser.All_mall.Clear();
                IsAll=true;
            }
            this.列表.Rows.Clear();



            List<string> picUrl = new List<string>();
            for (int i = 0; i < Mallinfo.Count; i++)
            {
                列表.Rows.Add();
                列表.Rows[i].Cells["操作"].Value = "删 除";
                列表.Rows[i].Cells["操作"].Style.ForeColor = Color.Red;
                列表.Rows[i].Cells["操作"].Style.SelectionForeColor = Color.Red;
                列表.Rows[i].Cells["Mall_Name"].Value = Mallinfo[i].mall_name;
                列表.Rows[i].Cells["Mall_Id"].Value = Mallinfo[i].mall_id;
                if (Mallinfo[i].mall_token_expire == 0)
                {
                    列表.Rows[i].Cells["state"].Value = "未授权";
                }
                else
                {
                    DateTime dt = MyConvert.StampToTime(Mallinfo[i].mall_token_expire);
                    列表.Rows[i].Cells["Expire"].Value = dt.ToString("yyyy-MM-dd HH:mm");
                    if (dt > DateTime.Now)
                    {
                        列表.Rows[i].Cells["state"].Value = "已授权";
                        列表.Rows[i].Cells["state"].Style.ForeColor = Color.Green;
                        列表.Rows[i].Cells["state"].Style.SelectionForeColor = Color.Green;
                        if (IsAll)
                        {
                            InitUser.All_mall.Add(Mallinfo[i]);
                        }
                    }
                    else
                    {
                        列表.Rows[i].Cells["state"].Value = "授权过期";
                        列表.Rows[i].Cells["state"].Style.ForeColor = Color.Red;
                        列表.Rows[i].Cells["state"].Style.SelectionForeColor = Color.Red;
                        列表.Rows[i].Cells["Expire"].Style.ForeColor = Color.Red;
                        列表.Rows[i].Cells["Expire"].Style.SelectionForeColor = Color.Red;
                    }
                }
                //var a = mallGroups.FirstOrDefault(x => x.group_id == Mallinfo[i].mall_group);
                //if (a != null)
                //{
                //    列表.Rows[i].Cells["MallGroup"].Value = a.group_name;
                //}

                列表.Rows[i].Cells["model"].Value = Mallinfo[i];
                列表.Rows[i].Cells["Mall_Type"].Value = Enum.GetName(typeof(MallTypes), Mallinfo[i].merchant_type);  //GetType MallTypes
                picUrl.Add(Mallinfo[i].logo);
            }
            uiProgressIndicator1.Visible = false;
            列表.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_RowPostPaint);
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


            if (InitUser.All_mall.Count > 0 && flag)
            {
                flag = false;
                Task task = Task.Run(() =>
                {
                    //偷跑一次数据
                    var ws = Stopwatch.StartNew();
                    requestGoodList requestGoodList = new requestGoodList();
                    requestGoodList.Malls = InitUser.All_mall[0];
                    requestGoodList.page_size = 1;
                    BackData backData = Good_List.Get(requestGoodList);
                    ws.Stop();
                    //UIMessageTip.Show(ws.ElapsedMilliseconds.ToString());
                });
            }
            UpdLog($"成功读取{列表.Rows.Count}个店铺！");
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ReadServerDataState)
            {
                ShowData();
                timer1.Stop();
            }
        }
        List<string> mallIds = new List<string>();
        /// <summary>
        /// 获取选中
        /// </summary>
        private int getCheck()
        {
            mallIds.Clear();
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                if (Convert.ToBoolean(列表.Rows[i].Cells["check"].Value))
                {
                    mallIds.Add(列表.Rows[i].Cells["Mall_Id"].Value.ToString());
                }
            }
            return mallIds.Count;
        }


        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (getCheck() == 0)
            {
                UIMessageTip.Show("请选择要删除的店铺!");
                return;
            }
            //删除店铺
            if (UIMessageBox.ShowAsk("是否确认删除选中的店铺？"))
            {
                //执行删除
                UpdLog("执行删除店铺！");
                BackMsg backMsg = Mall_Info.del(string.Join(",", mallIds));
                if (backMsg.Code == 0)
                {
                    UIMessageTip.ShowOk("删除成功！");
                    UpdLog($"成功删除{mallIds.Count}个店铺！");
                    Label1_Click(menuPanel.Controls[menuPanel.Controls.Count - 1], null);
                }
                else
                {
                    UIMessageBox.ShowError("删除失败！" + backMsg.Mess);
                    UpdLog("删除失败！" + backMsg.Mess);
                }
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            BrowserHelper.OpenBrowserUrl("https://fuwu.pinduoduo.com/service-market/service-detail?detailId=75585");
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            UpdLog("开始店铺授权！");
            AuthToken authToken = new AuthToken();
            if (authToken.ShowDialog() == DialogResult.OK)
            {
                //获取商家信息
                BackMsg backMsg = Mall_Info.Info(authToken.access_token);
                if (backMsg.Code == 0)
                {
                    Mallinfo model = JsonConvert.DeserializeObject<Mallinfo>(backMsg.Mess);
                    model.mall_token = authToken.access_token;
                    model.mall_token_expire = authToken.expires_at;
                    model.user_id = InitUser.User.UserId;
                    //添加至数据库
                    backMsg = Mall_Info.Upd(model);
                    if (backMsg.Code == 0)
                    {
                        //刷新数据
                        UIMessageTip.ShowOk("授权成功！");

                        Label1_Click(menuPanel.Controls[menuPanel.Controls.Count - 1], null);
                    }
                    else
                    {
                        UIMessageBox.ShowError("保存商家信息失败！" + backMsg.Mess);
                        UpdLog("保存商家信息失败！" + backMsg.Mess);
                        return;
                    }
                }
                else
                {
                    UIMessageBox.ShowError("获取商家信息失败！" + backMsg.Mess);
                    UpdLog("保存商家信息失败！" + backMsg.Mess);
                    return;
                }
                UpdLog("授权成功！");
            }
            else
            {
                UpdLog("用户取消授权。");
            }
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                列表.Rows[i].Cells["check"].Value = checkBox1.Checked;

            }
        }

        private void 列表_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView Obj = (DataGridView)sender;
                Obj.ClearSelection();
                Obj.Rows[e.RowIndex].Selected = true;
                Obj.CurrentCell = Obj.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (e.Button == MouseButtons.Left)
                {
                    switch (e.ColumnIndex)
                    {
                        case 1:
                            if (UIMessageBox.ShowAsk("是否确认删除'" + Obj.Rows[e.RowIndex].Cells["Mall_Name"].Value + "'？"))
                            {
                                //执行删除

                                BackMsg backMsg = Mall_Info.del(Obj.Rows[e.RowIndex].Cells["Mall_Id"].Value.ToString());
                                if (backMsg.Code == 0)
                                {
                                    UIMessageTip.ShowOk("删除成功！");
                                    ReadData();
                                }
                                else
                                {
                                    UIMessageBox.ShowError("删除失败！" + backMsg.Mess);
                                }
                            }
                            break;
                        default:
                            //Obj.Rows[e.RowIndex].Cells["check"].Value = !Convert.ToBoolean(Obj.Rows[e.RowIndex].Cells["check"].Value); ;
                            break;


                    }
                }

                //if (e.Button == MouseButtons.Right)
                //{
                //    uiContextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                //}
            }
        }

        private void 列表_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView Obj = (DataGridView)sender;
            //数据复原，只允许复制数据，不允许修改数据
            switch (e.ColumnIndex)
            {
                case 3://店铺id
                    Obj.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (Obj.Rows[e.RowIndex].Cells["model"].Value as Mallinfo).mall_id;
                    break;
                case 4://店铺名称
                    Obj.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (Obj.Rows[e.RowIndex].Cells["model"].Value as Mallinfo).mall_name;
                    break;
                default:
                    break;
            }
        }

        private void uiSymbolButton8_Click(object sender, EventArgs e)
        {
            ReadData();
        }

        private void uiSymbolButton4_Click(object sender, EventArgs e)
        {
            //加分组
            MallGroupEdit form = new MallGroupEdit();
            if (form.ShowDialog() == DialogResult.OK)
            {
                InitGroup();
            }

        }

        private void 修改分组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroupEdit edit = new GroupEdit();
            edit.mallGroups = mallGroups.FirstOrDefault(x => x.group_id.ToString() == changeLabel.Tag.ToString());
            if (edit.ShowDialog() == DialogResult.OK)
            {
                InitGroup();
            }

        }

        private void 删除分组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 删除店铺ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UIMessageBox.ShowAsk("是否删除该店铺？"))
            {
                BackMsg backMsg = Mall_Info.del(string.Join(",", mallIds));
                if (backMsg.Code == 0)
                {
                    UIMessageTip.ShowOk("删除成功！");
                    ReadData();
                }
                else
                {
                    UIMessageBox.ShowError("删除失败！" + backMsg.Mess);
                }
            }
        }

        private void 加入分组ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var row=列表.CurrentRow;
            //修改分组
            GroupChoose choose = new GroupChoose();
            choose.Mall= row.Cells["model"].Value as Mallinfo;
            choose.mallGroups=mallGroups;
            if(choose.ShowDialog() == DialogResult.OK) {
                ReadData();
            }
        }

        private void uiSymbolButton5_Click(object sender, EventArgs e)
        {
            GroupChange f= new GroupChange();
            f.Group = actLabel.Text.Substring(0, actLabel.Text.IndexOf("("));
            if (f.ShowDialog() == DialogResult.OK)
            {
                InitGroup();
            }
        }

        private void uiSymbolButton6_Click(object sender, EventArgs e)
        {
            //GroupDelete f= new GroupDelete();
            //if (f.ShowDialog() == DialogResult.OK)
            //{
            //    InitGroup();
            //}
            if (actLabel.Tag is null)
            {
                UIMessageTip.Show("请选择店铺分组");
                return;
            }
            if (UIMessageBox.ShowAsk("是否删除该分组？"))
            {
                try
                {
                    string Sql = "delete u_mall_group where group_id=" + actLabel.Tag.ToString();

                    BackMsg backMsg = Quick_Sql.upd(Sql);
                    if (backMsg.Code == 0)
                    {
                        UIMessageTip.ShowOk("删除成功！");
                        InitGroup();
                    }
                    else
                    {
                        UIMessageBox.ShowError("删除失败！" + backMsg.Mess);
                    }
                }
                catch (Exception ex)
                {
                    UIMessageBox.ShowError("删除失败！" + ex.Message);
                }
            }
        }
    }
}
