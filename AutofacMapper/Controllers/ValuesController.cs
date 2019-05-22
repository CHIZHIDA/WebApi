using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutofacMapper.Model;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AutofacMapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        //Controller构造函数中注入你的IMapper：
        private readonly IMapper _mapper;
        public ValuesController(IMapper mapper)
        {
            _mapper = mapper;
        }

        //单个对象转Dto：
        [HttpGet]
        [Route("GetUser")]
        public async Task<UserInfoDTO> GetUserDto()
        {
            //模拟数据
            var user = new UserInfo() { UserName = "临沂", UserPwd = "9527" };
            var stu = new StudentInfo() { StuName = "stu", StuNo = "007", UserName = "kobi" };

            //单个实体对象转单个Dto对象.
            var userDto = _mapper.Map<UserInfoDTO>(user);
            //多个实体对象转单个Dto对象
            var finalDto = _mapper.Map(stu, userDto);
            return finalDto;
        }

        //集合转Dto集合（data transform object）
        [HttpGet]
        [Route("GetUserList")]
        public async Task<List<UserInfoDTO>> GetUserList()
        {
            //模拟数据
            var UserList = new List<UserInfo>()
            {
           new UserInfo(){ UserName = "a", UserPwd = "9527" },
           new UserInfo(){ UserName = "b", UserPwd = "9528" },
           new UserInfo(){ UserName = "c", UserPwd = "9529" },
           new UserInfo(){ UserName = "d", UserPwd = "9530" },
            };
            //对象集合转Dto集合.
            var UserDtos = _mapper.Map<List<UserInfoDTO>>(UserList);
            return UserDtos;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
