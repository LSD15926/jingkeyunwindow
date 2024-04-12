using DevComponents.DotNetBar;
using jingkeyun.Class;
using jingkeyun.Data;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pdd_Models;
using Pdd_Models.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Class;
using jingkeyun.Data;
using jingkeyun.Windows;

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
            uiPagination1.PageSize = 20;
            //ReadData();
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
            uiProgressIndicator1.Visible = true;
            isFirst = true;
            timer1.Start();
            ReadServerDataState = true;
        }
        private async void ListData()
        {
            //var ws = Stopwatch.StartNew();

            AllGood.Clear();
            List<requestGoodList> requestGoods = new List<requestGoodList>(); //所有请求体集合
            for (int i = 0; i < InitUser.Choose_mall.Count; i++)
            {
                requestGoodList requestGoodList = new requestGoodList();
                requestGoodList.Malls = InitUser.Choose_mall[i];
                BackData backData = Good_List.Get(requestGoodList);
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
                    UIMessageBox.Show(ex.Message);
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
        private void ShowData()
        {
            if (!isFirst)
            {
                //完成数据刷新时
                if (this.列表.Rows.Count > 0)//非第一次渲染只更新列表最大值
                {
                    uiPagination1.TotalCount = AllGood.Count;
                    UpdLog($"成功读取{uiPagination1.TotalCount}条数据");
                    return;
                }
            }
            if (isFirst)
            {
                isFirst = false;
            }
            GoodsId.Clear();
            allImage.Clear();
            Url.Clear();
            this.列表.Rows.Clear();
            if (ReadServerData.Code != 0)
            {
                timer1.Stop();
                uiProgressIndicator1.Visible = false;
                UIMessageBox.ShowError("读取商品错误" + ReadServerData.Mess);
                UpdLog("读取商品错误" + ReadServerData.Mess);
                return;
            }
            if (AllGood.Count == 0)
            {
                timer1.Stop();
                uiProgressIndicator1.Visible = false;
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
                if (txtIds.Text.Trim().Contains("，"))
                {
                    goodIds = txtIds.Text.Trim().Split('，').ToList();
                }
                else if (txtIds.Text.Trim().Contains(" "))
                {
                    goodIds = txtIds.Text.Trim().Split(' ').ToList();
                }
                else if (txtIds.Text.Trim().Contains(","))
                {
                    goodIds = txtIds.Text.Trim().Split(',').ToList();
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
                BackData data = Offend_Word.get();
                var dataJson = JsonConvert.SerializeObject(data.Data);
                List<string> Words = JsonConvert.DeserializeObject<List<string>>(dataJson);
                if (Words != null)
                {
                    List<GoodListResponse> AllMatch = new List<GoodListResponse>();
                    foreach (var word in Words)
                    {
                        List<GoodListResponse> Match = detailModels.Where(name => name.goods_name.Contains(word)).ToList();
                        AllMatch.AddRange(Match);
                    }
                    detailModels = AllMatch.DistinctBy(x => x.goods_id).ToList();
                }

            }
            uiPagination1.TotalCount = detailModels.Count;

            int skipCount = (uiPagination1.ActivePage - 1) * uiPagination1.PageSize;
            detailModels = detailModels.Skip(skipCount).Take(uiPagination1.PageSize).ToList();

            foreach (var detail in detailModels)
            {
                int i = this.列表.Rows.Add();
                列表.Rows[i].Cells["Good_Id"].Value=detail.goods_id;
                列表.Rows[i].Cells["Good_Name"].Value = detail.goods_name;// + "\r\nID:" + detail.goods_id;
                列表.Rows[i].Cells["model"].Value = detail;
                列表.Rows[i].Cells["Sku"].Value = detail.sku_list.Count;
                列表.Rows[i].Cells["quantity"].Value = detail.goods_quantity;
                列表.Rows[i].Cells["statue"].Value = detail.is_onsale == 1 ? "在售中" : "已下架";
                列表.Rows[i].Cells["mall"].Value = detail.Mallinfo.mall_name;

                //走线程
                Url.Add(detail.thumb_url);

                GoodsId.Add(new RequstGoodDetail(detail.goods_id, detail.Mallinfo));
            }
            uiProgressIndicator1.Visible = false;

            列表.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_RowPostPaint);
            ShowPic = true;
            timer2.Start();
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
                ShowPrice = true;
            }
            catch (Exception ex)
            {

            }
        }
        bool ShowPic = false;
        bool ShowPrice = false;
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (ShowPrice)
            {
                //try
                //{
                //    for (int i = 0; i < Goodmodels.Count; i++)
                //    {
                //        var skuP = Goodmodels[i].sku_list.OrderBy(x => x.multi_price).ToList();

                //        string PriceString = "拼单价：￥";
                //        if (skuP[0].multi_price == skuP[skuP.Count - 1].multi_price)
                //        {
                //            PriceString += skuP[0].multi_price / 100.00;
                //        }
                //        else
                //        {
                //            PriceString += skuP[0].multi_price / 100.00 + "--" + skuP[skuP.Count - 1].multi_price / 100.00;
                //        }

                //        列表.Rows[i].Cells["price"].Value = PriceString;
                //        PriceString = "\r\n单买价：￥";
                //        var skuD = Goodmodels[i].sku_list.OrderBy(x => x.price).ToList();
                //        if (skuD[0].multi_price == skuD[skuP.Count - 1].multi_price)
                //        {
                //            PriceString += skuD[0].multi_price / 100.00;
                //        }
                //        else
                //        {
                //            PriceString += skuD[0].multi_price / 100.00 + "--" + skuD[skuD.Count - 1].multi_price / 100.00;
                //        }
                //        列表.Rows[i].Cells["price"].Value += PriceString;
                //        列表.Rows[i].Cells["price"].Value += "\r\n参考价：￥" + Goodmodels[i].market_price / 100.00;
                //    }
                ShowPrice = false;
                timer3.Stop();
                //}
                //catch (Exception ex)
                //{

                //}
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (allImage.Count != 列表.Rows.Count)
            {
                return;
            }
            if (ShowPic)
            {
                try
                {
                    for (int i = 0; i < allImage.Count; i++)
                    {
                        列表.Rows[i].Cells["Images"].Value = allImage[i];
                    }
                    ShowPic = false;
                    timer2.Stop();
                }
                catch (Exception ex)
                {

                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ReadServerDataState)
            {
                ShowData();
                ReadServerDataState = false;
                if (isComplete)
                {
                    isComplete = false;
                    timer1.Stop();
                }

            }
        }
        bool ReadServerDataState = false;
        BackData ReadServerData = new BackData();

        private void uiPagination1_PageChanged(object sender, object pagingSource, int pageIndex, int count)
        {
            ReadData();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            UpdLog("开始查询！");
            if (uiPagination1.ActivePage != 1)
                uiPagination1.ActivePage = 1;
            ReadData();
            UpdLog("查询结束！");
        }

        int cnt = 0;
        private void 列表_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.ColumnIndex == 0)
            {
                DataGridView Obj = (DataGridView)sender;
                DataGridViewCheckBoxCell ifcheck = (DataGridViewCheckBoxCell)Obj.Rows[e.RowIndex].Cells["check"];
                ifcheck.Value = !Convert.ToBoolean(ifcheck.Value);

                if (Convert.ToBoolean(ifcheck.Value))
                {
                    cnt++;
                }
                else
                {
                    cnt--;
                }
                uiLabel9.Text = cnt.ToString();
            }
        }

        private void 列表_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView Obj = (DataGridView)sender;
            //数据复原，只允许复制数据，不允许修改数据
            switch (e.ColumnIndex)
            {
                case 2://商品id
                    Obj.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (Obj.Rows[e.RowIndex].Cells["model"].Value as GoodListResponse).goods_id;
                    break;
                case 3://商品名称
                    Obj.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (Obj.Rows[e.RowIndex].Cells["model"].Value as GoodListResponse).goods_name;
                    break;
                default:
                    break;
            }
        }

        #region 批量修改
        private void uiButton3_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                UIMessageBox.Show("请至少选择一个商品！");
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
                    var NewGood = AllGood.Find(x => x.goods_id == good.goods_id);
                    NewGood=good;
                    NewGood.is_onsale = 1;
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
        /// 获取选中商品
        /// </summary>
        private int getChecked()
        {
            checkedGood.Clear();
            checkedImage.Clear();
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                if (Convert.ToBoolean(列表.Rows[i].Cells["check"].Value))
                {
                    checkedGood.Add(列表.Rows[i].Cells["model"].Value as GoodListResponse);
                    checkedImage.Add(列表.Rows[i].Cells["Images"].Value as Image);
                }
            }
            return checkedGood.Count;
        }
        /// <summary>
        /// 修改上下架
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeOnsale From = new ChangeOnsale();
            From.GoodsModel = checkedGood;
            if (From.ShowDialog() == DialogResult.OK)
            {
                foreach (var good in From.GoodsModel)
                {
                    var NewGood = AllGood.Find(x => x.goods_id == good.goods_id);
                    NewGood = good;
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
                UIMessageBox.Show("请至少选择一个商品！");
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
                UIMessageBox.Show("请至少选择一个商品！");
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
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ///批量修改标题
            ChangeXTitle Form = new ChangeXTitle();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的短标题！");
                //ReadData();
            }
        }

        private void 批量修改库存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ///批量修改标题
            ChangeQuantity Form = new ChangeQuantity();
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            if (Form.ShowDialog() == DialogResult.OK)
            {
                foreach (var good in Form.GoodsModel)
                {
                    var NewGood = AllGood.Find(x => x.goods_id == good.goods_id);
                    NewGood = good;
                    NewGood.is_onsale = 1;
                }
                ReadData();
                UpdLog($"成功修改{checkedGood.Count}个商品的库存！");
            }
        }

        private void 批量修改sku名称ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ///批量修改标题
            ChangeSkuName Form = new ChangeSkuName();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
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
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ///批量修改标题
            ChangeSkuPrice Form = new ChangeSkuPrice();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的价格！");
                ReadData();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 列表.Rows.Count; i++)
            {
                列表.Rows[i].Cells["check"].Value = checkBox1.Checked;

            }
            cnt = checkBox1.Checked ? 列表.Rows.Count : 0;
            uiLabel9.Text = cnt.ToString();
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
            }
        }

        /// <summary>
        /// 读取选中店铺下的商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton6_Click(object sender, EventArgs e)
        {
            if (InitUser.Choose_mall.Count == 0)
            {
                UIMessageBox.ShowError("请先选择店铺！");
                return;
            }
            UpdLog($"开始读取商品！");
            uiProgressIndicator1.Visible = true;
            timer1.Start();
            ReadServerDataState = false;
            isFirst = true;
            MyMethodDelegate methodDelegate = new MyMethodDelegate(ListData);
            // 创建线程实例，并传入委托实例作为ThreadStart的参数  
            Thread t = new Thread(() => methodDelegate());
            t.IsBackground = true;
            // 启动线程  
            t.Start();
        }
        public delegate void MyMethodDelegate();

        bool flag = false;
        private void 批量修改满两件折扣ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                UIMessageBox.Show("请至少选择一个商品！");
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
                UIMessageBox.Show("请至少选择一个商品！");
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
                UIMessageBox.Show("请至少选择一个商品！");
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
                UIMessageBox.Show("请至少选择一个商品！");
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
                UIMessageBox.Show("请至少选择一个商品！");
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
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }

            ChangeGoodDesc Form = new ChangeGoodDesc();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            if (Form.ShowDialog() == DialogResult.OK)
            {
                //Goodmodels=Form.goods_DetailModels;
                UpdLog($"成功修改{checkedGood.Count}个商品的商品描述！");
                //ReadData();
            }
        }

        private void 批量修改轮播图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeSlide Form = new ChangeSlide();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
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
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeDetailImage Form = new ChangeDetailImage();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
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
                UIMessageBox.Show("请至少选择一个商品！");
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
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeLongPic Form = new ChangeLongPic();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
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
                UIMessageBox.Show("请至少选择一个商品！");
                return;
            }
            ChangeGoodCode Form = new ChangeGoodCode();
            Form.goods_DetailModels = Goodmodels;
            Form.Images = checkedImage;
            Form.GoodsModel = checkedGood;
            if (Form.ShowDialog() == DialogResult.OK)
            {
                UpdLog($"成功修改{checkedGood.Count}个商品的商品编码！");
                //ReadData();
            }
        }
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiSymbolButton4_Click(object sender, EventArgs e)
        {
            txtIds.Text = "";
            txtGoodName.Text = "";
            ddlOnsale.SelectedIndex = 0;
            checkBox2.Checked = false;
            uiPagination1.ActivePage = 1;
            UpdLog("过滤条件已重置！");
        }

        private void 批量修改叶子类目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (getChecked() == 0)
            {
                UIMessageBox.Show("请至少选择一个商品！");
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

    }
}
