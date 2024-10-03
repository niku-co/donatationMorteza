using System;
using Common;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    [Auditable]
    public class ItemImage: BaseEntity<Guid>, IImage
    {
        public string Name { get; set; }
        public string PhysicalPath { get; set; }
        public string FileType { get; set; }
        public Item Item { get; set; }
        public int ItemId { get; set; }
    }
    public class ItemImageConfig : IEntityTypeConfiguration<ItemImage>
    {
        public void Configure(EntityTypeBuilder<ItemImage> builder)
        {
            builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
            builder.Property(c => c.PhysicalPath).HasMaxLength(500).IsRequired();
            builder.Property(c => c.FileType).HasMaxLength(10).IsRequired();
        }
    }
}