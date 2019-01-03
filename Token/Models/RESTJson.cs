using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Token.Models
{
    public class RESTJson
    {
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public object Data { get; set; }
    }
}