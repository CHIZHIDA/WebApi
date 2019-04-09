using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Token.Models
{
    /// <summary>
    /// 非业务参数对象
    /// </summary>
    public class HeadersInfo
    {
        //获取当前Unix时间戳转换成秒
        private string _timestamp = (DateTime.Now.ToUniversalTime().Ticks / 10000000).ToString();
        private string _sign;

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        /// <summary>
        /// 签名，规则：1、	根据非业务参数和业务参数拼接字符串并按照首字母排序(注意：如果首字母相同，则按照第二个字母排序，以此类推)
        /// </summary>
        public string sign
        {
            get { return _sign; }
            set { _sign = value; }
        }
    }
}