using AutofacMapper.Model;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutofacMapper.Method
{
    public class UserProfile : Profile
    {
        //添加你的实体映射关系.
        public UserProfile()
        {
            //UserInfoEntity转UserInfoDto.
            CreateMap<UserInfo, UserInfoDTO>()
            .BeforeMap((source, dto) =>
            {
                //可以较为精确的控制输出数据格式
                //dto.CreateTime = Convert.ToDateTime(source.CreateTime).ToString("yyyy-MM-dd");
                if (string.IsNullOrEmpty(source.GetCreateTime))
                {
                    source.GetCreateTime = Convert.ToDateTime(source.GetCreateTime).ToString("yyyy-MM-dd");
                }
                //dto.Role = "admin";
            })
                //AutoMapper自动扁平化映射，AutoMapper会将类中的属性进行分割，或匹配“Get”开头的方法
                //指定映射字段。将UserInfo.GetCreateTime映射到UserInfoDTO.TestTime
                .ForMember(dto => dto.TestTime, opt => opt.MapFrom(info => info.GetCreateTime))
                .ForMember(dto => dto.Role, opt => opt.Ignore())
                .ForMember(dto => dto.CreateTime, opt => opt.Ignore());

            CreateMap<StudentInfo, UserInfoDTO>();

            //CreateMap<StudentInfo, UserInfoDTO>()
            //.ForMember(dto => dto.TestTime, opt => opt.MapFrom(stu => stu.CreateTime));

            //检查dot是否都被映射，否则报错
            //Mapper.AssertConfigurationIsValid();
        }
    }
}
