using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Token.Methods;
using Token.Models;

namespace Token.Controllers
{
    public class TokenController : ApiController
    {

        #region 登陆测试
        /// <summary>
        /// 登陆接口，生成Token存储在cache缓存中
        /// </summary>
        /// <param name="logininfo"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public RESTJson Login([FromBody]LoginInfo logininfo)
        {
            string loginname = logininfo.loginName;
            string loginpwd = logininfo.loginPwd;

            //创建默认返回值对象
            RESTJson json = new RESTJson() { ErrCode = -1, ErrMsg = "defeate", Data = null };

            if (!ValidateUser(loginname, loginpwd))
            {
                json.ErrMsg = "username or password do not null";
                return json;
            }

            //从Cache缓存中读取数据
            var chacheTokenInfo = HttpRuntime.Cache.Get(loginname);

            if (chacheTokenInfo == null)
            {
                UtilityHelper.CreateToken(loginname);   //创建Token并写入Cache缓存中
            }

            json.ErrCode = 1;
            json.ErrMsg = "sucess";

            return json;
        }

        /// <summary>
        /// 测试全局异常记录特性接口
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public bool TestException()
        {
            throw new Exception("Exception Test");
        }

        /// <summary>
        /// 测试全局身份认证接口
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool TestToken([FromBody]LoginInfo loginInfo)
        {
            return true;
        }
        #endregion

        #region 请求方加密
        /// <summary>
        /// 生成客户端公钥和密钥地址
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public string CreateClientKeyPath()
        {
            return ClientEncryptionHelper.GenerateKeys();
        }

        /// <summary>
        /// 客户端加密生成签名
        /// </summary>
        /// <param name="logininfo"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public string GetClientEncryptionKey([FromBody]LoginInfo loginInfo)
        {
            //非业务参数（如：时间戳等）
            HeadersInfo headersInfo = new HeadersInfo();

            //根据非业务参数和业务参数拼接字符串并按照首字母排序
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic = SignHelper<HeadersInfo>.ObjConvertDic(dic, headersInfo);
            dic = SignHelper<LoginInfo>.ObjConvertDic(dic, loginInfo);
            string str = SignHelper<string>.DicSortToString(dic);

            //使用请求方的私钥进行加密生成签名
            string sign = ClientEncryptionHelper.privateToSign(str);

            //使用接收方的公钥进行加密生成加密报文
            return ServerEncryptionHelper.PubKeyEncryption(str += '_' + sign);
        }
        #endregion

        #region 接收方解密验签
        /// <summary>
        /// 生成服务端公钥和密钥地址
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public string CreateServerKeyPath()
        {
            return ServerEncryptionHelper.GenerateKeys();
        }

        /// <summary>
        /// 服务端验签
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public RESTJson ServerInspectionSign([FromBody]LoginInfo loginInfo)
        {
            RESTJson result = new RESTJson();

            //获取非业务参数请求头信息
            string timestamp = HttpContext.Current.Request.Headers["timestamp"];
            string sign = HttpContext.Current.Request.Headers["sign"];

            ////使用接收方密钥解密报文
            string message = ServerEncryptionHelper.PriKeyDecrypted(sign);

            ////截取第一个下划线'_'前的文本为消息头，最后一个下划线'_'后的文本为签名
            //string[] list = message.Split('_');

            //string messageHeader = list[0];
            //string deSign = list[list.Length - 1];

            ////查看消息头是否正确
            //if (messageHeader != ConfigurationManager.AppSettings["messageHeader"])
            //{
            //    result.ErrMsg = "非法数据";
            //    return result;
            //}

            //验签
            ServerEncryptionHelper.CheckSign(message);

            return null;
        }
        #endregion

        #region null
        [AllowAnonymous]
        public bool TestCacheToken([FromBody]LoginInfo logininfo)
        {
            string timestemp = UtilityHelper.GetTimestamp(DateTime.Now).ToString();

            Random random = new Random();
            string strrandom = random.Next(1000, 10000).ToString();     //随机生成一个大于等于1000，小于10000的四位随机数

            var data = JsonConvert.SerializeObject(logininfo);    //序列化参数

            var aa = UtilityHelper.GetSignature(timestemp, strrandom, logininfo.loginName, data);

            return false;
        }
        #endregion

        #region Helper
        /// <summary>
        /// 判断用户是否合法
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userpwd"></param>
        /// <returns></returns>
        private bool ValidateUser(string username, string userpwd)
        {
            //参数判空并去除空格
            string un = (username ?? string.Empty).Trim();
            string pwd = (userpwd ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userpwd))
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}