using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutofacMapper.Model
{
    public class UserInfo
    {
        private string _username;
        private string _userpwd;
        private string _getcreatetime = DateTime.Now.ToShortDateString();

        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        public string UserPwd
        {
            get { return _userpwd; }
            set { _userpwd = value; }
        }

        public string GetCreateTime
        {
            get { return _getcreatetime; }
            set { _getcreatetime = value; }
        }
    }
}
