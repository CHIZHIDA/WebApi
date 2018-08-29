using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.DAL
{
    public class Container
    {
        /// <summary>
        /// IOC容器
        /// </summary>
        public static IContainer container = null;

        /// <summary>
        /// 获取Idal的实例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            try
            {
                if (container == null)
                {
                    Initialise();
                }
            }
            catch (Exception ex)
            {

                throw new System.Exception("IOC实例化出错！" + ex.Message);
            }

            return container.Resolve<T>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialise()
        {
            var builder = new ContainerBuilder();
            //格式：builder.RegisterType<xxxx>().As<Ixxxx>().InstancePerLifetimeScope();
            builder.RegisterType<logininfoDAL>().As<IlogininfoDAL>().InstancePerLifetimeScope();
            container = builder.Build();
        }
    }
}
