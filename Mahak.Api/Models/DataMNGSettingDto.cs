using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class DataMNGSettingDto : BaseDto<DataMNGSettingDto, DataMNGSetting, Guid> 
    {
        public string APIUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
