using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml.Linq;
using 测试项目.Pinduoduo;

namespace 测试项目
{
    public partial class FMain : UIHeaderAsideMainFrame
    {
        public FMain()
        {
            InitializeComponent();
            //设置关联
            Aside.TabControl = MainTabControl;

            //增加页面到Main
            AddPage(new mailList(), 1001);
            //AddPage(new goodsList(), 1002);

            //设置Header节点索引
            Aside.SetNodePageIndex(Aside.Nodes[0], 1001);
            Aside.SetNodePageIndex(Aside.Nodes[1], 1002);

            //显示默认界面
            Aside.SelectFirst();
        }

        private void Aside_MenuItemClick(System.Windows.Forms.TreeNode node, NavMenuItem item, int pageIndex)
        {
            switch (pageIndex)
            {
                case 1002:
                    if (!ExistPage(pageIndex))
                        AddPage(new goodsList(), 1002);
                    SelectPage(pageIndex);
                    break;
            }
        }
    }
}
