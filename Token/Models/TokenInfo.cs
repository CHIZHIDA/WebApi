using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Token.Models
{
    public class TokenInfo
    {
        public TokenInfo()
        {
            iss = "Mark";   //该JWT的签发者，是否使用是可选的
            aud = "http://example.com"; //接收该JWT的一方，是否使用是可选的
            sub = "HomeCare.VIP";   //该JWT所面向的用户，是否使用是可选的
            jti = DateTime.Now.ToString("yyyyMMddhhmmss");  //在什么时候签发的(UNIX时间)，是否使用是可选的
            UserName = "D";
            UserRole = "HomeCare.Administrator";
            ExpireTime = DateTime.Now.AddMinutes(20);
            SignToken = Guid.NewGuid().ToString();
        }

        public string iss { get; set; }
        public string aud { get; set; }
        public string sub { get; set; }
        public string jti { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public DateTime ExpireTime { get; set; }
        public string SignToken { get; set; }
    }
}