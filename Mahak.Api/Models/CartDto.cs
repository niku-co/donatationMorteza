using AutoMapper;
using Common.Utilities;
using Entities;
using System.IO.Packaging;
using WebFramework.Api;

namespace Mahak.Api.Models;

public class CartDto : BaseDto<CartDto, Cart, Guid>
{
    public long TotalPrice { get; set; }
    public long AffectiveAmount { get; set; }
    public string PhoneNumber { get; set; }
    public ICollection<CartItemDto>? CartItems { get; set; }
    public int CategoryId { get; set; }
    public int OrderNum { get; set; }
    public string RefNum { get; set; }
    public string ResNum { get; set; }
    public string TerminalId { get; set; }
    public string TraceNumber { get; set; }
    public string CardNumber { get; set; }
    public long DateTime { get; set; }
    public bool ShoppingFree { get; set; }
    public bool Paid { get; set; } = false;
}
public class CartSelectDto : BaseDto<CartSelectDto, Cart, Guid>
{
    public int OrderNum { get; set; }
    public long TotalPrice { get; set; }
    public long TotalCount { get; set; }
    public string PhoneNumber { get; set; }
    public ICollection<CartItemsDto> CartItems { get; set; }
    //public ICollection<CartItemSelectDto> CartItems { get; set; }
    public CategoryTitleDto Category { get; set; }
    //public Category Category { get; set; }
    public string AffectiveAmount { get; set; }
    public string RefNum { get; set; }
    public string ResNum { get; set; }
    public string TerminalId { get; set; }
    public string TraceNumber { get; set; }
    public string CardNumber { get; set; }
    public long DateTime { get; set; }
    public string PerDateTime { get; set; }
    public bool ShoppingFree { get; set; }
    public bool Paid { get; set; } = false;

    public override void CustomMappings(IMappingExpression<Cart, CartSelectDto> mapping)
    {
        mapping.ForMember(
            dest => dest.PerDateTime,
            config => config.MapFrom(i => i.DateTime.ToLongStringFormat()));
    }
}
public class CategoryTitleDto : BaseDto<CategoryTitleDto, Category>
{
    public string DeviceId { get; set; }
    public string Title { get; set; }
    public Guid? ProvinceId { get; set; }
    public ICollection<Tag> Tags { get; set; }
}
public class CartItemsDto : BaseDto<CartItemsDto, CartItem, Guid>
{
    public int Quantity { get; set; }
    public ItemTitleDto Item { get; set; }
}
public class ItemTitleDto : BaseDto<ItemTitleDto, Item>
{
    public long Price { get; set; }
    public ICollection<ItemTranslationDto> ItemTranslations { get; set; }
}
public class CartPaginationDto : BaseDto<CartPaginationDto, Cart, Guid>
{
    public ICollection<Cart> Items { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
public class CartChartResult
{
    public IEnumerable<string> Label { get; set; }
    public IEnumerable<long> Data { get; set; }
}
public class CartChartDto : BaseDto<CartChartDto, Cart, Guid>
{
    //public string Title { get; set; }
    public Category Category { get; set; }
    public long TotalPrice { get; set; }
    public string PerDateTime { get; set; }
    public long DateTime { get; set; }
    public bool Paid { get; set; } = false;
    public override void CustomMappings(IMappingExpression<Cart, CartChartDto> mapping)
    {
        mapping.ForMember(
            dest => dest.PerDateTime,
            config => config.MapFrom(i => i.DateTime.ToShortStringFormat()));
    }
}

public class CartSummerySelectDto : BaseDto<CartSummerySelectDto, Cart, Guid>
{
    public double TotalPrice { get; set; }
    public double AveragePrice { get; set; }
    public double TotalCount { get; set; }
    public double TotalTrabsactionCount { get; set; }
    public double TotalTrabsactionPrice { get; set; }

    public double DailyTrabsactionCount { get; set; }
    public double DailyTrabsactionPrice { get; set; }

    public string CategoryTitle { get; set; }
    public int CategoryId { get; set; }
    public decimal PosUpTime { get; set; }
    public decimal UpTime { get; set; }
    public int TurnOnHours { get; set; }

}
