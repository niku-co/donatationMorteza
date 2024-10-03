using System;
using Entities;
using WebFramework.Api;

namespace Mahak.Api.Models
{
    public class CartItemDto : BaseDto<CartItemDto, CartItem, Guid>
    {
        public int Quantity { get; set; }
        public int ItemId { get; set; }
    }
    public class CartItemSelectDto : BaseDto<CartItemSelectDto, CartItem, Guid>
    {
        public int Quantity { get; set; }
        public ItemSelectDto Item { get; set; }
    }
}