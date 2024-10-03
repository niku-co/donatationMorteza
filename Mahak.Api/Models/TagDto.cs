using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class TagDto : BaseDto<TagDto, Tag, Guid>
    {
        public string Title { get; set; }
        public DateTime InsertTime { get; set; }
        public int? TagGroupId { get; set; }

        public TagGroup? TagGroup { get; set; }
    }
}
