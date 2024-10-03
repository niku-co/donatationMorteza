using AutoMapper;
using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class UserProvinceDto : BaseDto<UserProvinceDto, UserProvince, Guid>
    {
        public int UserId { get; set; }
        public Guid ProvinceId { get; set; }
        public UserDto User { get; set; }
        public ProvinceDto Province { get; set; }

        public override void CustomMappings(IMappingExpression<UserProvince, UserProvinceDto> mapping)
        {
            mapping.ForMember(
                dest => dest.User,
                config => config.Ignore());

            
        }

    }
}
