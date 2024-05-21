using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Class
{
    public static class StyleHelper
    {
        /// <summary>
        /// 高亮紫
        /// </summary>
        public static Color HighLigtPurple= Color.FromArgb(133, 97, 198);

        /// <summary>
        ///标题色
        /// </summary>
        public static Color Title = Color.FromArgb(102, 58, 183);
        /// <summary>
        /// 取消按钮
        /// </summary>
        public static Color CancelButton= Color.FromArgb(184, 134, 255);
        /// <summary>
        /// 确认按钮 
        /// </summary>
        public static Color OkButton = Color.FromArgb(119, 40, 245);

        public static void InitColor()
        {
            
        }

        public static void SetButtonColor(Sunny.UI.UIButton btn,Color color)
        {
            btn.StyleCustomMode = true;
            btn.Style = Sunny.UI.UIStyle.Custom;
            btn.FillColor = color;
            btn.FillHoverColor = color;
            btn.FillPressColor = color;
            btn.FillSelectedColor = color;
            btn.RectColor = color;
            btn.RectHoverColor = color;
            btn.RectPressColor = color;
            btn.RectSelectedColor = color;
        }
        public static void SetSymbolButtonColor(Sunny.UI.UISymbolButton btn, Color color)
        {
            btn.StyleCustomMode = true;
            btn.Style = Sunny.UI.UIStyle.Custom;
            btn.FillColor = color;
            btn.FillHoverColor = color;
            btn.FillPressColor = color;
            btn.FillSelectedColor = color;
            btn.RectColor = color;
            btn.RectHoverColor = color;
            btn.RectPressColor = color;
            btn.RectSelectedColor = color;
        }
    }
}
