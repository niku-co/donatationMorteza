using System;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class CartItem : BaseEntity<Guid>
    {
        public int Quantity { get; set; }
        public int ItemId { get; set; }
        public Guid CartId { get; set; }
        public Item Item { get; set; }
        public Cart Cart { get; set; }
    }
    public class CartItemConfig : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(i => new { i.ItemId, i.CartId });
            /*builder.HasOne<Item>().WithMany().HasForeignKey(c => c.ItemId);
            builder.HasOne<Cart>().WithMany(c=>c.CartItems).HasForeignKey(c => c.CartId);*/
        }
    }
}
