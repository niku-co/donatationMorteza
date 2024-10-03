using Entities.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{

    public class CategoryTag : IEntity
    {
        [ForeignKey("CategoriesId")]
        public int CategoriesId { get; set; }
        [ForeignKey("TagsId")]
        public Guid TagsId { get; set; }
        public Category Category { get; set; }
        public Tag Tag { get; set; }
    }

    public class CategoryTagConfig : IEntityTypeConfiguration<CategoryTag>
    {
        public void Configure(EntityTypeBuilder<CategoryTag> builder)
        {
            builder.HasKey(i => new { i.TagsId, i.CategoriesId });
        }
    }

}
