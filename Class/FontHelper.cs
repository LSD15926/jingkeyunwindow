using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Class
{
    public static class FontHelper
    {
        public static PrivateFontCollection pfc;
        public static void InitPfc()
        {
            pfc = new PrivateFontCollection();

            // 假设字体文件在项目的 Fonts 文件夹中  
            string fontPath = Path.Combine(Application.StartupPath, "Font", "Alibaba_PuHuiTi_2.0_75_SemiBold_75_SemiBold.ttf");
            pfc.AddFontFile(fontPath);
        }
    }
}
