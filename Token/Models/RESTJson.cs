using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Token.Models
{
    public class RESTJson
    {
        private int _errcode = -1;
        private string _errmsg = "error";
        private object _data = null;

        /// <summary>
        /// 返回代码
        /// </summary>
        public int ErrCode
        {
            get { return _errcode; }
            set { _errcode = value; }
        }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string ErrMsg
        {
            get { return _errmsg; }
            set { _errmsg = value; }
        }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }
    }
}