using APIOffice.Controllers.pddApi;
using jingkeyun.Class;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Windows.Forms;
using jingkeyun.Controls;
using jingkeyun.Pinduoduo;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Threading;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using jingkeyun;
using System.Drawing.Text;
using System.Drawing;
using System.Data.SqlClient;

namespace jingkeyun
{
    public partial class Form3 : Form
    {

        private PrivateFontCollection pfc;
        private Font customFont;

        public Form3()
        {
            InitializeComponent();
            InitializeCustomFont();
        }

        private void InitializeCustomFont()
        {
           
            
        }

        private void SomeControl_Paint(object sender, PaintEventArgs e)
        {
            // 使用自定义字体绘制文本  
            e.Graphics.DrawString("Hello, Custom Font!", customFont, Brushes.Black, new PointF(0, 0));
        }
    }
}
