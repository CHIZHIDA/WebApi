using Newtonsoft.Json;
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
            var chacheTokenInfo = HttpRuntime.Cache.Get(loginname);

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
    }
}