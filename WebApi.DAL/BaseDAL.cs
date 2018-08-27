using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.DAL
{
    public partial class BaseDAL<T> where T : class, new()
    {
        private DbContext dbContext = DbContexFactory.Create();

        public void Add(T t)
        {
            dbContext.Set<T>().Add(t);
        }

        public void Delete(T t)
        {
            dbContext.Set<T>().Remove(t);
        }

        public void Update(T t)
        {
            dbContext.Set<T>().AddOrUpdate(t);
        }

        public IQueryable<T> GetModels(Expression<Func<T,bool>> whereLambda)
        {
            return dbContext.Set<T>().Where(whereLambda);
        }
    }
}
