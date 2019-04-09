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
        /// 验证时间戳是否超时
        /// </summary>
        /// <param name="timestamp">时间戳秒(UCT)</param>
        /// <returns></returns>
        public static bool IsTimestampValidity(string timestamp)
        {
            //时间戳有效期:秒
            int validity = Convert.ToInt32(ConfigurationManager.AppSettings["TimestampValidity"]);
            //当前时间戳秒(UCT)
            long nowTicks = DateTime.Now.ToUniversalTime().Ticks / 10000000;

            return (nowTicks - Convert.ToInt64(timestamp)) > validity;
        }
    }
}