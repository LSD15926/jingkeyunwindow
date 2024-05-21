using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jingkeyun.Class
{
    public class FileHelper
    {
        public static void DeleteFilesAndFolders(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("目录不存在: " + path);
                return;
            }

            // 获取目录中的文件列表  
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            // 遍历文件并删除  
            foreach (FileInfo file in files)
            {
                try
                {
                    // 删除文件  
                    file.Delete();
                    Console.WriteLine("已删除文件: " + file.Name);
                }
                catch (Exception ex)
                {
                    // 处理删除文件时可能发生的异常  
                    Console.WriteLine("删除文件时出错: " + file.Name + " - " + ex.Message);
                }
            }

            // 获取目录中的子目录列表  
            DirectoryInfo[] dirs = new DirectoryInfo(path).GetDirectories();

            // 递归遍历子目录并删除其中的文件和子目录  
            foreach (DirectoryInfo dir in dirs)
            {
                DeleteFilesAndFolders(dir.FullName);

                // 最后删除空目录  
                try
                {
                    dir.Delete(true); // true 参数表示同时删除子目录和文件  
                    Console.WriteLine("已删除目录: " + dir.Name);
                }
                catch (Exception ex)
                {
                    // 处理删除目录时可能发生的异常  
                    Console.WriteLine("删除目录时出错: " + dir.Name + " - " + ex.Message);
                }
            }
        }
    }
}
