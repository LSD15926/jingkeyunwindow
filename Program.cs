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
using System.Reflection;
using System.Globalization;
using System.IO;

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
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            CefSettingClass.InitializeCefSetting();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FontHelper.InitPfc();
            Application.Run(new UpdateForm());
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);

            var assemblyAllName = assemblyName.Name + ".dll";
            //加载CefSharp相关库
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyPath = Path.Combine(Application.StartupPath, CefSettingClass.CefLibName, assemblyAllName);
                return File.Exists(assemblyPath) ? Assembly.LoadFile(assemblyPath) : null;
            }
            //判断程序集的区域性
            if (!assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                assemblyAllName = string.Format(@"{0}\{1}", assemblyName.CultureInfo, assemblyAllName);
            }
            using (Stream stream = executingAssembly.GetManifestResourceStream(assemblyAllName))
            {
                if (stream == null) return null;
                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
