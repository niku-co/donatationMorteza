using AutoMapper;
using Common.Utilities;
using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models;

public class SettingDto : BaseDto<SettingDto, Setting, Guid>
{
    public bool HasPrice { get; set; }
    public bool RestartIn24Hours { get; set; }
    public bool RestartPerhours { get; set; }
    //public string PhoneNumber { get; set; }
    public bool HasFactor { get; set; }
    public bool HasFreeShopping { get; set; }
    public bool HasPhoneNumber { get; set; }
    public bool MultiSelect { get; set; }
    public PriceUnitType PriceUnitType { get; set; }
    public bool Active { get; set; }
    public int? Hour { get; set; }
    public string BarcodeContent { get; set; }
    public string BackgroundHexCode { get; set; }
    public string ButtonHexCode { get; set; }
    public string TextColorHexCode { get; set; }
    public string HighlightColorHexCode { get; set; }
    public ICollection<LanguageKioskDto> Languages { get; set; }
    public ICollection<SettingTranslationDto> SettingTranslations { get; set; }
}
public class SettingSelectDto : BaseDto<SettingSelectDto, Setting, Guid>
{
    public bool HasPrice { get; set; }
    public bool RestartIn24Hours { get; set; }
    public bool RestartPerhours { get; set; }
    //public string PhoneNumber { get; set; }
    public bool HasFactor { get; set; }
    public bool HasFreeShopping { get; set; }
    public bool HasPhoneNumber { get; set; }
    public bool MultiSelect { get; set; }
    public string PriceUnitType { get; set; }
    public bool Active { get; set; }
    public int? Hour { get; set; }
    public string BarcodeContent { get; set; }
    public string BackgroundHexCode { get; set; }
    public string ButtonHexCode { get; set; }
    public string TextColorHexCode { get; set; }
    public string HighlightColorHexCode { get; set; }
    public string LogoMd5 { get; set; }
    public string FirstScreenMd5 { get; set; }
    public string LastScreenMd5 { get; set; }
    public string BackgroundMd5 { get; set; }
    public string SwipeCardMd5 { get; set; }
    public CategoryTitleDto Category { get; set; }
    public ICollection<LanguageKioskDto> Languages { get; set; }
    public ICollection<SettingTranslationDto> SettingTranslations { get; set; }

    public override void CustomMappings(IMappingExpression<Setting, SettingSelectDto> mapping)
    {
        mapping.ForMember(
            input => input.LogoMd5,
            config => config.MapFrom(i => !string.IsNullOrEmpty(i.LogoPhysicalPath) && File.Exists(i.LogoPhysicalPath)
                ? i.LogoPhysicalPath.CalcMd5()
                : string.Empty));

        mapping.ForMember(
            input => input.FirstScreenMd5,
            config => config.MapFrom(i => !string.IsNullOrEmpty(i.FirstScreenPhysicalPath) && File.Exists(i.FirstScreenPhysicalPath)
                ? i.FirstScreenPhysicalPath.CalcMd5()
                : string.Empty));
        mapping.ForMember(
            input => input.LastScreenMd5,
            config => config.MapFrom(i => !string.IsNullOrEmpty(i.LastScreenPhysicalPath) && File.Exists(i.LastScreenPhysicalPath)
                ? i.LastScreenPhysicalPath.CalcMd5()
                : string.Empty));

        mapping.ForMember(
            input => input.BackgroundMd5,
            config => config.MapFrom(i => !string.IsNullOrEmpty(i.BackgroundPhysicalPath) && File.Exists(i.BackgroundPhysicalPath)
                ? i.BackgroundPhysicalPath.CalcMd5()
                : string.Empty));

        mapping.ForMember(
            input => input.SwipeCardMd5,
            config => config.MapFrom(i => !string.IsNullOrEmpty(i.SwipeCardPhysicalPath) && File.Exists(i.SwipeCardPhysicalPath)
                ? i.SwipeCardPhysicalPath.CalcMd5()
                : string.Empty));
    }

    public class SettingImageDto : BaseDto<SettingImageDto, Setting, Guid>
    {
        public IFormFile LogoImageFile { get; set; }

        public IFormFile FirstScreenImageFile { get; set; }
        public IFormFile LastScreenImageFile { get; set; }
        public IFormFile BackgroundImageFile { get; set; }
        public IFormFile SwipeCardScreenImageFile { get; set; }

        public int[] CatIds { get; set; }
        public string TagIds { get; set; }
        public string ProvinceIds { get; set; }

    }
    public class SettingImageSelectDto : BaseDto<SettingImageSelectDto, Setting, Guid>
    {
        public string Logo { get; set; }
        public byte[] FirstScreen { get; set; }
        public byte[] LastScreen { get; set; }
        public byte[] Background { get; set; }
        public byte[] SwipeCard { get; set; }
    }
    public class FileDto
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
    }
}

public class MultiCatDto
{
    public SettingDto Data { get; set; }
    public string CatIds { get; set; }
    public string TagIds { get; set; }
    public string ProvinceIds { get; set; }
}