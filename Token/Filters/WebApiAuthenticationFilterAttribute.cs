using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Token.Methods;
using Token.Models;

namespace Token.Filters
{
    public class WebApiAuthenticationFilterAttribute : AuthorizationFilterAttribute
    {
        private const string UnauthorizedMessage = "请求未经授权，拒绝访问。";
        private const string TokenIllegalMessage = "Token无效，拒绝访问。";
        private const string LoginOverdueMessage = "登陆超时，请重新登陆。";

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
            {
                base.OnAuthorization(actionContext);
                return;
            }

            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                base.OnAuthorization(actionContext);
                return;
            }

            //根据上下文获取传递的Json数据
            StreamReader reader = new StreamReader(HttpContext.Current.Request.InputStream);
            string json = HttpUtility.UrlDecode(reader.ReadToEnd());

            //判断参数是否携带json字符串
            if (string.IsNullOrEmpty(json))
            {
                Challenge(actionContext, UnauthorizedMessage);
                return;
            }

            //Json反序列化
            TokenInfo tokenInfo = JsonHelper.JsonDeserialize<TokenInfo>(json);

            var token = tokenInfo.Token;
            var loginname = tokenInfo.UserName;

            //判断json字符串中是否携带token和loginname信息
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(loginname))
            {
                Challenge(actionContext, UnauthorizedMessage);
                return;
            }

            //从Cache缓存中获取该用户的token值
            TokenInfo chacheTokenInfo = HttpRuntime.Cache.Get(loginname) as TokenInfo;

            //判断cache缓存中的token是否失效
            if (chacheTokenInfo == null)
            {
                Challenge(actionContext, LoginOverdueMessage);
                return;
            }

            //比较token和cache缓存中的token是否一致
            if (token != chacheTokenInfo.Token)
            {
                Challenge(actionContext, TokenIllegalMessage);
                return;
            }

            //var principal = new GenericPrincipal(new GenericIdentity(loginname), null);

            //Thread.CurrentPrincipal = principal;
            //if (HttpContext.Current != null)
            //{
            //    HttpContext.Current.User = principal;
            //}

            base.OnAuthorization(actionContext);
        }

        /// <summary>
        /// 授权信息未通过返回
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="refuseReason">错误返回提示</param>
        private void Challenge(HttpActionContext actionContext, string refuseReason)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;

            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, refuseReason);
            //actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));//可以使用如下语句
            actionContext.Response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", string.Format("realm=\"{0}\"", host)));
        }

        protected virtual bool ValidateUser(string userName, string password)
        {
            if (userName.Equals("admin", StringComparison.OrdinalIgnoreCase) && password.Equals("api.admin")) //判断用户名及密码，实际可从数据库查询验证,可重写
            {
                return true;
            }
            return false;
        }
    }
}