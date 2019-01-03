using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using Token.Models;

namespace Token.Controllers
{
    public class CommonToken
    {
        public static string SecertKey = "This is a private key for service";   //服务端加密密钥，属于私钥

        public static string GenToken(TokenInfo M)
        {
            var jwtcreated = Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + 5);

            var jwtcreatedOver = Math.Round((DateTime.UtcNow.AddHours(2) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + 5);

            var payload = new Dictionary<string, dynamic>
            {
                {"iss", M.iss},//非必须。issuer 请求实体，可以是发起请求的用户的信息，也可是jwt的签发者。
                {"iat", jwtcreated},//非必须。issued at。 token创建时间，unix时间戳格式
                {"exp", jwtcreatedOver},//非必须。expire 指定token的生命周期。unix时间戳格式
                {"aud", M.aud},//非必须。接收该JWT的一方。
                {"sub", M.sub},//非必须。该JWT所面向的用户
                {"jti", M.jti},//非必须。JWT ID。针对当前token的唯一标识
                {"UserName", M.UserName},//自定义字段 用于存放当前登录人账户信息
                {"UserPwd", M.UserPwd},//自定义字段 用于存放当前登录人登录密码信息
                {"UserRole", M.UserRole},//自定义字段 用于存放当前登录人登录权限信息
            };

            return "";//JWT.JsonWebToken.Encode(payload, SecretKey,JWT.JwtHashAlgorithm.HS256); ;
        }

        protected String HMacSha256Hash(String key, String message)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                var buffer = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                var b64 = Base64UrlSafeString(buffer);
                return b64;
            }
        }

        protected String Base64UrlSafeString(Byte[] bytes)
        {
            var b64 = Convert.ToBase64String(bytes);
            return b64.Replace("=", "").Replace("+", "-").Replace("/", "_");
        }

        public string GetToken()
        {
            String secret = "eerp";
            String header = "{\"type\":\"JWT\",\"alg\":\"HS256\"}";
            String claim = "{\"iss\":\"cnooc\", \"sub\":\"yrm\", \"username\":\"yrm\", \"admin\":true}";

            TokenInfo tokenInfo = new TokenInfo();

            var encoding = Encoding.UTF8;
            var base64Header = Base64UrlSafeString(encoding.GetBytes(header));
            var base64Claim = Base64UrlSafeString(encoding.GetBytes(claim));
            var signature = HMacSha256Hash(secret, base64Header + "." + base64Claim);

            var jwt = base64Header + "." + base64Claim + "." + signature;

            return jwt;
            //Console.WriteLine(jwt);

        }



    }
}