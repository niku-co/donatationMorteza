using Entities;
using System;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class SettingTranslationDto: BaseDto<SettingTranslationDto, SettingTranslation>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string InputMessage { get; set; }
        public string ThanksMessage { get; set; }
        public int LanguageId { get; set; }
        //public Guid SettingId { get; set; }
    }
}
