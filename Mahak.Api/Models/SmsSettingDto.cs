using Common;
using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class SmsSettingDto : BaseDto<SmsSettingDto, SMSSetting, Guid>
    {
        public bool AllowSendingSms { get; set; }
        public string APIKey { get; set; }
        public string SmsNumber { get; set; }
        public string SmsMessage { get; set; }
        //public string SmsUrl { get; set; }

        public int ServiceType { get; set; }
        public string SmsUsername { get; set; }
        public string SmsPassword { get; set; }

    }
}
