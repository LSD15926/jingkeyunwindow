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
using DevComponents.DotNetBar;

namespace jingkeyun.Windows
{
    public partial class ChangeOnsale : UIForm
    {

        private List<GoodListResponse> _GoodsModel = new List<GoodListResponse>();

        public List<GoodListResponse> GoodsModel
        {
            get
            {
                return _GoodsModel;
            }
            set
            {
                _GoodsModel = value;
                uiLabel2.Text = value.Count.ToString();

            }
        }

        private List<requsetSaleBody> requsets = new List<requsetSaleBody>();
        public ChangeOnsale()
        {
            InitializeComponent();
            InitMyStyle();
        }
        private void InitMyStyle()
        {
            this.StyleCustomMode = true;
            this.Style = Sunny.UI.UIStyle.Custom;
            this.TitleColor = StyleHelper.Title;

            panel2.BackColor = StyleHelper.Title;

            StyleHelper.SetButtonColor(uiButton1, StyleHelper.OkButton);
            StyleHelper.SetButtonColor(uiButton2, StyleHelper.CancelButton);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            foreach (var item in _GoodsModel)
            {
                requsetSaleBody body = new requsetSaleBody();
                body.goods_id = item.goods_id;
                body.is_onsale = uiRadioButton1.Checked ? 1 : 0;
                body.Malls = item.Mallinfo;
                requsets.Add(body);

                item.is_onsale = body.is_onsale;
            }
            InitUser.RunningTask.Add("上下架" + stampNow, stampNow.ToString());
            UIMessageTip.ShowOk("已提交至后台处理");
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync(requsets);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        BackMsg RetMsg;
        private long stampNow;
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            InitUser.RunningTask.Remove("上下架" + stampNow);
            MyMessageBox.showCheck(RetMsg.Mess, "修改上下架");
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<requsetSaleBody> models = e.Argument as List<requsetSaleBody>;
            RetMsg = Good_Sale.Status(models);
        }
        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
