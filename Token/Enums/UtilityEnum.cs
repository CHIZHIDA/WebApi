using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Token.Enums
{
    public class UtilityEnum
    {
        /// <summary>
        /// 数据返回结果
        /// </summary>
        public enum InspectionResult
        {
            [Description("非法数据")]
            Invalid,
            [Description("有效数据")]
            Validity,
            [Description("数据超时")]
            Timeout
        }
    }
}