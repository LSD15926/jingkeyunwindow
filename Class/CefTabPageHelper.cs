using jingkeyun.Windows.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jingkeyun.Class
{
    public static class CefTabPageHelper
    {
        private static WebTabPageForm form;

        public static void showPage(string strUrl,string pageName="")
        {
            form = new WebTabPageForm();
            form.AddTabPage(strUrl, pageName);
            form.Show();
        }

        internal static void showMobilePage(string url)
        {
            MobilePageForm form = new MobilePageForm();
            form.WebUrl = url;
            form.Show();
        }
    }
}
