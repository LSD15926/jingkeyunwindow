using jingkeyun.Data;
using MoreLinq;
using Newtonsoft.Json;
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

namespace jingkeyun.Controls
{
    public partial class SkuBoxs : UserControl
    {


        public SkuBoxs()
        {
            InitializeComponent();
        }
        private bool _BgColor = false;
        public bool BgColor
        {
            get { return _BgColor; }
            set
            {
                _BgColor = value;
                if (value)
                {
                    this.BackColor = Color.FromArgb(220, 210, 231);
                }
            }
        }
        private Goods_detailModel _DetailModel;
        public Goods_detailModel DetailModel
        {
            get { return _DetailModel; }
            set
            {
                _DetailModel = value;
                txtName.Text = value.goods_name+ " ID:" + value.goods_id.ToString();
                ShowSku();
            }
        }

        private void ShowSku()
        {
            try
            {
                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel3.Controls.Clear();
                //判断有几种规格
                if (_DetailModel.sku_list[0].spec.Count == 2)
                {
                    List<SpecItem> _specItems = new List<SpecItem>();

                    foreach (var item2 in _DetailModel.sku_list)
                    {
                        if (item2.spec != null)
                            _specItems.AddRange(item2.spec);
                    }
                    var NewSpec = _specItems.DistinctBy(x => x.spec_id).ToList();

                    long Pid = 0;
                    for (int i = 0; i < NewSpec.Count; i++)
                    {
                        if (i == 0)
                        {
                            Pid = (long)NewSpec[i].parent_id;
                            txtP1.Text = NewSpec[0].parent_name;
                            txtP2.Text= NewSpec[1].parent_name;
                        }
                            
                        UITextBox textBox = new UITextBox();
                        textBox.Text = NewSpec[i].spec_name;
                        textBox.Tag = NewSpec[i];
                        textBox.Size = new Size(140, 29);
                        if (Pid == (long)NewSpec[i].parent_id)
                            flowLayoutPanel1.Controls.Add(textBox);
                        else
                            flowLayoutPanel3.Controls.Add(textBox);
                    }
                }
                else
                {
                    GgPanel2.Visible = false;
                    txtP1.Text= _DetailModel.sku_list[0].spec[0].parent_name;
                    foreach (var sku in _DetailModel.sku_list)
                    {
                        UITextBox textBox = new UITextBox();
                        textBox.Text = sku.spec[0].spec_name;
                        textBox.Tag = sku.spec[0];
                        textBox.Size = new Size(140, 29);
                        flowLayoutPanel1.Controls.Add(textBox);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Change(string s1, string s2, int type)
        {
            switch (type)
            {
                case 0:
                    foreach (var item in flowLayoutPanel1.Controls)
                    {
                        if (item.GetType().Name == "UITextBox")
                        {
                            (item as UITextBox).Text = (item as UITextBox).Text.Replace(s1, s2);
                        }
                    }
                    foreach (var item in flowLayoutPanel3.Controls)
                    {
                        if (item.GetType().Name == "UITextBox")
                        {
                            (item as UITextBox).Text = (item as UITextBox).Text.Replace(s1, s2);
                        }
                    }
                    break;
                case 1:
                    foreach (var item in flowLayoutPanel1.Controls)
                    {
                        if (item.GetType().Name == "UITextBox")
                        {
                            (item as UITextBox).Text = s1 + (item as UITextBox).Text;
                        }
                    }
                    foreach (var item in flowLayoutPanel3.Controls)
                    {
                        if (item.GetType().Name == "UITextBox")
                        {
                            (item as UITextBox).Text = s1 + (item as UITextBox).Text;
                        }
                    }
                    break;
                case 2:
                    foreach (var item in flowLayoutPanel1.Controls)
                    {
                        if (item.GetType().Name == "UITextBox")
                        {
                            (item as UITextBox).Text = (item as UITextBox).Text + s1;
                        }
                    }
                    foreach (var item in flowLayoutPanel3.Controls)
                    {
                        if (item.GetType().Name == "UITextBox")
                        {
                            (item as UITextBox).Text = (item as UITextBox).Text + s1;
                        }
                    }
                    break;
            }
        }

        public List<Sku_listItem> getNewSku()
        {
            var item = _DetailModel.sku_list;
            //寻找改变的sku
            try
            {
                foreach (var C in flowLayoutPanel1.Controls)
                {
                    if (C.GetType().Name == "UITextBox")
                    {
                        if ((C as UITextBox).Text != ((C as UITextBox).Tag as SpecItem).spec_name)//存在修改
                        {
                            //获取新的spec编码
                            BackMsg msg = Good_Spec.Get(((C as UITextBox).Tag as SpecItem).parent_name, (C as UITextBox).Text, _DetailModel.mall.mall_token);
                            if (msg.Code == 0)
                            {
                                SpecItem spec = JsonConvert.DeserializeObject<SpecItem>(msg.Mess);
                                long specId = ((C as UITextBox).Tag as SpecItem).spec_id;
                                foreach (var item2 in item)
                                {
                                    int index = item2.spec.FindIndex(x => x.spec_id == specId);
                                    if (index != -1)
                                    {
                                        item2.spec[index].spec_id=spec.spec_id;
                                        item2.spec[index].spec_name=spec.spec_name;
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var C in flowLayoutPanel3.Controls)
                {
                    if (C.GetType().Name == "UITextBox")
                    {
                        if ((C as UITextBox).Text != ((C as UITextBox).Tag as SpecItem).spec_name)//存在修改
                        {
                            //获取新的spec编码
                            BackMsg msg = Good_Spec.Get(((C as UITextBox).Tag as SpecItem).parent_id.ToString(), (C as UITextBox).Text, _DetailModel.mall.mall_token);
                            if (msg.Code == 0)
                            {
                                SpecItem spec = JsonConvert.DeserializeObject<SpecItem>(msg.Mess);
                                long specId = ((C as UITextBox).Tag as SpecItem).spec_id;
                                foreach (var item2 in item)
                                {
                                    int index = item2.spec.FindIndex(x => x.spec_id == specId);
                                    if (index != -1)
                                    {
                                        item2.spec[index].spec_id = spec.spec_id;
                                        item2.spec[index].spec_name = spec.spec_name;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return item;
        }

    }
}
