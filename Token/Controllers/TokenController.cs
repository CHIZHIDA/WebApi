using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Token.Methods;
using Token.Models;

namespace Token.Controllers
{
    public class TokenController : ApiController
    {
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
            TokenInfo chacheTokenInfo = (TokenInfo)HttpRuntime.Cache.Get(loginname);

            if (chacheTokenInfo == null)
            {
                UtilityHelper.CreateToken(loginname);   //创建Token并写入Cache缓存中
            }

            json.ErrCode = 1;
            json.ErrMsg = "sucess";

            return json;
        }

        [AllowAnonymous]
        public bool TestException()
        {
            throw new Exception("Exception Test");
        }

        public bool TestToken([FromBody]TokenInfo token)
        {
            return true;
        }
        
        public bool TestCacheToken([FromBody]LoginInfo logininfo)
        {
            return false;
        }

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
        
        /// <summary>
        /// 判断Token是否有效
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsExitTokenInfo(string username)
        {
            if (HttpRuntime.Cache.Get(username) != null)
            {
                return true;
            }

            return false;
        }
    }
}