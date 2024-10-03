using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class ItemTranslation : BaseEntity
    {
        public string Title { get; set; }
        public Language Language { get; set; }
        public Item Item { get; set; }
        public int LanguageId { get; set; }
        public int ItemId { get; set; }
    }
    public class ItemTranslationConfig : IEntityTypeConfiguration<ItemTranslation>
    {
        public void Configure(EntityTypeBuilder<ItemTranslation> builder)
        {
            builder.HasAlternateKey(i => new { i.ItemId, i.LanguageId });
        }
    }
}
