using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.BLL
{
    public class Container
    {
        /// <summary>
        /// IOC容器
        /// </summary>
        public static IContainer container = null;

        /// <summary>
        /// 获取Dal实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            try
            {
                if(container == null)
                {
                    Initialise();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IOC实例化出错" + ex.Message);
            }

            return container.Resolve<T>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialise()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<logininfoService>().As<IlogininfoServece>().InstancePerLifetimeScope();
            container = builder.Build();
        }
    }
}
