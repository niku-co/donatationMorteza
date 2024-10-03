using AutoMapper;
using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models;

public class ItemDto : BaseDto<ItemDto, Item>
{
    public long Price { get; set; }
    public bool Active { get; set; }
    public bool Special { get; set; }
    public short? Priority { get; set; }
    public bool? ShowTitle { get; set; }
    public string? IntentionCode { get; set; }
    public IFormFile Thumbnail { get; set; }
    public ICollection<int> CategoriesId { get; set; }
    internal ICollection<Category> Categories { get; set; }
    public ICollection<ItemTranslationDto> ItemTranslations { get; set; }

    public override void CustomMappings(IMappingExpression<Item, ItemDto> mapping)
    {
        mapping.ForMember(
            input => input.CategoriesId,
            config => config.MapFrom(i => i.Categories.Select(i => i.Id)));
    }
}
//public class ItemDtoValidator : AbstractValidator<ItemDto>
//{
//    public ItemDtoValidator()
//    {
//        RuleFor(i => i.Title).NotEmpty().WithMessage("عنوان آیتم را مشخص کنید.");
//    }
//}

public class ItemThumbnail
{
    public int Id { get; set; }
    public IFormFile File { get; set; }
}

public class ItemSelectDto : BaseDto<ItemSelectDto, Item>
{
    public long Price { get; set; }
    public bool Active { get; set; }
    public bool Special { get; set; }
    public short? Priority { get; set; }
    public bool? ShowTitle { get; set; }
    public string ImageName { get; set; }
    public ImageFileDto Thumbnail { get; set; }
    public string ImageType { get; set; }
    public ICollection<CategoryTitleDto> Categories { get; set; }
    public ICollection<ItemTranslationDto> ItemTranslations { get; set; }
    public string? IntentionCode { get; set; }

    public override void CustomMappings(IMappingExpression<Item, ItemSelectDto> mapping)
    {
        mapping.ForMember(
            dest => dest.ImageName,
            config => config.MapFrom(i => i.ItemImage.Name));
        mapping.ForMember(
            dest => dest.ImageType,
            config => config.MapFrom(i => i.ItemImage.FileType));

        mapping.ForMember(
        dest => dest.Thumbnail,
            config => config.MapFrom(i => i.ThumbnailPath.GetFile()));
    }
}

public class ItemPaginationDto : BaseDto<ItemPaginationDto, Item>
{
    public ICollection<ItemMinimalDto> Items { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
public class ItemMinimalDto : BaseDto<ItemMinimalDto, Item>
{
    public string? IntentionCode { get; set; }
    public long Price { get; set; }
    public bool Active { get; set; }
    public bool Special { get; set; }
    public short? Priority { get; set; }
    public bool? ShowTitle { get; set; }
    public ICollection<CategoryTitleDto> Categories { get; set; }
    //public int CategoryId { get; set; }
    public ICollection<ItemTranslationDto> ItemTranslations { get; set; }
}
public class CategoryItemsDto : BaseDto<CategoryItemsDto, Item>
{
    public string? IntentionCode { get; set; }
    public long Price { get; set; }
    public bool Active { get; set; }
    public bool Special { get; set; }
    public short? Priority { get; set; }
    public bool? ShowTitle { get; set; }
    public ICollection<ItemTranslationDto> ItemTranslations { get; set; }
}


public class DataItemDto : BaseDto<DataItemDto, Item>
{
    public string Title { get; set; }
    public long Price { get; set; }
    public long TotalPrice { get; set; }
    public int Count { get; set; }
}

public class DataDetailItemDto  
{
    public string name_family { get; set; }
    public long amount { get; set; }
    public long totalPrice { get; set; }
    public string card_number { get; set; }

    public string transaction_number { get; set; }

    public string provience { get; set; }
    public string Nationalcode { get; set; }
    public string mobile { get; set; }
    public string date_string { get; set; }
    public string purpose_code { get; set; }
    public string source_id { get; set; }
    public string time_string { get; set; }
    public string branch { get; set; }

}

