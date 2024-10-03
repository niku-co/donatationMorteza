using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class ItemTranslationDto : BaseDto<ItemTranslationDto, ItemTranslation>
    {
        public string Title { get; set; }
        public int LanguageId { get; set; }
        public int ItemId { get; set; }
    }
}
