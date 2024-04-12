using jingkeyun;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using jingkeyun.Pinduoduo;
using jingkeyun.Windows;
using jingkeyun.Class;

namespace jingkeyun
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FontHelper.InitPfc();
            Application.Run(new LoginForm());
        }
    }
}
