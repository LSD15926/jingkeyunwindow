using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace jingkeyun.Class
{
    public class Config
    {
        #region 加密类
        public class Secret
        {
            public static string sha1(string str)
            {

                byte[] cleanBytes = Encoding.Default.GetBytes(str);
                byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanBytes);
                return BitConverter.ToString(hashedBytes).Replace("-", "");

            }
            public static string MyMD5(string Str)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] InBytes = Encoding.UTF8.GetBytes(Str);
                byte[] OutBytes = md5.ComputeHash(InBytes);
                string OutString = "";
                for (int i = 0; i < OutBytes.Length; i++)
                {
                    OutString += OutBytes[i].ToString("x2");
                }
                return OutString;
            }

            public static readonly string IV = "SuFjcEmp/TE=";
            public static readonly string Key = "KIPSToILGp6fl+3gXJvMsN4IajizYBBT";
            public static string 加密(string inputValue)
            {
                TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();

                provider.IV = Convert.FromBase64String(IV);
                provider.Key = Convert.FromBase64String(Key);


                // 创建内存流来保存加密后的流
                MemoryStream mStream = new MemoryStream();

                // 创建加密转换流
                CryptoStream cStream = new CryptoStream(mStream,
                provider.CreateEncryptor(), CryptoStreamMode.Write);

                // 使用UTF8编码获取输入字符串的字节。
                byte[] toEncrypt = new UTF8Encoding().GetBytes(inputValue);

                // 将字节写到转换流里面去。
                cStream.Write(toEncrypt, 0, toEncrypt.Length);
                cStream.FlushFinalBlock();

                // 在调用转换流的FlushFinalBlock方法后，内部就会进行转换了,此时mStream就是加密后的流了。
                byte[] ret = mStream.ToArray();

                // Close the streams.
                cStream.Close();
                mStream.Close();

                //将加密后的字节进行64编码。
                return (Convert.ToBase64String(ret)).Replace("+", "＋").Replace("/", "／");
            }

            public static string 解密(string inputValue)
            {
                TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
                inputValue = inputValue.Replace("＋", "+");
                inputValue = inputValue.Replace("%2f", "/");
                inputValue = inputValue.Replace(" ", "+");
                inputValue = inputValue.Replace("／", "/");
                provider.IV = Convert.FromBase64String(IV);
                provider.Key = Convert.FromBase64String(Key);

                byte[] inputEquivalent = Convert.FromBase64String(inputValue);

                // 创建内存流保存解密后的数据
                MemoryStream msDecrypt = new MemoryStream();

                // 创建转换流。
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                                                            provider.CreateDecryptor(),
                                                            CryptoStreamMode.Write);

                csDecrypt.Write(inputEquivalent, 0, inputEquivalent.Length);

                csDecrypt.FlushFinalBlock();
                csDecrypt.Close();

                //获取字符串。
                return new UTF8Encoding().GetString(msDecrypt.ToArray());
            }
        }




        #endregion
        public class OperateIniFile
        {
            #region API函数声明

            [DllImport("kernel32")]//返回0表示失败，非0为成功
            private static extern long WritePrivateProfileString(string section, string key,
                string val, string filePath);

            [DllImport("kernel32")]//返回取得字符串缓冲区的长度
            private static extern long GetPrivateProfileString(string section, string key,
                string def, StringBuilder retVal, int size, string filePath);


            #endregion

            #region 读Ini文件

            public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
            {
                if (File.Exists(iniFilePath))
                {
                    StringBuilder temp = new StringBuilder(1024*100);
                    GetPrivateProfileString(Section, Key, NoText, temp, 1024*100, iniFilePath);
                    return temp.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }

            #endregion

            #region 写Ini文件

            public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
            {
                if (File.Exists(iniFilePath))
                {
                    long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                    if (OpStation == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            #endregion
        }
        public static string Version
        {
            get
            {
                string content = "获取版本号失败";
                try
                {
                    content = OperateIniFile.ReadIniData("version", "key", "", Application.StartupPath + "/config.ini");

                }
                catch
                {


                }

                return "V" + content;

            }



        }
    }
}
