using jingkeyun.Data;
using jingkeyun.Windows.Helper;
using Pdd_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jingkeyun.Class
{
    public static class InitUser
    {

        public static LoginUser User { get; set; } = new LoginUser();

        /// <summary>
        /// 已授权店铺
        /// </summary>
        public static List<Mallinfo> All_mall { get; set; } = new List<Mallinfo>();

        public static List<Mallinfo> Choose_mall { get; set; } = new List<Mallinfo>();

        public static FormMain MainForm = null;

        /// <summary>
        /// 后台运行的任务
        /// </summary>
        public static Dictionary<string,string> RunningTask=new Dictionary<string, string>();


        //public static string pageUrl = "http://jkysh.qpeter.cn/";  
        //public static string pageUrl = "http://192.168.1.7:8081/";
        // public static string pageUrl = "http://127.0.0.1:8848/";
        public static string pageUrl = "http://47.94.170.14/";
    }
}
