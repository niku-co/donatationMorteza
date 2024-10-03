using AutoMapper;
using Entities;
using FluentValidation;
using System.Text;
using WebFramework.Api;
using Item = Entities.Item;

namespace Mahak.Api.Models;

public class CategoryDto : BaseDto<CategoryDto, Category>
{
    public string DeviceId { get; set; }
    public string Title { get; set; }
    public string Ip { get; set; }
    public string TerminalNo { get; set; }
    public string? LocName { get; set; }
    public string? ErrorMessage { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public IFormFile Thumbnail { get; set; }
    public string? ApkVersion { get; set; }
    public string? AnydeskCode { get; set; }
    public string? SimCardNumber { get; set; }
    public string? AgentCode { get; set; }
    public string? AgentFullname { get; set; }
    public string? AgentMobile { get; set; }
    public string? TagIds { get; set; }

    public Guid? ProvinceId { get; set; }
    public Guid? CategoryAgentId { get; set; }
}
public class CategoryUpdateDto : BaseDto<CategoryUpdateDto, Category>
{
    public string DeviceId { get; set; }
    public string Title { get; set; }
    public string Ip { get; set; }
    public string TerminalNo { get; set; }
    public string? LocName { get; set; }
    public string? ErrorMessage { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? ApkVersion { get; set; }
    public string? AnydeskCode { get; set; }
    public string? SimCardNumber { get; set; }
    public string? AgentCode { get; set; }
    public string? AgentFullname { get; set; }
    public string? AgentMobile { get; set; }
    public Guid ProvinceId { get; set; }
    public string? Branch { get; set; }
    public Guid[] TagIds { get; set; }
    public Guid? CategoryAgentId { get; set; }
    // public ProvinceDto Province { get; set; }
}
public class CategoryErrorDto : BaseDto<CategoryErrorDto, Category>
{
    public string DeviceId { get; set; }
    public string? ErrorMessage { get; set; }

}
public class CategoryPingDto : BaseDto<CategoryPingDto, CategoryPingLog, Guid>
{
    public string DeviceId { get; set; }
    public long TotalCounter { get; set; }
    public long SuccessCounter { get; set; }
    public DateTimeOffset Date { get; set; }
    public int TurnOnHours { get; set; }
    public long PosTotalCounter { get; set; }
    public long PosSuccessCounter { get; set; }

}

public class CategorySelectErrorDto : BaseDto<CategorySelectErrorDto, Category>
{
    public string DeviceId { get; set; }
    public string? ErrorMessage { get; set; }

}
public class CategorySelectDto : BaseDto<CategorySelectDto, Category>
{
    public string DeviceId { get; set; }
    public string Title { get; set; }
    public bool Active { get; set; }
    public string Ip { get; set; }
    public string TerminalNo { get; set; }
    public string? LocName { get; set; }
    public DateTime LastRequest { get; set; }
    public SettingSelectDto Setting { get; set; }
    public bool? State { get; set; }
    public string? ErrorMessage { get; set; }
    public int Hour { get; set; }
    public string LastActive { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public ImageFileDto Thumbnail { get; set; }
    public ICollection<CategoryItemsDto> Items { get; set; }
    public ICollection<TagDto> Tags { get; set; }
    public string? ApkVersion { get; set; }
    public string? AnydeskCode { get; set; }
    public string? SimCardNumber { get; set; }
    public string? AgentCode { get; set; }
    public string? AgentFullname { get; set; }
    public string? AgentMobile { get; set; }
    public string? Branch { get; set; }
    public Guid? ProvinceId { get; set; }
    public ProvinceDto? Province { get; set; }
    public Guid? CategoryAgentId { get; set; }
    public CategoryAgent? CategoryAgent { get; set; }
    public override void CustomMappings(IMappingExpression<Category, CategorySelectDto> mapping)
    {
        mapping.ForMember(
            dest => dest.State,
            config => config.MapFrom(i => ((DateTime.Now - i.LastRequest).TotalHours) <= (double)i.Setting.Hour ? true : false));

        mapping.ForMember(
        dest => dest.LastActive,
            config => config.MapFrom(i => i.LastRequest.CalcDifference()));

        mapping.ForMember(
        dest => dest.Thumbnail,
            config => config.MapFrom(i => i.ThumbnailPath.GetFile()));
        mapping.ForMember(
        dest => dest.Active,
            config => config.MapFrom(i => i.Setting.Active));
    }
}

public class CategorySearchDto : BaseDto<CategorySearchDto, Category>
{
    public string DeviceId { get; set; }
    public string Title { get; set; }
    public bool Active { get; set; }
    public string Ip { get; set; }
    public string TerminalNo { get; set; }
    public string? LocName { get; set; }
    //public SettingSelectDto Setting { get; set; }
    public bool? State { get; set; }
    public string? ErrorMessage { get; set; }
    public int Hour { get; set; }
    public string LastActive { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public ImageFileDto Thumbnail { get; set; }
    //public ICollection<ItemSelectDto> Items { get; set; }
    public string? ApkVersion { get; set; }
    public string? AnydeskCode { get; set; }
    public string? SimCardNumber { get; set; }
    public string? AgentCode { get; set; }
    public string? Branch { get; set; }
    public string? AgentFullname { get; set; }
    public string? AgentMobile { get; set; }

    public Guid? CategoryAgentId { get; set; }
    public CategoryAgent? CategoryAgent { get; set; }
    public override void CustomMappings(IMappingExpression<Category, CategorySearchDto> mapping)
    {
        mapping.ForMember(
            dest => dest.State,
            config => config.MapFrom(i => ((DateTime.Now - i.LastRequest).TotalHours) <= (double)i.Setting.Hour ? true : false));

        mapping.ForMember(
        dest => dest.LastActive,
            config => config.MapFrom(i => i.LastRequest.CalcDifference()));

        mapping.ForMember(
        dest => dest.Thumbnail,
            config => config.MapFrom(i => i.ThumbnailPath.GetFile()));
        mapping.ForMember(
        dest => dest.Active,
            config => config.MapFrom(i => i.Setting.Active));
    }
}
public class CategoryPaginationDto : BaseDto<CategoryPaginationDto, Category>
{
    public ICollection<CategorySelectDto> Items { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public string LastActive { get; set; }
    //public string? Thumbnail { get; set; }
    public override void CustomMappings(IMappingExpression<Category, CategoryPaginationDto> mapping)
    {
        mapping.ForMember(
        dest => dest.LastActive,
            config => config.MapFrom(i => i.LastRequest.CalcDifference()));
    }
}
//public class CategoryPageDto : BaseDto<CategoryPageDto, Category>
//{
//    public string DeviceId { get; set; }
//    public string Title { get; set; }
//    public bool Active { get; set; }
//    public string Ip { get; set; }
//    public string TerminalNo { get; set; }
//    public string? LocName { get; set; }
//    //public SettingSelectDto Setting { get; set; }
//    public bool? State { get; set; }
//    public string? ErrorMessage { get; set; }
//    public int Hour { get; set; }
//    public string LastActive { get; set; }
//    public double? Latitude { get; set; }
//    public double? Longitude { get; set; }
//    public ImageFileDto Thumbnail { get; set; }
//    public ICollection<Item> Items { get; set; }
//    public override void CustomMappings(IMappingExpression<Category, CategoryPageDto> mapping)
//    {
//        mapping.ForMember(
//            dest => dest.State,
//            config => config.MapFrom(i => ((int)(DateTime.Now - i.LastRequest).TotalHours) <= i.Setting.Hour ? true : false));

//        mapping.ForMember(
//        dest => dest.LastActive,
//            config => config.MapFrom(i => i.LastRequest.CalcDifference()));

//        mapping.ForMember(
//        dest => dest.Thumbnail,
//            config => config.MapFrom(i => i.ThumbnailPath.GetFile()));
//        mapping.ForMember(
//        dest => dest.Active,
//            config => config.MapFrom(i => i.Setting.Active));
//    }
//}
//public class KioskCategorySelectDto : BaseDto<KioskCategorySelectDto, Category>
//{
//    public string DeviceId { get; set; }
//    public string Title { get; set; }
//    public string? LocName { get; set; }
//    public string? ErrorMessage { get; set; }
//    public SettingSelectDto Setting { get; set; }
//    public ICollection<ItemSelectDto> Items { get; set; }
//}
public class CategoryValidator : AbstractValidator<CategoryDto>
{
    public CategoryValidator(Data.Contracts.IRepository<Category> repository)
    {
        RuleFor(i => i.DeviceId)
            .Must(s => repository.TableNoTracking.All(i => !i.DeviceId.Equals(s, StringComparison.InvariantCultureIgnoreCase)))
            .WithMessage("شناسه دستگاه نمی‌تواند تکراری باشد!");
    }
}
public class CategoryChartDto : BaseDto<CategoryChartDto, Category>
{
    public string DeviceId { get; set; }
    public string Title { get; set; }
    public string Ip { get; set; }
    public string TerminalNo { get; set; }
    public string LastActive { get; set; }
    public ICollection<Item> Items { get; set; }
    public ICollection<Cart> Carts { get; set; }
    public override void CustomMappings(IMappingExpression<Category, CategoryChartDto> mapping)
    {
        mapping.AfterMap((src, dest, context) => context.Mapper.Map(src.Items.SelectMany(i => i.CartItems.Select(j => j.Cart)), dest.Carts));
        mapping.ForMember(
        dest => dest.LastActive,
            config => config.MapFrom(i => i.LastRequest.CalcDifference()));
    }
}
public class CategoryThumbnail
{
    public int Id { get; set; }
    public IFormFile File { get; set; }
}
public class ImageFileDto
{
    public string Content { get; set; }
    public string FileName { get; set; }
}
public class CategoryMapData
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public IEnumerable<CategoryMapDataItem> CategoryMapDataItems { get; set; }
    public int Count { get; set; }
}

public class CategoryPieChartData
{
    public bool Active { get; set; }
    public long Count { get; set; }
    public bool State { get; set; }

}

public class CartLineChartData
{
    public long[] Data { get; set; }
    public int[] Count { get; set; }
    public string[] Label { get; set; }

}

public class DashboardData
{
    public int DailyTransaction { get; set; }
    public long DailyAmountTransaction { get; set; }
    public int TotalActiveCategory { get; set; }
    public int TotalItems { get; set; }

}


public class CategoryMapDataItem
{
    public string? Title { get; set; }
    public string? DeviceId { get; set; }
    public string? LocName { get; set; }
    public bool? State { get; set; }
    public bool? Active { get; set; }
}
public static class ExtensionMethods
{
    public static string CalcDifference(this DateTime lastRequest)
    {
        var difference = (DateTime.Now - lastRequest);
        var totalMinutes = Math.Round(difference.TotalMinutes);

        var days = (int)totalMinutes / 60 / 24;
        var hours = (int)((int)totalMinutes - days * 60 * 24) / 60;
        var minutes = ((int)totalMinutes - days * 60 * 24 - hours * 60);

        StringBuilder sb = new();

        if (days == 0 && hours == 0 && minutes == 0)
            return string.Empty;

        if (days > 0)
            sb.Append($"{days} روز و ");
        if (hours > 0)
            sb.Append($"{hours} ساعت و ");
        if (minutes > 0)
            sb.Append($"{minutes} دقیقه ");

        var result = $"{sb.ToString().Trim(' ').Trim('و')}";
        if (!string.IsNullOrEmpty(result))
            result += " پیش";

        return result;
    }

    public static ImageFileDto GetFile(this string filePath)
    {
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            using var fileStream = new FileStream(filePath, FileMode.Open);
            byte[] imageData = new byte[fileStream.Length];
            fileStream.Read(imageData, 0, (int)fileStream.Length);
            string base64Image = Convert.ToBase64String(imageData);

            return new ImageFileDto
            {
                Content = base64Image,
                FileName = Path.GetFileName(filePath)
            };
        }
        else
            return null;
    }

}
