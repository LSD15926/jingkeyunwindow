using CefSharp.WinForms;
using CefSharp;
using System.IO;
using System.Windows.Forms;

namespace jingkeyun
{
    public class CefSettingClass
    {
        public const string CefLibName = "library"; //cef目录名称

        /// <summary>
        /// 初始化CEF的配置
        /// </summary>
        public static void InitializeCefSetting()
        {
            //这段代码一定要在new ChromiumWebBrowser之前调用
            //作用是初始化浏览器的语言
            string appPath = Application.StartupPath;
            CefSettings settings = new CefSettings
            {
                //设置中文
                Locale = "zh-CN",
                //设置浏览器的访问头
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36",
                //设置浏览器子程序启动路径
                BrowserSubprocessPath = Path.Combine(appPath, "CefSharp.BrowserSubprocess.exe"),
            };
            //全局浏览器缓存的数据将存储在磁盘上的位置
            settings.CachePath = Path.Combine(appPath, "cache");
            //调试日志的目录和文件名。如果为空，则使用默认名称“debug.log”
            settings.LogFile = Path.Combine(appPath, @"log\debug.log");
            //禁用 USB 键盘检测 虚拟键盘
            settings.CefCommandLineArgs.Add("disable-usb-keyboard-detect", "1");
            //启用视频调谐
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            //启用触屏事件
            settings.CefCommandLineArgs.Add("touch-events", "1");
            //禁用代理服务器
            settings.CefCommandLineArgs.Add("no-proxy-server", "1");
            //禁用浏览器使用自动代理检测
            settings.CefCommandLineArgs.Add("proxy-auto-detect", "0");
            //忽略显卡黑名单
            settings.CefCommandLineArgs.Add("ignore-gpu-blacklist", "1");
            //跨域设置
            settings.CefCommandLineArgs.Add("--disable-web-security", "1");
            //禁用硬件加速GPU
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            //禁用屏外渲染最佳性能
            settings.SetOffScreenRenderingBestPerformanceArgs();
            //让浏览器的消息循环在一个单独的线程中执行
            settings.MultiThreadedMessageLoop = true;
            //忽略 URL 获取器证书请求
            settings.CefCommandLineArgs.Add("--ignore-urlfetcher-cert-requests", "1");
            //忽略证书错误
            settings.CefCommandLineArgs.Add("--ignore-certificate-errors", "1");
            //允许不安全的本地主机
            settings.CefCommandLineArgs.Add("allow-insecure-localhost", "1");
            //设置浏览器可使用的最大内存值为1GB（1024）
            settings.CefCommandLineArgs.Add("--js-flags", $"--max_old_space_size=1024");
            //禁用渲染器中的局部光栅
            settings.CefCommandLineArgs.Add("--disable-partial-raster", "1");
            //当使用CPU光栅化时，禁用低分辨率平铺
            settings.CefCommandLineArgs.Add("disable-low-res-tiling", "1");
            //禁用GPU合成？
            //setting.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            //强制设置设备的缩放因子
            settings.CefCommandLineArgs.Add("force-device-scale-factor", "1");
            //远程调试 URL
            settings.CefCommandLineArgs.Add("remote-debugging-port", "0");
            //未知
            settings.CefCommandLineArgs.Add("disable-features", "BlockInsecurePrivateNetworkRequests");
            Cef.Initialize(settings);
        }
        /// <summary>
        /// 注册方法，给javascript调用
        /// </summary>
        /// <param name="browser"></param>
        //public static void JavascriptObjectRepository(ref ChromiumWebBrowser browser)
        //{
        //    CefSharpSettings.WcfEnabled = true;
        //    browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
        //    browser.JavascriptObjectRepository.Register("boundAsync", new BoundObject(), true, BindingOptions.DefaultBinder);
        //}
    }
}