using AutoMapper;
using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class UserTagDto : BaseDto<UserTagDto, UserTag, Guid>
    {
        public int UserId { get; set; }
        public Guid TagId { get; set; }
        public UserDto User { get; set; }
        public TagDto Tag { get; set; }

        public override void CustomMappings(IMappingExpression<UserTag, UserTagDto> mapping)
        {
            mapping.ForMember(
                dest => dest.User,
                config => config.Ignore());

            
        }

    }
}
