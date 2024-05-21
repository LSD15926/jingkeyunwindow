using ICSharpCode.SharpZipLib.Zip;
using jingkeyun.Class;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static jingkeyun.Class.Config;

namespace jingkeyun
{
    public partial class UpdateForm : UIForm
    {
        public UpdateForm()
        {
            InitializeComponent();
            
        }
        BackgroundWorker worker = new BackgroundWorker();
        private void UpdateProgressBar(object sender, ProgressChangedEventArgs e)//进度条
        {
            if (this.uiProcessBar1.Maximum >= e.ProgressPercentage)
            {
                this.uiProcessBar1.Value = e.ProgressPercentage;
            }
        }
        private void CompleteWork(object sender, RunWorkerCompletedEventArgs e)//关闭窗口
        {
            this.DialogResult = DialogResult.OK;
        }
        public string HttpGet(string Url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private void test(object sendr, DoWorkEventArgs e)
        {
            try
            {
                string Url = ConfigurationManager.AppSettings["DownUrl"].ToString(); //"https://localhost:5001/api/Update/DownUrl";
                string downUrl = HttpGet(Url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(downUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                long totalBytes = response.ContentLength;
                this.BeginInvoke(new Action(() => { this.uiProcessBar1.Maximum = (int)totalBytes; }));


                string filename = "newFile.zip";
                Stream st = response.GetResponseStream();
                Stream so = new FileStream(filename, FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024 * 5];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    so.Write(by, 0, osize);
                    worker.ReportProgress((int)totalDownloadedByte);

                    osize = st.Read(by, 0, (int)by.Length);
                    //Thread.Sleep(500);
                    if (worker.CancellationPending)
                    {
                        so.Close();
                        st.Close();
                        break;
                    }
                }
                so.Close();
                st.Close();
                if (UnZip(filename, "newFile"))
                {
                    File.Delete(filename);
                    // 启动 bat 脚本
                    Cmd("reboot.bat");
                    // 退出当前应用程序
                    Process.GetCurrentProcess().Kill();//此方法完全奏效，绝对是完全退出。
                }
                else
                {
                    MessageBox.Show("解压失败");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 解压ZIP压缩包到指定的目录下
        /// </summary>
        /// <param name="fileToUnZip"></param>
        /// <param name="zipedFolder"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool UnZip(string fileToUnZip, string zipedFolder, string password = null)
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip)) return false;
            if (!Directory.Exists(zipedFolder)) Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip.Trim()));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');
                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }
                        using (fs = File.Create(fileName))
                        {
                            int size = 1024 * 5;
                            byte[] data = new byte[size];
                            while (true)
                            {
                                size = zipStream.Read(data, 0, data.Length);
                                if (size > 0) fs.Write(data, 0, size);
                                else break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }

        /// <summary>
        /// 调用系统cmd命令，执行bat命令
        /// </summary>
        /// <param name="bat"></param>
        private void Cmd(string bat)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = bat;
            proc.StartInfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.RedirectStandardError = false;
            proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            proc.StartInfo.CreateNoWindow = false;
            proc.Start();
            proc.Close();
        }
        private void UpdateForm_Load(object sender, EventArgs e)
        {
            
        }

        private void UpdateForm_AfterShown(object sender, EventArgs e)
        {
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.RunWorkerCompleted += CompleteWork;//操作完成就会执行CompleteWork事件
            worker.ProgressChanged += UpdateProgressBar;//进程发
            try
            {
                //检测版本号
                //获取本地版本号
                string Local_V = Config.OperateIniFile.ReadIniData("version", "key", "", Path.Combine(Directory.GetCurrentDirectory(), "Config.ini"));
                string Web_V = HttpGet(ConfigurationManager.AppSettings["Version"].ToString());//"https://localhost:5001/api/Update/Version"
                if (Local_V.Equals(Web_V))
                {
                    //跳转页面
                    this.Hide();
                    LoginForm loginForm = new LoginForm();
                    loginForm.Show();
                }
                else
                {
                    worker.DoWork += test;
                    if (!worker.IsBusy)
                    {
                        uiLabel1.Text = "正在下载文件...";
                        worker.RunWorkerAsync();
                        Task.Run(() =>
                        {
                            try
                            {
                                //删除缓存文件
                                FileHelper.DeleteFilesAndFolders(Path.Combine(Application.StartupPath, "cache"));
                            }
                            catch { }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.ShowError("检查更新失败！" + ex.Message);
            }
        }
    }
}
