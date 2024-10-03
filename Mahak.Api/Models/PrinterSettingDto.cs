using Common;
using Entities;
using WebFramework.Api;
using static Mahak.Api.Models.SettingSelectDto;

namespace Mahak.Api.Models
{
    public class PrinterSettingDto : BaseDto<PrinterSettingDto, PrinterSetting, Guid>
    {
        public bool ActivePrinter { get; set; }
        public string PrintText { get; set; }
        public string QRCodeUrl { get; set; }
        public FileDto logoValue { get; set; }
        public IFormFile LogoImageFile { get; set; }

    }
}
