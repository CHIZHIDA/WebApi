using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Token.Filters;

namespace Token
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            //增加全局的权限过滤，通过token认证才能访问。
            config.Filters.Add(new WebApiAuthenticationFilterAttribute());

            //增加全局的错误异常，写入日直操作。
            config.Services.Add(typeof(IExceptionLogger), new WebApiErroLogger());
        }
    }
}
