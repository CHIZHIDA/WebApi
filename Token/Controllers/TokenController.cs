using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Token.Enums;
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
        public RESTJson CreateClientKeyPath()
        {
            RESTJson result = new RESTJson();

            result.ErrMsg = ClientEncryptionHelper.GenerateKeys();
            result.ErrCode = 1;

            return result;
        }

        /// <summary>
        /// 客户端加密生成签名
        /// </summary>
        /// <param name="logininfo"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public RESTJson GetClientEncryptionKey([FromBody]LoginInfo loginInfo)
        {
            RESTJson result = new RESTJson();

            //非业务参数（如：时间戳等）
            HeadersInfo headersInfo = new HeadersInfo();

            //根据非业务参数和业务参数拼接字符串并按照首字母排序
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic = SignHelper<HeadersInfo>.ObjConvertDic(dic, headersInfo);
            dic = SignHelper<LoginInfo>.ObjConvertDic(dic, loginInfo);
            string str = SignHelper<string>.DicSortToString(dic);

            //使用请求方的私钥进行加密生成签名
            string sign = ClientEncryptionHelper.privateToSign(str);

            //判空
            if (string.IsNullOrEmpty(sign))
            {
                result.ErrMsg = "生成报文失败";
                return result;
            }

            //使用接收方的公钥进行加密生成加密报文
            string message = ServerEncryptionHelper.PubKeyEncryption(str += '_' + sign);
            if (!string.IsNullOrEmpty(message))
            {
                result.ErrCode = 1;
                result.ErrMsg = "生成报文成功";
                result.Data = message;
            }

            return result;
        }
        #endregion

        #region 接收方解密验签
        /// <summary>
        /// 生成服务端公钥和密钥地址
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public RESTJson CreateServerKeyPath()
        {
            RESTJson result = new RESTJson();

            result.ErrMsg = ServerEncryptionHelper.GenerateKeys();
            result.ErrCode = 1;

            return result;
        }

        /// <summary>
        /// 服务端验签
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public static string ServerInspectionSign()
        {
            RESTJson result = new RESTJson();

            //获取非业务参数请求头信息
            string timestamp = HttpContext.Current.Request.Headers["timestamp"];
            string sign = HttpContext.Current.Request.Headers["sign"];

            //判断timestamp是否超时
            if (!UtilityHelper.IsTimestampValidity(timestamp))
            {
                return UtilityEnum.InspectionResult.数据超时.ToString();
            }

            //使用接收方密钥解密报文
            string message = ServerEncryptionHelper.PriKeyDecrypted(sign);

            //验签
            if (ServerEncryptionHelper.CheckSign(message))
            {
                return UtilityEnum.InspectionResult.合法数据.ToString();
            }
            else
            {
                return UtilityEnum.InspectionResult.非法数据.ToString();
            }
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