using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutofacMapper.Model
{
    public class StudentInfo
    {
        private string _stuname;
        private string _stuno;
        private DateTime _createtime = DateTime.Now;
        private string _username;

        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }
        public string StuName
        {
            get { return _stuname; }
            set { _stuname = value; }
        }

        public string StuNo
        {
            get { return _stuno; }
            set { _stuno = value; }
        }

        public DateTime CreateTime
        {
            get { return _createtime; }
            set { _createtime = value; }
        }
    }
}
