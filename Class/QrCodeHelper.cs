using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using ZXing.QrCode;
using ZXing;

namespace jingkeyun.Class
{
    public static class QrCodeHelper
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Bitmap getCode(string message)
        {
            if(string.IsNullOrEmpty(message))
                return null;    
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    DisableECI = true,//设置内容编码
                    CharacterSet = "UTF-8",  //设置二维码的宽度和高度
                    Height = 200,
                    Width = 200,
                    Margin = 1
                }
            };
            return writer.Write(message);
        }
    }
}
