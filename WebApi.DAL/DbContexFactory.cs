using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using WebApi.Model;

namespace WebApi.DAL
{
    public partial class DbContexFactory
    {
        /// <summary>
        /// 创建EF上下文对象，已存在就直接取，不存在就创建，保证线程内是唯一对象
        /// </summary>
        /// <returns></returns>
        public static DbContext Create()
        {
            DbContext dbContext = CallContext.GetData("DbContext") as DbContext;
            if(dbContext == null)
            {
                dbContext = new CHIZHIDADatabaseEntities();
                CallContext.SetData("DbContext", dbContext);
            }
            return dbContext;
        }
    }
}
