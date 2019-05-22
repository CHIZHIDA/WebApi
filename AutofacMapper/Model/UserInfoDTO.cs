using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutofacMapper.Model
{
    public class UserInfoDTO
    {
        private string _username;
        private string _userpwd;
        private string _role;
        private DateTime _createtime;
        private string _testtime;

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

        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }

        public DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }

        public string TestTime
        {
            get { return _testtime; }
            set { _testtime = value; }
        }
    }
}
