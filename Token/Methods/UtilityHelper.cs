using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Token.Models;

namespace Token.Methods
{
    public class UtilityHelper
    {

        /// <summary>
        /// 创建Token并记录入缓存cache
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userpwd"></param>
        /// <returns></returns>
        public static string CreateToken(string loginname)
        {
            TokenInfo tokenInfo = new TokenInfo();
            tokenInfo.UserName = loginname;
            tokenInfo.SignToken = Guid.NewGuid().ToString();    //创建一个GUID作为签名Token

            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(0, tokenInfo.UserName, DateTime.Now,
            //                DateTime.Now.AddMinutes(1), true, string.Format("{0}&{1}&{2}", tokenInfo.UserName, tokenInfo.SignToken, tokenInfo.UserRole),
            //                FormsAuthentication.FormsCookiePath);

            //string Token = FormsAuthentication.Encrypt(ticket);

            //写入Cache缓存
            //HttpRuntime.Cache.Insert(loginname, tokenInfo, null, DateTime.Now.AddMinutes(20), TimeSpan.Zero); //设置绝对过期时间
            HttpRuntime.Cache.Insert(loginname, tokenInfo, null, DateTime.MaxValue, TimeSpan.FromMinutes(5));   //设置变化时间过期(平滑过期)

            return tokenInfo.SignToken;
        }

        /// <summary>
        /// 记录程序异常日志
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg">信息说明</param>
        /// <param name="typestr">文件后缀名</param>
        public static void WriteLog(Exception ex, string msg = "", string typestr = "")
        {
            try
            {
                string path = ConfigurationManager.AppSettings["logpath"];
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileMsg = Path.Combine(path, string.Format("AppLog_{0}.txt", DateTime.Now.ToString("yyyyMMdd_") + typestr));
                var sb = new StringBuilder();
                sb.AppendLine("-------------------------");
                sb.AppendLine(DateTime.Now.ToString());
                sb.AppendLine("信息:" + msg);
                if (ex != null)
                {
                    sb.AppendLine("成员名: " + ex.TargetSite);
                    sb.AppendLine("异常信息: " + ex.Message);
                    sb.AppendLine("引发异常的程序集或对象: " + ex.Source);
                    sb.AppendLine("栈：" + ex.StackTrace);
                }

                using (FileStream fs = File.Open(fileMsg, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    byte[] buffer = Encoding.Default.GetBytes(sb.ToString());
                    fs.Seek(0, SeekOrigin.End);
                    fs.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ek)
            {
                throw ex;//  MessageBox.Show(ek.Message);
            }
        }

        /// <summary>
        /// 从缓存中获取对应Token
        /// </summary>
        /// <param name="loginname">当前请求用户loginname</param>
        /// <returns></returns>
        public static TokenInfo GetTokenInfo(string loginname)
        {
            if (string.IsNullOrEmpty(loginname))
            {
                return null;
            }

            TokenInfo tokenInfo = HttpRuntime.Cache.Get(loginname) as TokenInfo;

            if (tokenInfo != null)
            {
                return tokenInfo;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取签名认证字符串
        /// </summary>
        /// <param name="timeStamp">发起请求时的时间戳（单位：毫秒）</param>
        /// <param name="nonce">随机数</param>
        /// <param name="loginname">当前请求用户loginname</param>
        /// <param name="data">序列化后的参数字符串</param>
        /// <returns></returns>
        public static string GetSignature(string timeStamp, string nonce, string loginname, string data)
        {
            TokenInfo tokeninfo = GetTokenInfo(loginname);      //获取当前请求用户的Token信息

            if (tokeninfo != null)
            {
                var hash = System.Security.Cryptography.MD5.Create();
                //拼接签名数据
                var signStr = timeStamp + nonce + loginname + tokeninfo.SignToken.ToString() + data;
                //将字符串中字符按升序排序
                var sortStr = string.Concat(signStr.OrderBy(c => c));
                var bytes = Encoding.UTF8.GetBytes(sortStr);
                //使用MD5加密
                var md5Val = hash.ComputeHash(bytes);
                //把二进制转化为大写的十六进制
                StringBuilder result = new StringBuilder();
                foreach (var c in md5Val)
                {
                    result.Append(c.ToString("X2"));
                }
                return result.ToString().ToUpper();
            }
            else
            {
                throw new Exception("token为null，用户名称为：" + loginname);
            }
        }

        /// <summary>
        /// 判断Token是否有效
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static bool IsExitTokenInfo(string username)
        {
            if (HttpRuntime.Cache.Get(username) != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取1970-01-01至dateTime的毫秒数
        /// </summary>
        public static long GetTimestamp(DateTime dateTime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (dateTime.Ticks - dt.Ticks) / 10000;
        }

        /// <summary>
        /// 根据时间戳timestamp（单位毫秒）计算日期
        /// </summary>
        public static DateTime NewDate(long timestamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long tt = dt.Ticks + timestamp * 10000;
            return new DateTime(tt);
        }
    }
}