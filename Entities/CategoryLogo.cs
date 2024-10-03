//using Common;
//using Entities.Common;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using Microsoft.EntityFrameworkCore;
//using System;

//namespace Entities;

//[Auditable]
//public class CategoryLogo : BaseEntity<Guid>, IImage
//{
//    public string Name { get; set; }
//    public string PhysicalPath { get; set; }
//    public string FileType { get; set; }
//    public Setting Setting { get; set; }
//    public Guid SettingId { get; set; }
//}
//public class CategoryLogoConfig : IEntityTypeConfiguration<CategoryLogo>
//{
//    public void Configure(EntityTypeBuilder<CategoryLogo> builder)
//    {
//        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
//        builder.Property(c => c.PhysicalPath).HasMaxLength(500).IsRequired();
//        builder.Property(c => c.FileType).HasMaxLength(10).IsRequired();
//    }
//}
//[Auditable]
//public class CategoryFirstScreen : BaseEntity<Guid>, IImage
//{
//    public string Name { get; set; }
//    public string PhysicalPath { get; set; }
//    public string FileType { get; set; }
//    public Setting Setting { get; set; }
//    public Guid SettingId { get; set; }
//}
//public class CategoryFirstScreenConfig : IEntityTypeConfiguration<CategoryLogo>
//{
//    public void Configure(EntityTypeBuilder<CategoryLogo> builder)
//    {
//        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
//        builder.Property(c => c.PhysicalPath).HasMaxLength(500).IsRequired();
//        builder.Property(c => c.FileType).HasMaxLength(10).IsRequired();
//    }
//}
//[Auditable]
//public class CategoryLastScreen : BaseEntity<Guid>, IImage
//{
//    public string Name { get; set; }
//    public string PhysicalPath { get; set; }
//    public string FileType { get; set; }
//    public Setting Setting { get; set; }
//    public Guid SettingId { get; set; }
//}
//public class CategoryLastScreenConfig : IEntityTypeConfiguration<CategoryLogo>
//{
//    public void Configure(EntityTypeBuilder<CategoryLogo> builder)
//    {
//        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
//        builder.Property(c => c.PhysicalPath).HasMaxLength(500).IsRequired();
//        builder.Property(c => c.FileType).HasMaxLength(10).IsRequired();
//    }
//}