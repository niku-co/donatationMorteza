using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class TagGroupDto : BaseDto<TagGroupDto, TagGroup, int>
    {
        public string Title { get; set; }
        public DateTime InsertTime { get; set; }
    }
}
