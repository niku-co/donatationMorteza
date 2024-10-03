using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities
{
    public class SettingTranslation : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string InputMessage { get; set; }
        public string ThanksMessage { get; set; }
        public Language Language { get; set; }
        public Setting Setting { get; set; }
        public int LanguageId { get; set; }
        public Guid SettingId { get; set; }
    }
    public class SettingTranslationConfig : IEntityTypeConfiguration<SettingTranslation>
    {
        public void Configure(EntityTypeBuilder<SettingTranslation> builder)
        {
            builder.Property(i => i.InputMessage).HasMaxLength(150);
            builder.Property(i => i.ThanksMessage).HasMaxLength(300);
        }
    }
}
