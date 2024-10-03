using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class ProvinceSetttingDto : BaseDto<ProvinceSetttingDto, ProvinceSettting, Guid>
    {
        public bool ApplyProvince { get; set; }
    }
}
