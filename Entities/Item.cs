using System.Collections.Generic;
using Common;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    [Auditable]
    public class Item : BaseEntity
    {
        //public string Title { get; set; }
        //public string Description { get; set; }
        public long Price { get; set; }
        public bool? Active { get; set; }
        public bool Special { get; set; }
        public short? Priority { get; set; }
        public bool? ShowTitle { get; set; }
        public string? IntentionCode { get; set; }
        public string? ThumbnailPath { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ItemImage? ItemImage { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<ItemTranslation> ItemTranslations { get; set; }
    }

    [Auditable]
    public class CategoryItem
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
    }
    public class ItemConfig : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            /*builder.HasOne(i => i.CartItem)
                .WithOne(i => i.Item)
                .HasForeignKey<CartItem>(i => i.ItemId);*/
            builder.Property(i => i.ShowTitle).HasDefaultValue(true);
            builder.Property(i => i.Active).HasDefaultValue(true);
            builder.Property(i => i.IntentionCode).HasMaxLength(50);
            builder.HasOne(i => i.ItemImage)
                .WithOne(i => i.Item)
                .HasForeignKey<ItemImage>(i => i.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
