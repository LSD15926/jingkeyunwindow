using APIOffice.pddApi;
using CefSharp.WinForms;
using CefSharp;
using jingkeyun.Class;
using jingkeyun.Data;
using jingkeyun.Windows;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Pinduoduo
{
    public partial class mailList : UIPage
    {

        public event UpdateLogDelegate UpdLog;
        Thread List = null;
        public mailList()
        {
            InitializeComponent();
            StyleHelper.SetSymbolButtonColor(uiSymbolButton1, StyleHelper.OkButton);
            StyleHelper.SetSymbolButtonColor(uiSymbolButton2, StyleHelper.OkButton);
            StyleHelper.SetSymbolButtonColor(uiSymbolButton3, StyleHelper.OkButton);
            StyleHelper.SetSymbolButtonColor(uiSymbolButton4, StyleHelper.OkButton);
            StyleHelper.SetSymbolButtonColor(uiSymbolButton5, StyleHelper.OkButton);
            StyleHelper.SetSymbolButtonColor(uiSymbolButton6, StyleHelper.OkButton);
            StyleHelper.SetSymbolButtonColor(uiSymbolButton8, StyleHelper.OkButton);


        }
        private void mailList_Load(object sender, EventArgs e)
        {
            //初始化列表页
            InitBrowser(); 
            InitGroup();
        }
        bool IsShown=false;

        ChromiumWebBrowser Chrome;
        JsObject_Mail jsObject = new JsObject_Mail();
        public void InitBrowser()
        {
            Chrome = new ChromiumWebBrowser(InitUser.pageUrl + "jingkeyun/mailTable.html");
            Chrome.MenuHandler = new MenuHandler();
            //Chrome.KeyboardHandler = new CEFKeyBoardHander();
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled };
            Chrome.Dock = DockStyle.Fill;
            CefSharpSettings.WcfEnabled = true;
            Chrome.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;

            jsObject.form = this;
            Chrome.JavascriptObjectRepository.Register("boundAsync", jsObject, true, BindingOptions.DefaultBinder);
            //Chrome.FrameLoadEnd += Chrome_FrameLoadEnd;
            this.panel3.Controls.Add(Chrome);
        }
        bool LoadEnd = false;
        //private void Chrome_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        //{
        //    //if(LoadEnd)
        //    //    return;
        //    //LoadEnd = true;
        //    this.BeginInvoke(new Action(() =>
        //    {
        //        InitGroup();
        //    }));
        //}

        List<MallGroup> mallGroups = new List<MallGroup>();
        List<Mallinfo> mallAll = new List<Mallinfo>();
        private void InitGroup()
        {
            //获取所有店铺分组
            BackData backData = new BackData();
            backData = Mall_Group.List(InitUser.User.UserId);
            if (backData.Code != 0)
            {
                MyMessageBox.ShowError("获取店铺分组出错！");
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
                //Label1_Click(label, null);
            }
        }

        //选中操作的分组
        UILabel changeLabel = null;
        public void showOnPage()
        {
            this.BeginInvoke(new Action(() => { IsShown = true; ReadData(); }));
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
            if(!IsShown)
                return;
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
                MyMessageBox.ShowError(ReadServerData.Mess);
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
                    cnt = mallAll.Where(x => x.mall_group.Split(',').ToList().Contains(label.Tag.ToString())).ToList().Count();
                label.Text = label.Text.Substring(0, label.Text.IndexOf("(")) + "(" + cnt + ")";
            }

            if (actLabel.Tag != null)
            {
                Mallinfo = Mallinfo.Where(x => x.mall_group.Split(',').ToList().Contains(actLabel.Tag.ToString())).ToList();
            }
            else
            {
                InitUser.All_mall.Clear();

                InitUser.All_mall = Mallinfo.Where(x => x.mall_token_expire > MyConvert.ToTimeStamp(DateTime.Now)).ToList();
            }

            jsObject.setData(Chrome, Mallinfo);

            uiProgressIndicator1.Visible = false;

            if (InitUser.All_mall.Count > 0 && flag)
            {
                flag = false;
                Task task = Task.Run(() =>
                {
                    //偷跑一次数据
                    requestGoodList requestGoodList = new requestGoodList();
                    requestGoodList.Malls = InitUser.All_mall[0];
                    requestGoodList.page_size = 1;
                    BackData backData = Good_List.Get(requestGoodList);
                });
            }
            UpdLog($"成功读取{Mallinfo.Count}个店铺！");
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
        public void getCheck(List<MallTabel> tabels)
        {
            mallIds.Clear();
            foreach (var tabel in tabels)
            {
                mallIds.Add(tabel.mall_id.ToString());
            }

        }


        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (mallIds.Count == 0)
            {
                UIMessageTip.Show("请选择要删除的店铺!");
                return;
            }
            //删除店铺
            if (MyMessageBox.ShowAsk("是否确认删除选中的店铺？"))
            {
                //执行删除
                UpdLog("执行删除店铺！"+ string.Join(",", mallIds));
                BackMsg backMsg = Mall_Info.del(string.Join(",", mallIds));
                if (backMsg.Code == 0)
                {
                    UIMessageTip.ShowOk("删除成功！");
                    UpdLog($"成功删除{mallIds.Count}个店铺！");
                    Label1_Click(menuPanel.Controls[menuPanel.Controls.Count - 1], null);
                }
                else
                {
                    MyMessageBox.ShowError("删除失败！" + backMsg.Mess);
                    UpdLog("删除失败！" + backMsg.Mess);
                }
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            CefTabPageHelper.showPage("https://fuwu.pinduoduo.com/service-market/service-detail?detailId=15385","服务购买");
            //InitUser.MainForm.F3.WebUrl = "https://fuwu.pinduoduo.com/service-market/service-detail?detailId=15385";
            //InitUser.MainForm.uiSymbolLabel1_Click(InitUser.MainForm.uiSymbolLabel7, null);
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
                        MyMessageBox.ShowError("保存商家信息失败！" + backMsg.Mess);
                        UpdLog("保存商家信息失败！" + backMsg.Mess);
                        return;
                    }
                }
                else
                {
                    MyMessageBox.ShowError("获取商家信息失败！" + backMsg.Mess);
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
                Label1_Click(actLabel, null);
            }

        }
        private void uiSymbolButton5_Click(object sender, EventArgs e)
        {
            if (menuPanel.Controls.Count == 1)
            {
                UIMessageTip.Show("没有可修改的分组");
                return;
            }
            GroupChange f = new GroupChange();
            f.Group = actLabel.Text.Substring(0, actLabel.Text.IndexOf("("));
            if (f.ShowDialog() == DialogResult.OK)
            {
                InitGroup();
                Label1_Click(actLabel, null);
            }
        }
        private void uiSymbolButton6_Click(object sender, EventArgs e)
        {
            if (actLabel.Tag is null)
            {
                UIMessageTip.Show("请选择店铺分组");
                return;
            }
            if (MyMessageBox.ShowAsk("是否删除该分组？"))
            {
                try
                {
                    string Sql = "delete u_mall_group where group_id=" + actLabel.Tag.ToString();

                    BackMsg backMsg = Quick_Sql.upd(Sql);
                    if (backMsg.Code == 0)
                    {
                        UIMessageTip.ShowOk("删除成功！");
                        InitGroup();
                        Label1_Click(actLabel, null);
                    }
                    else
                    {
                        MyMessageBox.ShowError("删除失败！" + backMsg.Mess);
                    }
                }
                catch (Exception ex)
                {
                    MyMessageBox.ShowError("删除失败！" + ex.Message);
                }
            }
        }

        private void panel3_Resize(object sender, EventArgs e)
        {
            uiProgressIndicator1.Location = new Point(panel3.Width / 2 - 50, panel3.Height / 2 - 50);
        }

        public void removeMall(MallTabel tabel)
        {
            this.BeginInvoke(new Action(() =>
            {
                //删除店铺
                if (MyMessageBox.ShowAsk("是否删除店铺【" + tabel.mall_name+"】？"))
                {
                    //执行删除
                    UpdLog("执行删除店铺！" + string.Join(",", mallIds));
                    BackMsg backMsg = Mall_Info.del(tabel.mall_id.ToString());
                    if (backMsg.Code == 0)
                    {
                        UIMessageTip.ShowOk("删除成功！");
                        UpdLog($"成功删除{mallIds.Count}个店铺！");
                        Label1_Click(menuPanel.Controls[menuPanel.Controls.Count - 1], null);
                    }
                    else
                    {
                        MyMessageBox.ShowError("删除失败！" + backMsg.Mess);
                        UpdLog("删除失败！" + backMsg.Mess);
                    }
                }
            }));

        }
    }
}
