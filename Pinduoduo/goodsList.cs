using CefSharp;
using CefSharp.WinForms;
using jingkeyun.Class;
using jingkeyun.Data;
using jingkeyun.Windows;
using MoreLinq;
using Newtonsoft.Json;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using EditInfo = jingkeyun.Windows.EditInfo;

namespace jingkeyun.Pinduoduo
{
    /// <summary>
    /// 商品列表
    /// </summary>
    public partial class goodsList : UIPage
    {
        public event UpdateLogDelegate UpdLog;
        Thread List = null;
        public goodsList()
        {
            InitializeComponent();
            uiPagination1.StyleCustomMode = true;
            uiPagination1.Style = UIStyle.Purple;
        }
        private void goodsList_Load(object sender, EventArgs e)
        {
            if (UpdLog == null)
            {
                UpdLog += Error_UpdLog;
            }
            Class.ComboBoxItem item = new Class.ComboBoxItem();
            item.Text = "全部";
            item.Value = -1;
            ddlOnsale.Items.Add(item);
            item = new Class.ComboBoxItem();
            item.Text = "在售中";
            item.Value = 1;
            ddlOnsale.Items.Add(item);
            item = new Class.ComboBoxItem();
            item.Text = "已下架";
            item.Value = 0;
            ddlOnsale.Items.Add(item);
            ddlOnsale.SelectedIndex = 0;
            //默认选中所有已授权的
            InitUser.Choose_mall = InitUser.All_mall;
            txtMalls.Text = InitUser.Choose_mall.Count + "个店铺";
            //ReadData();
            ddlquantity.SelectedIndex = 0;

            //初始化列表页
            InitBrowser();


        }
        ChromiumWebBrowser Chrome;
        JsObject_Good jsObject = new JsObject_Good();
        public void InitBrowser()
        {
            Chrome = new ChromiumWebBrowser(InitUser.pageUrl + "jingkeyun/goodTable.html");
            Chrome.MenuHandler = new MenuHandler();
            Chrome.KeyboardHandler = new CEFKeyBoardHander();
            Chrome.BrowserSettings = new BrowserSettings() { WebGl = CefState.Enabled, ImageLoading = CefState.Enabled, RemoteFonts = CefState.Enabled };
            Chrome.Dock = DockStyle.Fill;
            CefSharpSettings.WcfEnabled = true;
            Chrome.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;

            jsObject.form = this;
            Chrome.JavascriptObjectRepository.Register("boundAsync", jsObject, true, BindingOptions.DefaultBinder);
            this.panelPage.Controls.Add(Chrome);
        }

        private void Error_UpdLog(string message)
        {
            return;
        }
        requestGoodList goodsGetModel = new requestGoodList();
        private void ReadData()
        {
            uiLabel9.Text = "0";
            cnt = 0;
            if (InitUser.Choose_mall.Count == 0)
            {
                UIMessageTip.ShowError("请先选择店铺！");
                return;
            }

            if (uiProgressIndicator1.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    uiProgressIndicator1.Visible = true;
                }));
            }
            else
            {
                uiProgressIndicator1.Visible = true;

            }
            isFirst = true;
            timer1.Start();
            ReadServerDataState = true;
        }
        private async void ListData(bool IsOnale)
        {
            //var ws = Stopwatch.StartNew();
            isComplete = false;
            AllGood.Clear();
            List<requestGoodList> requestGoods = new List<requestGoodList>(); //所有请求体集合
            for (int i = 0; i < InitUser.Choose_mall.Count; i++)
            {
                requestGoodList GoodList = new requestGoodList();
                GoodList.Malls = InitUser.Choose_mall[i];
                if (IsOnale)
                    GoodList.is_onsale = 1;
                BackData backData = Good_List.Get(GoodList);
                if (backData.Code != 0)//出错直接结束查询
                {
                    ReadServerData = backData;
                    break;
                }
                var json = JsonConvert.SerializeObject(backData.Data);
                List<GoodListResponse> m1 = JsonConvert.DeserializeObject<List<GoodListResponse>>(json);
                if (m1 == null)
                {
                    ReadServerData = backData;
                    break;
                }
                foreach (var response in m1)
                {
                    response.Mallinfo = InitUser.Choose_mall[i];
                }
                AllGood.AddRange(m1);
                //如果是第一次，刷新页面
                if (i == 0)
                {
                    ReadServerDataState = true;
                    //return;//测试省钱版跳出
                    //isFirst = false;
                }

                //循环分析剩余数据
                int max = MyConvert.ToInt(backData.Mess);
                max -= 100;
                int page = 2;
                while (max > 0)
                {
                    requestGoodList request = new requestGoodList();
                    request.Malls = InitUser.Choose_mall[i];
                    request.page = page;
                    request.page_size = 100;
                    if (IsOnale)
                        request.is_onsale = 1;
                    requestGoods.Add(request);
                    max -= 100;
                    page++;
                }
            }
            if (requestGoods.Count > 0)
            {
                try
                {
                    List<Task<BackData>> tasks = new List<Task<BackData>>();
                    requestGoods.ForEach(request => tasks.Add(Task.Run(async () => await getAsync(request))));


                    var results = await Task.WhenAll(tasks);
                    //ws.Stop();
                    //UIMessageTip.Show(ws.ElapsedMilliseconds.ToString());

                    //赋值
                }
                catch (Exception ex)
                {
                    MyMessageBox.Show(ex.Message);
                }
            }

            isComplete = true;
            ReadServerDataState = true;
        }
        public async Task<BackData> getAsync(requestGoodList model)
        {
            BackData messData = new BackData();
            try
            {
                HttpClient client = new HttpClient();
                string apiUrl = ConfigurationManager.AppSettings["GoodsList"].ToString();

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                // 发送异步POST请求  
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // 确保请求成功  
                response.EnsureSuccessStatusCode();

                // 读取响应内容  
                string responseBody = await response.Content.ReadAsStringAsync();

                messData = JsonConvert.DeserializeObject<BackData>(responseBody.ToString());
            }
            catch (Exception ex)
            {
                messData.Code = 2001;
                messData.Mess = ex.Message.ToString();
            }
            if (messData.Code == 0)
            {

                var json1 = JsonConvert.SerializeObject(messData.Data);
                List<GoodListResponse> m2 = JsonConvert.DeserializeObject<List<GoodListResponse>>(json1);
                if (m2 != null)
                {
                    foreach (var response in m2)
                    {
                        response.Mallinfo = model.Malls;
                    }
                    lock (AllGood)
                    {
                        AllGood.AddRange(m2);
                    }
                }
            }
            ReadServerDataState = true;
            return messData;

        }


        bool isComplete = false;
        bool isFirst = true;
        Thread thr, thr1;
        //商品列表数量
        int TableSum =0;
        private void ShowData()
        {

            if (!isFirst)
            {
                //完成数据刷新时
                if (TableSum == uiPagination1.PageSize)//非第一次渲染只更新列表最大值
                {
                    uiPagination1.TotalCount = AllGood.Count;
                    UpdLog($"成功读取{uiPagination1.TotalCount}条数据");
                    return;
                }
                if (isComplete)//全部数据搞完
                {
                    timer1.Stop();
                    timer2.Stop();
                }
                else
                    return;
            }
            if (isFirst)
            {
                isFirst = false;
            }
            GoodsId.Clear();
            allImage.Clear();
            Url.Clear();
            if (ReadServerData.Code != 0)
            {
                timer1.Stop();
                MyMessageBox.IsShowLoading = false;
                uiProgressIndicator1.Visible=false;
                MyMessageBox.ShowError("读取商品错误" + ReadServerData.Mess);
                UpdLog("读取商品错误" + ReadServerData.Mess);
                return;
            }
            if (AllGood.Count == 0)
            {
                timer1.Stop();
                jsObject.setData(AllGood, Chrome);
                uiProgressIndicator1.Visible = false;
                MyMessageBox.IsShowLoading = false;
                uiPagination1.TotalCount = 0;
                return;
            }
            //根据条件筛选
            List<GoodListResponse> detailModels = AllGood;
            if (!string.IsNullOrEmpty(txtGoodName.Text))
            {
                detailModels = detailModels.Where(name => name.goods_name.Contains(txtGoodName.Text.Trim())).ToList();
            }
            if (ddlOnsale.SelectedIndex > 0)
            {
                detailModels = detailModels.Where(x => x.is_onsale == (int)(ddlOnsale.SelectedItem as Class.ComboBoxItem).Value).ToList();
            }
            if (!string.IsNullOrEmpty(txtIds.Text))
            {
                //根据id获取数据
                List<string> goodIds = new List<string>();
                if (txtIds.Text.Trim().Contains("\r\n"))
                {
                    goodIds = txtIds.Text.Trim().Split("\r\n").ToList();
                }
                else
                {
                    goodIds.Add(txtIds.Text.Trim());
                }
                List<GoodListResponse> AllMatch = new List<GoodListResponse>();
                foreach (var word in goodIds)
                {
                    List<GoodListResponse> Match = detailModels.Where(name => name.goods_id.ToString() == word).ToList();
                    AllMatch.AddRange(Match);
                }
                detailModels = AllMatch.DistinctBy(x => x.goods_id).ToList();
            }
            if (checkBox2.Checked)
            {
                //违规词筛选
                List<OffenWord> Words = Offend_Word.GetAll();
                if (Words != null)
                {
                    List<GoodListResponse> AllMatch = new List<GoodListResponse>();
                    foreach (var word in Words)
                    {
                        List<GoodListResponse> Match = detailModels.Where(name => name.goods_name.Contains(word.word)).ToList();
                        AllMatch.AddRange(Match);
                    }
                    detailModels = AllMatch.DistinctBy(x => x.goods_id).ToList();
                }

            }
            if (ddlquantity.SelectedIndex > 0)
            {
                switch (ddlquantity.SelectedIndex)
                {
                    case 1:
                        detailModels = detailModels.Where(x => x.goods_quantity >= MyConvert.ToInt(txtquantity.Text)).ToList();
                        break;
                    case 2:
                        detailModels = detailModels.Where(x => x.goods_quantity <= MyConvert.ToInt(txtquantity.Text)).ToList();
                        break;
                }
            }
            uiPagination1.TotalCount = detailModels.Count;
            //uiPagination1.PageSize = 3;//测试用
            int skipCount = (uiPagination1.ActivePage - 1) * uiPagination1.PageSize;
            detailModels = detailModels.Skip(skipCount).Take(uiPagination1.PageSize).ToList();

            //渲染数据
            jsObject.setData(detailModels, Chrome);
            TableSum = detailModels.Count;

            foreach (var detail in detailModels)
            {
                GoodsId.Add(new RequstGoodDetail(detail.goods_id, detail.Mallinfo));
                Url.Add(detail.thumb_url);
            }
            MyMessageBox.IsShowLoading = false;
            uiProgressIndicator1.Visible = false;

            thr = new Thread(new ThreadStart(GetDetail));
            thr.IsBackground = true;
            thr.Start();


            allImage = new Image[Url.Count].ToList();
            Parallel.For(0, Url.Count, i =>
            {
                Image image = Image.FromStream(System.Net.WebRequest.Create(Url[i]).GetResponse().GetResponseStream());
                allImage[i] = image;
            });

            UpdLog($"成功读取{uiPagination1.TotalCount}条数据");
        }


        List<RequstGoodDetail> GoodsId = new List<RequstGoodDetail>();
        /// <summary>
        /// 当前所有商品
        /// </summary>
        List<Goods_detailModel> Goodmodels = new List<Goods_detailModel>();
        private void GetDetail()
        {
            try
            {
                BackMsg backMsg = new BackMsg();
                backMsg = Good_Edit.List(GoodsId);
                if (backMsg.Code == 0)
                {
                    Goodmodels = JsonConvert.DeserializeObject<List<Goods_detailModel>>(backMsg.Mess);
                }
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
                ReadServerDataState = false;
            }
        }
        bool ReadServerDataState = false;
        BackData ReadServerData = new BackData();

        private void uiPagination1_PageChanged(object sender, object pagingSource, int pageIndex, int count)
        {
            ReadData();
        }
        private void clearMemory_Click(object sender, EventArgs e)
        {
            if (MyMessageBox.ShowAsk("是否清除本地缓存？"))
            {
                flag_onsale_down = false;
                flag_All_down = false;
                AllGood.Clear();
                uiButton1_Click(null, null);
            }
        }

        bool flag_onsale_down = false;//在售中是否缓存
        bool flag_All_down=false;//全部数据是否缓存
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (InitUser.Choose_mall.Count == 0)
            {
                MyMessageBox.ShowError("请先选择店铺！");
                return;
            }
            UpdLog("开始查询！");
            
            if (uiPagination1.ActivePage != 1)
                uiPagination1.ActivePage = 1;
            //选中在售，且未下载缓存
            if (!flag_onsale_down && ddlOnsale.SelectedIndex == 1)
            {
                MyMessageBox.ShowLoading();
                timer1.Start();
                timer2.Start();
                ReadServerDataState = false;
                isFirst = true;
                MyMethodDelegate methodDelegate = new MyMethodDelegate(ListData);
                // 创建线程实例，并传入委托实例作为ThreadStart的参数  
                Thread t = new Thread(() => methodDelegate(true));
                t.IsBackground = true;
                // 启动线程  
                t.Start();

                flag_onsale_down = true;
            }
            else if (!flag_All_down && ddlOnsale.SelectedIndex != 1)//未选在售，且未下载缓存
            {
                MyMessageBox.ShowLoading();
                AllGood.Clear();
                timer1.Start();
                timer2.Start();
                ReadServerDataState = false;
                isFirst = true;
                MyMethodDelegate methodDelegate = new MyMethodDelegate(ListData);
                // 创建线程实例，并传入委托实例作为ThreadStart的参数  
                Thread t = new Thread(() => methodDelegate(false));
                t.IsBackground = true;
                // 启动线程  
                t.Start();

                flag_All_down =true;
                flag_onsale_down=true;
            }
            else//其他情况直接缓存中查找
            {
                ReadData();
            }
            UpdLog("查询结束！");
        }

        int cnt = 0;

        #region 批量修改
        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ///批量修改标题
            ChangeTitle Form = new ChangeTitle();
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            if (Form.ShowDialog() == DialogResult.OK)
            {
                foreach (var good in Form.GoodsModel)
                {
                    AllGood.Find(x => x.goods_id == good.goods_id).goods_name=good.goods_name;
                    AllGood.Find(x => x.goods_id == good.goods_id).is_onsale = 1;

                }
                ReadData();
                UpdLog($"成功修改{checkedGood.Count}个商品的标题！");
            }
        }

        private List<GoodListResponse> AllGood = new List<GoodListResponse>();
        private List<GoodListResponse> checkedGood = new List<GoodListResponse>();
        private List<Image> checkedImage = new List<Image>();
        private List<Image> allImage = new List<Image>();
        private List<string> Url = new List<string>();

        /// <summary>
        /// 修改上下架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeOnsale From = new ChangeOnsale();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                foreach (var good in From.GoodsModel)
                {
                    AllGood.Find(x => x.goods_id == good.goods_id).is_onsale=good.is_onsale;
                }
                ReadData();
                UpdLog($"成功修改{checkedGood.Count}个商品上下架状态！");
                //ReadData();
            }
        }
        /// <summary>
        /// 修改运费模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton10_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeTemplate From = new ChangeTemplate();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的运费模板！");
                //ReadData();
            }
        }
        /// <summary>
        /// 承诺发货时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton8_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeShipmentTime From = new ChangeShipmentTime();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的承诺发货时间！");
                //ReadData();
            }
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            uiContextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
        }

        private void 批量修改短标题ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ///批量修改短标题
            ChangeXTitle Form = new ChangeXTitle();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                Goodmodels = Form.goods_DetailModels;
                UpdLog($"成功修改{checkedGood.Count}个商品的短标题！");
                //ReadData();
            }
        }

        private void 批量修改库存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeQuantity Form = new ChangeQuantity();
            Form.goods_DetailModels = Goodmodels;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                foreach (var good in Form.GoodsModel)
                {
                    AllGood.Find(x => x.goods_id == good.goods_id).goods_quantity=good.goods_quantity;
                    AllGood.Find(x => x.goods_id == good.goods_id).sku_list = good.sku_list;
                }
                ReadData();
                UpdLog($"成功修改{checkedGood.Count}个商品的库存！");
            }
        }

        private void 批量修改sku名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ///批量修改sku名称
            ChangeSkuName Form = new ChangeSkuName();
            Form.goods_DetailModels = Goodmodels;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的sku名称！");
                ReadData();
            }
        }

        private void 批量修改sku价格ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeSkuPrice Form = new ChangeSkuPrice();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的价格！");
                ReadData();
            }
        }


        /// <summary>
        /// 选择店铺
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click_1(object sender, EventArgs e)
        {

            //所有已授权的店铺显示
            ChooseMall f = new ChooseMall();
            if (f.ShowDialog() == DialogResult.OK)
            {
                txtMalls.Text = InitUser.Choose_mall.Count + "个店铺";
                UpdLog($"选择{InitUser.Choose_mall.Count}个店铺");
                //强制刷新页面
                flag_onsale_down = false;
                flag_All_down = false;
                AllGood.Clear();
                uiButton1_Click(null, null);
            }
        }



        public delegate void MyMethodDelegate(bool flag);

        bool flag = false;
        private void 批量修改满两件折扣ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeTwoDiscounts From = new ChangeTwoDiscounts();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的满两件折扣！");
            }
        }

        private void 批量修改限购次数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeLimit From = new ChangeLimit();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的限购次数！");
            }
        }

        private void 批量修改单次限量ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeLimit From = new ChangeLimit();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的单次限量！");
            }
        }

        private void 批量修改假一赔十ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeIsFolt From = new ChangeIsFolt();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的假一赔十！");
            }
        }

        private void 批量修改坏了包赔ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeBadClaim From = new ChangeBadClaim();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的坏了包赔！");
            }
        }

        private void 批量修改商品描述ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeGoodDesc Form = new ChangeGoodDesc();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                Goodmodels = Form.goods_DetailModels;
                UpdLog($"成功修改{checkedGood.Count}个商品的商品描述！");
                //ReadData();
            }
        }

        private void 批量修改轮播图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeSlide Form = new ChangeSlide();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的轮播图！");
                //ReadData();
            }
        }

        private void 批量修改详情图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeDetailImage Form = new ChangeDetailImage();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的详情图！");
                //ReadData();
            }
        }
        private void 批量修改白底图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeWritePic Form = new ChangeWritePic();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的白底图！");
                //ReadData();
            }
        }

        private void 批量修改长途ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeLongPic Form = new ChangeLongPic();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的长图！");
                //ReadData();
            }
        }

        private void 批量修改商品编码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeGoodCode Form = new ChangeGoodCode();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的商品编码！");
                //ReadData();
            }
        }

        private void 批量修改商品属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeAttr Form = new ChangeAttr();
            Form.DetailModel = Goodmodels.Where(x=>checkedGood.FindIndex(i=>i.goods_id==x.goods_id)>-1).ToList();
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的商品属性！");
                //ReadData();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //选中区间的商品
            int begin = MyConvert.ToInt(beginS.Text) - 1;
            int end = MyConvert.ToInt(endS.Text);
            jsObject.setSelectiom(begin, end, Chrome);
        }

        private void beginS_TextChanged(object sender, EventArgs e)
        {
            beginS.Maximum = uiPagination1.PageSize < beginS.Maximum ? uiPagination1.PageSize : beginS.Maximum;
            int i = MyConvert.ToInt(beginS.Text);
            beginS.Text = i.ToString();
            endS.Minimum = i;
        }

        private void endS_TextChanged(object sender, EventArgs e)
        {
            endS.Maximum = uiPagination1.PageSize;
            int i = MyConvert.ToInt(endS.Text);
            endS.Text = i.ToString();
        }

        private void txtIds_Enter(object sender, EventArgs e)
        {
            uiPanel3.Controls.SetChildIndex(txtIds, 0);
            txtIds.Height = 70;
        }

        private void txtIds_Leave(object sender, EventArgs e)
        {
            txtIds.Height = 29;
        }


        private void 修改商品预售ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangePreSale Form = new ChangePreSale();
            Form.GoodsModel = Goodmodels.Where(x => checkedGood.FindIndex(i => i.goods_id == x.goods_id) > -1).ToList();
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {   
                UpdLog($"成功修改{checkedGood.Count}个商品的商品预售！");
                //ReadData();
            }
        }


        private void 修改规格预售ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeSkuPre Form = new ChangeSkuPre();
            Form.goods_DetailModels = Goodmodels.Where(x => checkedGood.FindIndex(i => i.goods_id == x.goods_id) > -1).ToList();
            Form.GoodsModel = checkedGood;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的商品预售！");
                //ReadData();
            }
        }
        private void 批量修改叶子类目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                MyMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeCatId Form = new ChangeCatId();
            Form.GoodsModel = checkedGood;
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的叶子类目！");
                //ReadData();
            }
        }
        #endregion

        #region 单个修改

        public void showMenu(GoodListResponse Model)
        {
            SelectModel = Model;
            Invoke(new MethodInvoker(delegate
            {
                this.MenuStripGood.Show(MousePosition.X, MousePosition.Y);
            }));
        }
        public void getSelect(List<GoodListResponse> goods)
        {
            ///获取选中的数据
            Invoke(new MethodInvoker(delegate
            {
                uiLabel9.Text = goods.Count.ToString();
            }));
            checkedGood = goods;
        }
        /// <summary>
        /// 获取选中商品
        /// </summary>
        private int getChecked()
        {
            //获取图片
            //Image image = null;
            checkedImage.Clear();
            foreach (GoodListResponse good in checkedGood)
            {
                int i = Url.FindIndex(x => x == good.thumb_url);
                checkedImage.Add(allImage[i]);
            }
            return checkedGood.Count;
        }
        GoodListResponse SelectModel;
        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            //修改标题
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditInfo f = new EditInfo();
            f.pageType = 0;
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            f.GoodsModel = model;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).goods_name = f.GoodsModel.goods_name;
                AllGood.FirstOrDefault(x => x.goods_id == model.goods_id).goods_name = f.GoodsModel.goods_name;
                ReadData();
            }
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            //商品描述
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditInfo f = new EditInfo();
            f.pageType = 2;
            
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            f.GoodsModel = model;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).goods_desc = f.DetailModel.goods_desc;
            }
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            //商品短标题
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditInfo f = new EditInfo();
            f.pageType = 1;
            
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            f.GoodsModel = model;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).tiny_name = f.DetailModel.tiny_name;
            }
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            //单次限量
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditSet f = new EditSet();
            f.pageType = 1;
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).order_limit = f.DetailModel.order_limit;
            }
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            //限购
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditSet f = new EditSet();
            f.pageType = 2;
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).buy_limit = f.DetailModel.buy_limit;
            }
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            //坏了包赔
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditSet f = new EditSet();
            f.pageType = 3;
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).bad_fruit_claim = f.DetailModel.bad_fruit_claim;
            }
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            //假一赔十
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditSet f = new EditSet();
            f.pageType = 4;
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).is_folt = f.DetailModel.is_folt;
            }
        }

        private void 修改运费模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //运费模板
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditSet f = new EditSet();
            f.pageType = 5;
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).cost_template_id = f.DetailModel.cost_template_id;
            }
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            //发货时间
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditSet f = new EditSet();
            f.pageType = 6;
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).shipment_limit_second = f.DetailModel.shipment_limit_second;
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).delivery_one_day = f.DetailModel.delivery_one_day;
            }
        }


        public void DoSale(GoodListResponse model)
        {
            SelectModel = model;
            修改上下架ToolStripMenuItem_Click(null, null);
        }
        private void 修改上下架ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoodListResponse model = SelectModel;
            var str = model.is_onsale == 1 ? "下架" : "上架";
            if (MyMessageBox.ShowAsk($"是否修改该商品状态为{str}"))
            {
                //执行修改
                requsetSaleBody requset = new requsetSaleBody();
                requset.is_onsale = model.is_onsale == 1 ? 0 : 1;
                requset.goods_id = model.goods_id;
                requset.Malls = model.Mallinfo;

                BackMsg backMsg = Good_Sale.SetOne(requset);
                if (backMsg.Code == 0)
                {
                    UIMessageTip.Show("修改成功！");
                    model.is_onsale = requset.is_onsale;
                    AllGood.Find(x=>x.goods_id==model.goods_id).is_onsale=model.is_onsale;
                    ReadData();
                }
                else
                {
                    MyMessageBox.ShowError("修改失败！" + backMsg.Mess);
                }
            }
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            //轮播图
            GoodListResponse model = SelectModel;
            List<GoodListResponse> models = new List<GoodListResponse>();
            models.Add(model);
            List<Image> images = new List<Image>();
            images.Add(Image.FromStream(System.Net.WebRequest.Create(SelectModel.thumb_url).GetResponse().GetResponseStream()));
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeSlide Form = new ChangeSlide();
            Form.goods_DetailModels = Goodmodels;
            Form.Text = "修改轮播图";
            Form.Images = images;
            Form.GoodsModel = models;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的轮播图！");
            }
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            //详情图
            GoodListResponse model = SelectModel;
            List<GoodListResponse> models = new List<GoodListResponse>();
            models.Add(model);
            List<Image> images = new List<Image>();
            images.Add(Image.FromStream(System.Net.WebRequest.Create(SelectModel.thumb_url).GetResponse().GetResponseStream()));
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ChangeDetailImage Form = new ChangeDetailImage();
            Form.goods_DetailModels = Goodmodels;
            Form.Text = "修改详情图";
            Form.Images = images;
            Form.GoodsModel = models;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的详情图！");
                //ReadData();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            txtGoodSum.Text = $"({AllGood.Count})";
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            //修改库存
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditQuantity f = new EditQuantity();
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == SelectModel.goods_id);
            f.GoodsModel = SelectModel;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                AllGood.Find(x=>x.goods_id==SelectModel.goods_id).goods_quantity= f.GoodsModel.goods_quantity;
                ReadData();
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            //修改叶子类目
            EditCat f = new EditCat();
            f.GoodsModel = SelectModel;
            if (f.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            //修改规格编码
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditCode f = new EditCode();
            f.goods_DetailModels = Goodmodels.FirstOrDefault(x => x.goods_id == SelectModel.goods_id);
            f.GoodsModel = SelectModel;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            //修改sku Name
            GoodListResponse model = SelectModel;
            List<GoodListResponse> models = new List<GoodListResponse>();
            models.Add(model);
            List<Image> images = new List<Image>();
            images.Add(Image.FromStream(System.Net.WebRequest.Create(SelectModel.thumb_url).GetResponse().GetResponseStream()));
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            ///批量修改sku名称
            ChangeSkuName Form = new ChangeSkuName();
            Form.goods_DetailModels = Goodmodels;
            Form.GoodsModel = models;
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的sku名称！");
                ReadData();
            }
        }

        private void 修改商品属性ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditAttr f = new EditAttr();
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
            }
        }

      

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //修改两件折扣
            GoodListResponse model = SelectModel;
            MyMessageBox.ShowLoading();
            if (thr != null)
            {
                while (thr.ThreadState == System.Threading.ThreadState.Background)
                {
                }
            }
            EditSet f = new EditSet();
            f.pageType = 0;
            f.DetailModel = Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id);
            MyMessageBox.IsShowLoading = false;
            Thread.Sleep(100);
            if (f.ShowDialog() == DialogResult.OK)
            {
                Goodmodels.FirstOrDefault(x => x.goods_id == model.goods_id).two_pieces_discount = f.DetailModel.two_pieces_discount;
            }
        }

        #endregion
    }
}
