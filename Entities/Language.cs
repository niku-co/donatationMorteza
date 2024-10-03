using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Entities
{
    public class Language : BaseEntity
    {
        public string CodeName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ErrorMessage { get; set; }
        public bool? Active { get; set; }

        //public string Image { get; set; }
        public ICollection<Setting> Settings { get; set; }
    }
    public class LanguageConfig : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.Property(i => i.CodeName).HasMaxLength(2).IsUnicode(false);
            builder.Property(i => i.Name).HasMaxLength(20).IsUnicode(true);
            builder.HasAlternateKey(i => i.CodeName);
            builder.HasAlternateKey(i => i.Name);
            builder.Property(i=>i.Active).HasDefaultValue(true);
        }
    }

}
