using jingkeyun.Class;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using 测试项目.Pinduoduo;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace jingkeyun
{
    public partial class MainForm : UIHeaderAsideMainFooterFrame
    {
        public MainForm()
        {
            InitializeComponent();
            UIStyles.InitColorful(Color.FromArgb(107, 77, 160), Color.White);
            InitMyStyle();
            uiLabel1.Text = DateTime.Now.ToString("    HH:mm:ss \n yyyy-MM-dd");
            uiLabel2.Text = InitUser.User.user_name;
            uiLabel3.Text = "版本：V1.0.1";
            timer1.Start();
            ActLabel = this.uiSymbolLabel1;
            ActLabel.ForeColor=Color.Yellow;
            ActLabel.SymbolColor=Color.Yellow;
            //绑定菜单
            InitMenu();
        }

        
        private void InitMyStyle()
        {
            this.uiSymbolLabel1.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel1.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel2.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel2.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel3.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel3.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel4.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel4.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel5.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel5.ForeColor = System.Drawing.Color.White;
            this.uiSymbolLabel6.SymbolColor = System.Drawing.Color.White;
            this.uiSymbolLabel6.ForeColor = System.Drawing.Color.White;
            uiLabel1.ForeColor = System.Drawing.Color.White;
        }
        private void InitMenu()
        {
            //设置关联
            Aside.TabControl = MainTabControl;

            //增加页面到Main
            AddPage(new mailList(), 1001);
            AddPage(new goodsList(), 1002);
            //设置Header节点索引
            Aside.CreateNode("店铺管理", 1001);
            Aside.CreateNode("商品管理", 1002);
            Aside.CreateNode("运营管理", 1003);
            Aside.CreateNode("售后管理", 1004);
            Aside.CreateNode("数据中心", 1005);
            Aside.CreateNode("系统设置", 1006);

            //显示默认界面
            Aside.SelectFirst();

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            uiLabel1.Text = DateTime.Now.ToString("    HH:mm:ss \n yyyy-MM-dd");
        }

        private void Aside_MenuItemClick(System.Windows.Forms.TreeNode node, NavMenuItem item, int pageIndex)
        {

        }
        private UISymbolLabel ActLabel = null;
        private void uiSymbolLabel1_Click(object sender, EventArgs e)
        {
            //获取点击的区域
            ActLabel.ForeColor = Color.White;
            ActLabel.SymbolColor = Color.White;
            ActLabel = sender as UISymbolLabel;
            ActLabel.ForeColor=Color.Yellow;
            ActLabel.SymbolColor=Color.Yellow;

            //跳转页面
            SelectPage(MyConvert.ToInt(ActLabel.Tag));
        }
    }
}
