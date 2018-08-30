using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.DAL;
using WebApi.Model;

namespace WebApi.BLL
{
    public partial class logininfoService:BaseService<logininfo>,IlogininfoServece
    {
        private IlogininfoDAL logininfoDAL = DAL.Container.Resolve<IlogininfoDAL>();
        public override void SetDal()
        {
            Dal = logininfoDAL;
        }
    }
}
