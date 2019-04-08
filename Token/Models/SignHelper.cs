using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Token.Models
{
    public class SignHelper<T>
    {
        /// <summary>
        /// 对象转字典
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ObjConvertDic(Dictionary<string, object> dic, T obj)
        {
            //判空
            if (obj == null)
            {
                return dic;
            }

            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

            string dickeyname = string.Empty;   //用于存储 表名+字段名

            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    dickeyname = t.Name + "_" + p.Name;

                    // 进行判NULL处理 以及 重复键处理
                    if (m.Invoke(obj, new object[] { }) != null && !dic.ContainsKey(dickeyname))
                    {
                        dic.Add(dickeyname, m.Invoke(obj, new object[] { })); // 向字典添加元素
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// 字典中将key值进行升序排序，并将对应的value值拼接为字符串输出
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string DicSortToString(Dictionary<string, object> dic)
        {
            //根据字典键升序
            var ascDic = from objDic in dic orderby objDic.Key ascending select objDic;

            //报文消息头
            string str = ConfigurationManager.AppSettings["messageHeader"];

            foreach (var item in ascDic)
            {
                str += "_" + item.Value;
            }

            return str;
        }
    }
}