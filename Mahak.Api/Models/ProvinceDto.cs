using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class ProvinceDto : BaseDto<ProvinceDto, Province, Guid>
    {
        public int Code { get; set; }
        public string Name { get; set; }

       // public ICollection<CategoryDto> Categories { get; set; }
    }
}
