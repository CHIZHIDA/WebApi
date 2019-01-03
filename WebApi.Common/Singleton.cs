using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Common
{
    public class Singleton<T> where T : class, new()
    {
        private static T instance;
        private static readonly T syncRoot = new T();   //程序运行时创建一个静态只读的进程辅助对象

        private Singleton() //构造方法让其private，堵死外界创建的可能
        {
        }

        public static T GetInstance()    //本类实例的唯一全局访问点
        {
            if (instance == null)   //先判断实例是否存在，不存在再加锁处理
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        return instance = Activator.CreateInstance<T>();
                    }
                }
            }
            return instance;
        }
    }
}
