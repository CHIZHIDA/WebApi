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
        private string _timestamp = ((DateTime.Now.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000).ToString(); //默认将当前时间转换为时间戳
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