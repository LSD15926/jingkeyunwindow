using APIOffice.pddApi;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Controls;
using jingkeyun.Data;
using jingkeyun.Class;
using CefSharp.DevTools.Inspector;
using Sunny.UI.Win32;
using Newtonsoft.Json;
using CefSharp.DevTools.CSS;

namespace jingkeyun.Windows
{
    public partial class EditSet : UIForm
    {
        /// <summary>
        /// 0 两件折扣
        /// 1 单次限量
        /// 2 限购次数
        /// 3 坏了包赔
        /// 4 假一赔十
        /// 5 运费模板
        /// 6 发货时间
        /// </summary>
        public int pageType { get; set; } = 0;


        private Goods_detailModel _DetailModel {  get; set; }
        public Goods_detailModel DetailModel
        {
            get { return _DetailModel; }
            set { _DetailModel = value; 
                switch (pageType)
                {
                    case 0:
                        this.Text = "编辑满件折扣";
                        panel_zk.Visible=true;
                        ddlDisCount.Value= (int)value.two_pieces_discount*1.0/10;
                        break;
                    case 1:
                        this.Text = "编辑单次限量";
                        panel_xl.Visible=true;
                        uiIntegerUpDown2.Value=MyConvert.ToInt(value.order_limit);
                        break;
                    case 2:
                        this.Text = "编辑限购次数";
                        panel_xg.Visible = true;
                        uiIntegerUpDown1.Value = MyConvert.ToInt(value.buy_limit);
                        break;
                    case 3:
                        this.Text = "编辑坏了包赔";
                        panel_hlbp.Visible = true;
                        uiRadioButton1.Checked= MyConvert.ToInt(value.bad_fruit_claim)==1;
                        uiRadioButton2.Checked = MyConvert.ToInt(value.bad_fruit_claim) == 0;
                        break;
                    case 4:
                        this.Text = "编辑假一赔十";
                        panel_jyps.Visible = true;
                        uiRadioButton3.Checked = MyConvert.ToInt(value.is_folt) == 0;
                        uiRadioButton4.Checked = MyConvert.ToInt(value.is_folt) == 1;
                        break;
                    case 5:
                        this.Text = "编辑运费模板";
                        panel_yfmb.Visible = true;
                        InitTemp();
                        //uiIntegerUpDown1.Value = MyConvert.ToInt(value.buy_limit);
                        break;
                    case 6:
                        this.Text = "编辑发货时间";
                        panel_fhsj.Visible = true;
                        int time = MyConvert.ToInt(value.shipment_limit_second);
                        if (value.delivery_one_day == 1)
                        {
                            uiRadioButton10.Checked = true;
                        }
                        else if (time == 86400)
                        {
                            uiRadioButton9.Checked = true;
                        }
                        else if (time == 172800)
                        {
                            uiRadioButton8.Checked = true;
                        }
                       break;
                }
                uiLabel15.Text=value.goods_name;
            } 
        }
        private void InitTemp()
        {
            TempRequset requset = new TempRequset();
            requset.mall = _DetailModel.mall;
            BackMsg msg = Good_Template.out_Get(requset);
            if (msg.Code != 0)
            {
                return;
            }
            List<GoodsTemplate> templates = JsonConvert.DeserializeObject<List<GoodsTemplate>>(msg.Mess);
            ComboBoxItem item = new ComboBoxItem();
            for (int i = 0; i < templates.Count; i++)
            {
                item = new ComboBoxItem();
                item.Text = templates[i].template_name;
                item.Value = templates[i].template_id;
                ddlTemp.Items.Add(item);
                if(MyConvert.ToLong(_DetailModel.cost_template_id)== templates[i].template_id)
                    ddlTemp.SelectedIndex = i;
            }
        }

        public EditSet()
        {
            InitializeComponent();

            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;

            panel2.BackColor = this.TitleColor;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
        }


        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {
            //获取提交请求列表

            MyMessageBox.ShowLoading(this);
            RequstGoodEditModel model = new RequstGoodEditModel();
            model.goods_id = DetailModel.goods_id;
            model.Malls = DetailModel.mall;

            switch (pageType)
            {
                case 0:
                    model.ApiType = (int)GoodsEdit.满件折扣;
                    if (ddlDisCount.Value < 5 || ddlDisCount.Value > 9.9)
                    {
                        MyMessageBox.IsShowLoading = false;
                        UIMessageTip.Show("满件折扣应为5-9.9折");
                        return;
                    }
                    model.two_pieces_discount = MyConvert.ToInt(ddlDisCount.Value * 10);
                    break;
                case 1:
                    model.ApiType = (int)GoodsEdit.限量;
                    model.order_limit = uiIntegerUpDown2.Value;
                    break;
                case 2:
                    model.ApiType = (int)GoodsEdit.限购;
                    model.buy_limit = uiIntegerUpDown1.Value;
                    break;
                case 3:
                    model.ApiType = (int)GoodsEdit.坏果包赔;
                    model.bad_fruit_claim = uiRadioButton1.Checked ? 1 : 0;
                    break;
                case 4:
                    model.ApiType = (int)GoodsEdit.假一赔十;
                    model.is_folt = uiRadioButton4.Checked ? 1 : 0;
                    break;
                case 5:
                    model.ApiType = (int)GoodsEdit.运费模板;
                    model.cost_template_id = MyConvert.ToLong((ddlTemp.SelectedItem as ComboBoxItem).Value);
                    break;
                case 6:
                    model.ApiType = (int)GoodsEdit.发货时间;
                    int shipment_limit_second = 0;
                    foreach (var radio in panel_fhsj.Controls)
                    {
                        if (radio.GetType().Name == "UIRadioButton")
                        {
                            if (((UIRadioButton)radio).Checked)
                            {
                                shipment_limit_second = Convert.ToInt32(((UIRadioButton)radio).Tag.ToString());
                            }
                        }
                    }
                    model.shipment_limit_second=shipment_limit_second;
                    break;
            }
            //发送修改请求
            BackMsg RetMsg = Good_Edit.PostGoodEdit(model);
            if (RetMsg.Code == 0)
            {
                MyMessageBox.IsShowLoading = false;
                MyMessageBox.showCheck("修改成功！");
                //修改本地数据
                switch (pageType)
                {
                    case 0:
                        _DetailModel.two_pieces_discount=model.two_pieces_discount;
                        break;
                    case 1:
                        _DetailModel.order_limit=model.order_limit;
                        break;
                    case 2:
                        _DetailModel.buy_limit=model.buy_limit;
                        break;
                    case 3:
                        _DetailModel.bad_fruit_claim=model.bad_fruit_claim;
                        break;
                    case 4:
                        _DetailModel.is_folt=model.is_folt;
                        break;
                    case 5:
                        _DetailModel.cost_template_id=model.cost_template_id;
                        break;
                    case 6:
                        _DetailModel.delivery_one_day = uiRadioButton10.Checked ? 1 : 0;
                        _DetailModel.shipment_limit_second=model.shipment_limit_second;
                        break;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MyMessageBox.IsShowLoading = false;
                MyMessageBox.ShowError(RetMsg.Mess);
            }

        }

    }
}
