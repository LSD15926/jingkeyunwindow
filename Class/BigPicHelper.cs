using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Class
{
    public static class BigPicHelper
    {
        private static BigPictureForm BPf=new BigPictureForm();
        public static void Show(Image image)
        {
            if(image==null)
                return;
            BPf.ShowBigPicture(image);
        }

        public static void Hide()
        {
            BPf.Hide();
        }
    }
}
