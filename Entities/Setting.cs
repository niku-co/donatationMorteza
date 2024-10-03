using System;
using System.Collections.Generic;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class Setting : BaseEntity<Guid>
    {
        public bool HasPrice { get; set; }
        public bool RestartIn24Hours { get; set; }
        public bool RestartPerhours { get; set; }
       // public string PhoneNumber { get; set; }
        public bool HasFactor { get; set; }
        public bool HasFreeShopping { get; set; }
        public bool HasPhoneNumber { get; set; }
        public bool MultiSelect { get; set; }
        public string PriceUnitType { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public bool? Active { get; set; }
        public int? Hour { get; set; }
        public string BarcodeContent { get; set; }
        public string BackgroundHexCode { get; set; }
        public string ButtonHexCode { get; set; }
        public string TextColorHexCode { get; set; }
        public string HighlightColorHexCode { get; set; }
        public string LogoPhysicalPath { get; set; }
        public string FirstScreenPhysicalPath { get; set; }
        public string LastScreenPhysicalPath { get; set; }
        public string BackgroundPhysicalPath { get; set; }
        public string SwipeCardPhysicalPath { get; set; }
        public ICollection<SettingTranslation> SettingTranslations { get; set; }
        public ICollection<Language> Languages { get; set; }

        
    }

    public class SettingConfig : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.Property(i => i.PriceUnitType).HasConversion<string>();
            builder.Property(i => i.PriceUnitType).HasMaxLength(15);
            builder.Property(i => i.PriceUnitType).HasDefaultValue(PriceUnitType.Rial);
            builder.Property(i => i.Active).HasDefaultValue(true);
            builder.Property(i => i.Hour).HasDefaultValue(24);
            builder.Property(i => i.BarcodeContent).HasDefaultValue("https://mahak-charity.org");
            builder.Property(i => i.BackgroundHexCode).HasMaxLength(6);
            //builder.Property(c => c.LogoPhysicalPath).HasMaxLength(100);
            //builder.Property(c => c.FirstScreenPhysicalPath).HasMaxLength(100);
            //builder.Property(c => c.LastScreenPhysicalPath).HasMaxLength(100);

            //builder.HasOne(i => i.CategoryLogo)
            //    .WithOne(i => i.Setting)
            //    .HasForeignKey<CategoryLogo>(i => i.SettingId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
