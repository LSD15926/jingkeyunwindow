using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;//验证码Bitmap类需要的命名空间
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Management;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Pdd_Models;
using Sunny.UI;

namespace BsPhpHelper
{
    public class BsPhp
    {

        //定义变量
        private  string AppEn_KEY;
        private  string AppEn_MD5; //获取自身文件MD5
        private  string AppEn_BSphpSeSsl;//连接标记，类似Cookies
        public  Image AppEn_imga;//验证码图片。图片验证码是可选项，主要是为了防止恶意注册
                                //验证码的使用与否，需要在管理后台的“系统”->“验证码设置”中可以进行设置

        //定义状态码查询 字典
        public  Dictionary<string, string> StatusCode;

        //5个配置参数
        private string AppEn_ImgaUrl = "http://app.bsphp.com/index.php?m=coode&sessl=";//验证码路径
        private string AppEn_HOST_KEY = "36b3d8ab2f82afe5c4c6835e1b8ab4af";//通信认证Key
        private string AppEn_password = "6eByzKz10FQnY3bgyL";//通讯密码
        private string AppEn_Url = "https://www.gtrscloud.com/AppEn.php?appid=28060010&m=6c756ddda207c8e9c03a6d70bfadfe72";//服务器地址
        private string AppEn_InSgin = "[KEY]123456_cj";//IN服务器签名
        private string AppEn_ToSgin = "[KEY]456789_cj";//TO服务器签名
        //private string AppEn_MoShi;     //发包模式大写 POST  GET        

        public BsPhp()
        {

            ///////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////配置段开始//////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////

            AppEn_ImgaUrl = "http://app.bsphp.com/index.php?m=coode&sessl=";//验证码路径
            AppEn_HOST_KEY = "36b3d8ab2f82afe5c4c6835e1b8ab4af";//通信认证Key
            AppEn_password = "6eByzKz10FQnY3bgyL";//通讯密码
            AppEn_InSgin = "[KEY]123456_cj";//IN服务器签名
            AppEn_ToSgin = "[KEY]456789_cj";//TO服务器签名
            AppEn_Url = "https://www.gtrscloud.com/AppEn.php?appid=28060010&m=6c756ddda207c8e9c03a6d70bfadfe72";//服务器地址





            //AppEn_MoShi = "POST";//发包模式大写 POST  GET  
            /////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////配置段结束/////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////
            ////使用SDK演示时请先阅读查看相关配置说明(服务端的)


            //获取3个基本值

            GetCpuID();//这里采用DLL的获取CPU序列号作为机器码，用户也可以自己找方法获取别的绑定参数
            GetMD5();//此函数不是返回真实MD5

            //下面这个在网络不好的时候比较耗时，会影响界面显示速度，此处使用简单调用法进行演示。
            //追求完美的作者可以采用其他方式初始化下面2个值，在界面显示后在初始化这2个值。
            AppEn_BSphpSeSsl = AppEn_GetBSphpSeSsL();


            // MessageBox.Show(AppEn_GetBSphpSeSsL());
            //初始化状态码查询字典
            initStatusDic();
        }


        //初始化状态查询字典
        private void initStatusDic()
        {
            StatusCode = new Dictionary<string, string>();
            StatusCode.Add("1000", "用户已经存在。请选择其他！");
            StatusCode.Add("1001", "你选择用户名可以使用！");
            StatusCode.Add("1002", "2次密码输入不一致！");
            StatusCode.Add("1003", "账号长度错误限制 3-15位");
            StatusCode.Add("1004", "账号格式错误,只能选择 26位字母+数字和_下滑线");
            StatusCode.Add("1005", "恭喜你注册成功！");
            StatusCode.Add("1006", "账号注册失败,你要注册账号可能被抢注了.");
            StatusCode.Add("1007", "密码长度限制 3-15位数");
            StatusCode.Add("1008", "账号错误");
            StatusCode.Add("1009", "密码错误");
            StatusCode.Add("1010", "登陆账号不存在");
            StatusCode.Add("1011", "登陆成功");
            StatusCode.Add("1012", "密码错误");
            StatusCode.Add("1013", "登陆失败");
            StatusCode.Add("1014", "QQ号错误");
            StatusCode.Add("1015", "邮箱错误");
            StatusCode.Add("1016", "手机号码错误");
            StatusCode.Add("1017", "保存成功");
            StatusCode.Add("1018", "保存失败");
            StatusCode.Add("1019", "不能输入空格");
            StatusCode.Add("1020", "请输入密保问题或者答案");
            StatusCode.Add("1021", "账号已经被冻结禁止登陆");
            StatusCode.Add("1022", "记录查询失败");
            StatusCode.Add("1023", "你的密保信息已经填写");
            StatusCode.Add("1024", "密保信息添加成功");
            StatusCode.Add("1025", "密保添加失败");
            StatusCode.Add("1026", "密码不能包含空格");
            StatusCode.Add("1027", "请输入旧密码");
            StatusCode.Add("1028", "旧密码不能和新密码一致");
            StatusCode.Add("1029", "密码修改失败");
            StatusCode.Add("1030", "旧密码不正确,请重新输入");
            StatusCode.Add("1031", "密码修改成功");
            StatusCode.Add("1032", "表单信息不能为空,请返回填写");
            StatusCode.Add("1033", "密码已经成功通过密保信息修改");
            StatusCode.Add("1034", "密保信息错误");
            StatusCode.Add("1035", "密码找回失败");
            StatusCode.Add("1036", "验证码不能为空");
            StatusCode.Add("1037", "验证码正确");
            StatusCode.Add("1038", "验证码错误");
            StatusCode.Add("1039", "检测账号不能为空");
            StatusCode.Add("1040", "账号不能包含空格");
            StatusCode.Add("1041", "长时空闲超时执行正常");
            StatusCode.Add("1042", "长时空闲超时执行异常");
            StatusCode.Add("1043", "账号不存在");
            StatusCode.Add("1044", "账号已经被冻结禁止登陆");
            StatusCode.Add("1045", "登陆超时,由于你长时间不停留请重新登陆");
            StatusCode.Add("1046", "你在别处已经登陆,被迫登出！");
            StatusCode.Add("1047", "已经登陆");
            StatusCode.Add("1048", "你需要登陆才可以访问");
            StatusCode.Add("1049", "没有登录或登录已超时请登陆,在继续你的操作！");
            StatusCode.Add("1050", "密保不能少于4字符");
            StatusCode.Add("1051", "密保信息未填写");
            StatusCode.Add("1052", "充值账号不能为空");
            StatusCode.Add("1053", "充值卡号不能为空");
            StatusCode.Add("1054", "充值卡密码不能为空");
            StatusCode.Add("1055", "充值账号不能包含空格");
            StatusCode.Add("1056", "充值卡号不能包含空格");
            StatusCode.Add("1057", "充值卡密码不能包含空格");
            StatusCode.Add("1058", "充值卡号或充值卡密码错误");
            StatusCode.Add("1059", "充值的用户账号不存在");
            StatusCode.Add("1060", "充值卡账号密码错误或者不存在");
            StatusCode.Add("1061", "用户没有使用过要充值软件,拒绝充值");
            StatusCode.Add("1062", "激活成功,赶快去使用吧！");
            StatusCode.Add("1063", "充值失败！");
            StatusCode.Add("1064", "充值卡已经充值过了");
            StatusCode.Add("1065", "你留言和反馈我们已经收到,谢谢你的支持");
            StatusCode.Add("1066", "提交留言失败");
            StatusCode.Add("1067", "请输入标题");
            StatusCode.Add("1068", "请输入留言内容");
            StatusCode.Add("1069", "激活成功,请在次验证就可以使用了！");
            StatusCode.Add("1070", "添加失败");
            StatusCode.Add("1071", "糟糕卡串已经存在了");
            StatusCode.Add("1072", "还没有存在");
            StatusCode.Add("1073", "卡串不存在或者错误");
            StatusCode.Add("1074", "你使用激活串已到期作废！");
            StatusCode.Add("1075", "卡串已经存在,无法激活");
            StatusCode.Add("1076", "car执行错误");
            StatusCode.Add("1077", "请检查卡串号或者密码错误！");
            StatusCode.Add("1078", "你使用权已经被冻结,无法验证！");
            StatusCode.Add("1079", "验证失败请重新验证,或是否已经登陆");
            StatusCode.Add("1080", "验证成功");
            StatusCode.Add("1081", "登录验证成功！");
            StatusCode.Add("1082", "你的使用期已到,请购卡在使用");
            StatusCode.Add("1083", "没有查询到用户信息");
            StatusCode.Add("1084", "该用户不是使用本软件的");
            StatusCode.Add("1085", "用户已经被冻结");
            StatusCode.Add("1086", "卡串或者验证串已经被冻结,无法继续。");
            StatusCode.Add("1087", "帐号已经到期请充值在使用。");
            StatusCode.Add("1088", "请输入一个邮箱作为你的帐号");
            StatusCode.Add("1089", "恭喜你注册成功,赶快去你邮箱把你帐号激活吧！");
            StatusCode.Add("1090", "密保邮箱不能为空！");
            StatusCode.Add("1091", "密保邮箱格式不正确,请重新输入！");
            StatusCode.Add("1092", "QQ号不能为空！");
            StatusCode.Add("1093", "QQ号格式输入不正确,请重新输入！");
            StatusCode.Add("1094", "你的帐号还没激活,现在已经有一封激活邮件发到你注册邮箱上,赶快去激活吧！");
            StatusCode.Add("5000", "无法接收到GET数据包");
            StatusCode.Add("5001", "无法接收到POST数据包");
            StatusCode.Add("5002", "数据包内出现系统屏蔽字符串");
            StatusCode.Add("5003", "数据包已经过期,拒绝接收");
            StatusCode.Add("5004", "接口不存在,连接失败");
            StatusCode.Add("5005", "软件连接号错误,访问软件不存在");
            StatusCode.Add("5006", "软件MD5验证失败");
            StatusCode.Add("5007", "非法的请求,身份验证失败！");
            StatusCode.Add("5008", "欢迎你首次使用！,请重新登陆.");
            StatusCode.Add("5009", "绑定特征码,已经有人绑定过了,不能重复绑定,不能登陆");
            StatusCode.Add("5010", "当前绑定特征已经有人绑定了");
            StatusCode.Add("5011", "账号注册成功,当前机器特征已经有人绑定");
            StatusCode.Add("5012", "当前特征已经有人绑定,无法在绑定");
            StatusCode.Add("5013", "绑定成功！");
            StatusCode.Add("5014", "绑定失败,请重试！");
            StatusCode.Add("5015", "绑定特征不能为空");
            StatusCode.Add("5016", "已经绑定了,不需要在绑定");
            StatusCode.Add("5017", "使用软件不属于登陆模式");
            StatusCode.Add("5018", "当前卡串已经到期,无法解除绑定");
            StatusCode.Add("5019", "解除绑定将到期,无法解除绑定");
            StatusCode.Add("5020", "解除绑定失败,请重试或者联系相关人员解决");
            StatusCode.Add("5021", "解除绑定成功,已经扣除对应时间");
            StatusCode.Add("5022", "当前的卡串已经解除绑定,无须在解除绑定,请直接绑定就可以");
            StatusCode.Add("5023", "绑定KEY不能为空");
            StatusCode.Add("5024", "恭喜你！绑定成功");
            StatusCode.Add("5025", "绑定失败,当前卡串已经绑定key,必须解除绑定才能在绑定");
            StatusCode.Add("5026", "账号登录超时,由于你长时间没有操作请种新登录");
            StatusCode.Add("5027", "登录状态更新失败！");
            StatusCode.Add("5028", "登录状态更新成功！");
            StatusCode.Add("5029", "帐号没有到期！");
            StatusCode.Add("5030", "登录帐号已经到期！");
            StatusCode.Add("5031", "执行正常");
            StatusCode.Add("5032", "扣点模式之扣点执行失败");
            StatusCode.Add("9980", "用户VIP到期");
            StatusCode.Add("BSPHP_940006", "AppID md5参数错误 不能为空！");
            StatusCode.Add("BSPHP_940007", "AppID md5参数错误！服务器地址不正确 ");
            StatusCode.Add("BSPHP_940008", "通信认证Key不能为空！");
            StatusCode.Add("BSPHP_940009", "通信认证Key验证失败！");
            StatusCode.Add("BSPHP_940010", "BSphpSeSsL连接串不可为空！");
        }

        //充值
        public  BackMsg AppEn_vipchong(string user, string password, string userset, string ka, string pwd)
        {
            return HttpPost("&api=chong.lg&user=" + user + "&userpwd=" + password + "&userset=" + userset + "&ka=" + ka + "&pwd=" + pwd);
        }


        ////心跳包
        public  BackMsg AppEn_timeout()
        {
            return HttpPost("&api=timeout.lg");
        }

        //解绑
        public  BackMsg AppEn_Unbundling(string user, string password)
        {
            return HttpPost("&api=jiekey.lg&user=" + user + "&pwd=" + password);
        }

        //注册
        public  BackMsg AppEn_registration(string user, string pwd, string pwdb, string ques, string ans, string qq, string mail, string mobile, string img)
        {
            return HttpPost("&api=registration.lg&user=" + user + "&pwd=" + pwd + "&pwdb=" + pwdb + "&mibao_wenti=" + ques + "&mibao_daan=" + ans + "&qq=" + qq + "&mail=" + mail + "&mobile=" + mobile + "&coode=" + img);
        }

       

        //登录
        public  BackMsg AppEn_LogIn(string User, string pwd, string imga)
        {
            if (imga == "")
            {
                imga = "null";
            }
            return HttpPost("&api=login.lg&user=" + User + "&pwd=" + pwd + "&key=" + AppEn_KEY + "&coode=" + imga + "&maxoror=" + AppEn_KEY);
        }

        //取用户信息  QQ mail mobile
        public  BackMsg AppEn_GetUserInfo(string info)
        {
            return HttpPost("&api=getuserinfo.lg&info=" + info);
        }
        public BackMsg AppEn_Cancell()
        {
            return HttpPost("&api=cancellation.lg");
        }

        //BSphpSeSsL 获取
        public  string AppEn_GetBSphpSeSsL()
        {
            BackMsg back= HttpPost("&api=BSphpSeSsL.in");
            if (back.Code == 0)
                return back.Mess;
            return "";
        }


        private  BackMsg HttpPost(string postdata)
        {
            BackMsg backMsg = new BackMsg();
            // 地址
            HttpWebRequest hreq = (HttpWebRequest)HttpWebRequest.Create(AppEn_Url);
            hreq.Method = "POST";
            hreq.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            // hreq.Accept = "Application/json, Text/javascript, */*; q=0.01"

            hreq.Timeout = 30 * 1000;


            try
            {

                // Await Task.Run(Sub() //net framework 4.5 以上可用此异步操作技术。

                string sdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //时间


                //随机生成appsafecode
                string appsafecode = Convert.ToString(new Random().Next(100000, 999999));

                //组装API接口
                string strPost = "&BSphpSeSsL=" + AppEn_BSphpSeSsl + "&mutualkey=" + AppEn_HOST_KEY + "&md5=" + AppEn_MD5 + "&date=" + sdate + "&appsafecode=" + appsafecode + postdata;

                //DES加密
                string desPost = DesEncrypt(strPost, AppEn_password);

                //服务器接收签名KEY设置
                string serverKey = AppEn_InSgin.Replace("[KEY]", desPost);

                //签名值
                string sginMd5 = MD5Encrypt32(serverKey);

                // url编码 + 签名
                var strPostdes = "parameter=" + UrlEncode(desPost) + "&sgin=" + sginMd5;


                byte[] byte1 = Encoding.UTF8.GetBytes(strPostdes);
                hreq.ContentLength = byte1.Length;

                Stream poststream = hreq.GetRequestStream();
                poststream.Write(byte1, 0, byte1.Length);
                poststream.Close();

                HttpWebResponse hres = (HttpWebResponse)hreq.GetResponse();


                // 解析返回结果
                string strResponse = "";
                StreamReader reader = new StreamReader(hres.GetResponseStream(), Encoding.UTF8);
                strResponse = reader.ReadToEnd();
                // End Sub)

                //DES解码
                string sjson = DesDecrypt(strResponse, AppEn_password);
                                

                //{"response":{"data":"sslid-appen-d85701fa0da66ec120799f881f847c7a15524360848341","date":"2019-03-13 08:14:44","unix":"1552436084","microtime":"0.103242","appsafecode":"safecode1234","sgin":"d41d8cd98f00b204e9800998ecf8427e"}}

                if (sjson.StartsWith("{\"response"))
                {
                    int index = sjson.LastIndexOf("}}");
                    sjson = sjson.Substring(0, index + 2);
                    JsonReturn aJson = JsonConvert.DeserializeAnonymousType(sjson, new JsonReturn());

                    //判断 appsafecode 是否与发出时一致
                    if (aJson.response.appsafecode != appsafecode)
                    {
                        backMsg.Code = 101;
                        backMsg.Mess = "appsafecode与发出时不一致";
                        return backMsg;
                    }
                        


                    //返回签名验证
                    //data ＋ date ＋ unix ＋ microtime ＋ appsafecode
                    string sSginReturn = aJson.response.data + aJson.response.date + aJson.response.unix + aJson.response.microtime + aJson.response.appsafecode;

                    //服务器返回签名KEY设置
                    string serverKeyreturn = AppEn_ToSgin.Replace("[KEY]", sSginReturn);

                    //签名值
                    string sginMd5return = MD5Encrypt32(serverKeyreturn);

                    if (sginMd5return != aJson.response.sgin)
                    {
                        backMsg.Code = 102;
                        backMsg.Mess = "tosgin 签名验证失败";
                        return backMsg;
                    }

                    else
                    {
                        backMsg.Code = 0;
                        backMsg.Mess = aJson.response.data;
                        return backMsg;
                    }

                }

                //条件不符合要求
                backMsg.Code = 103;
                backMsg.Mess = sjson;
                return backMsg;

            }
            catch (Exception ex)
            {
                backMsg.Code = 103;
                backMsg.Mess = ex.Message;
                return backMsg;
            }
        }

        public  void GetCpuID()
        {
            try
            {
                string cpuInfo = "";
                ManagementClass cimobject = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = cimobject.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }

                moc = null/* TODO Change to default(_) if this is not a reference type */;
                cimobject = null/* TODO Change to default(_) if this is not a reference type */;
                AppEn_KEY= cpuInfo;
            }
            catch
            {
                AppEn_KEY= "unknowncpu";
            }
        }

        // 此函数只为模拟值
        public  void GetMD5()
        {
            AppEn_MD5="263C02479158BD9C";
        }



        public static string MD5Encrypt32(string sourcestring)
        {
            string p1 = sourcestring;
            string pwd = "";

            MD5 md5 = MD5.Create();

            // 加密后是一个字节类型的数组，注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(p1));

            // 将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。
                pwd = pwd + s[i].ToString("x2");
            }

            return pwd;
        }


        /// 
        /// DES加密字符串
        /// 
        /// 明文
        /// 密钥
        /// 
        public static string DesEncrypt(string strText, string encryptKey)
        {
            string keymd5 = MD5Encrypt32(encryptKey);

            if (keymd5.Length < 8)
            {
                int iAdd = 8 - keymd5.Length;
                string sAdd = "";
                for (int i = 1; i <= iAdd; i++)
                {
                    sAdd = sAdd + "0";
                }

                keymd5 = keymd5 + sAdd;
            }
            else
            {
                keymd5 = keymd5.Substring(0, 8);

            }
            encryptKey = keymd5.ToLower();


            string outString = "";
            byte[] byKey = null;
            byte[] IV = Encoding.Default.GetBytes(encryptKey);
            try
            {
                byKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, encryptKey.Length));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.ECB;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                outString = Convert.ToBase64String(ms.ToArray());
            }
            catch (System.Exception)
            {
                outString = "";
            }
            return outString;
        }

        /// 
        /// 解密字符串
        /// 
        /// 密文
        /// 密钥
        /// 
        static string DesDecrypt(string strText, string decryptKey)
        {
            string keymd5 = MD5Encrypt32(decryptKey);

            if (keymd5.Length < 8)
            {
                int iAdd = 8 - keymd5.Length;
                string sAdd = "";
                for (int i = 1; i <= iAdd; i++)
                {
                    sAdd = sAdd + "0";
                }

                keymd5 = keymd5 + sAdd;

            }
            else
            {
                keymd5 = keymd5.Substring(0, 8);

            }

            decryptKey = keymd5.ToLower();

            string outString = "";
            byte[] byKey = null;
            byte[] IV = Encoding.Default.GetBytes(decryptKey);
            byte[] inputByteArray = new Byte[strText.Length];
            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(decryptKey.Substring(0, decryptKey.Length));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                des.Mode = CipherMode.ECB;
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = new System.Text.UTF8Encoding();
                outString = encoding.GetString(ms.ToArray());
            }
            catch (System.Exception)
            {
                outString = "";
            }
            return outString;
        }


        /// <summary>
        /// 对Url进行编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, bool isUpper = false)
        {
            return UrlEncode(url, Encoding.UTF8, isUpper);
        }

        /// <summary>
        /// 对Url进行编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, Encoding encoding, bool isUpper = false)
        {
            var result = HttpUtility.UrlEncode(url, encoding);
            if (!isUpper)
                return result;
            return GetUpperEncode(result);
        }

        /// <summary>
        /// 获取大写编码字符串
        /// </summary>
        private static string GetUpperEncode(string encode)
        {
            var result = new StringBuilder();
            int index = int.MinValue;
            for (int i = 0; i < encode.Length; i++)
            {
                string character = encode[i].ToString();
                if (character == "%")
                    index = i;
                if (i - index == 1 || i - index == 2)
                    character = character.ToUpper();
                result.Append(character);
            }
            return result.ToString();
        }


    }


    //JSON数据结构定义
    public class JsonReturn
    {
        public JsonData response;
    }

    public class JsonData
    {

        public string data;
        public string date;
        public string unix;
        public string microtime;
        public string appsafecode;
        public string sgin;


    }
}

