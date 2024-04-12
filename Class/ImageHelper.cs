using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jingkeyun.Class
{
    public class ImageHelper
    {
        public static string ConvertImageToBase64(string imagePath)
        {
            using (Image image = Image.FromFile(imagePath))
            {
                ImageFormat format = GetImageFormat(image);
                using (MemoryStream mStream = new MemoryStream())
                {
                    // 将图片保存到MemoryStream中
                    image.Save(mStream, format);
                    // 将MemoryStream中的字节转换为Base64字符串
                    byte[] imageBytes = mStream.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    // 返回Base64字符串，包含图片类型
                    return $"data:image/{format.ToString().ToLower()};base64,{base64String}";
                }
            }
        }
        private static System.Drawing.Imaging.ImageFormat GetImageFormat(Image _img)
        {
            if (_img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
            {
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            }
            if (_img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
            {
                return System.Drawing.Imaging.ImageFormat.Gif;
            }
            if (_img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
            {
                return System.Drawing.Imaging.ImageFormat.Png;
            }
            if (_img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
            {
                return System.Drawing.Imaging.ImageFormat.Bmp;
            }
            return null;
        }
    }
}
