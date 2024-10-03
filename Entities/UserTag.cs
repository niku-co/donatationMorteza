using System;
using Entities.Common;
using Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class UserTag : BaseEntity<Guid>
    {
        public int UserId { get; set; }
        public Guid TagId { get; set; }
        public Entities.User.User User { get; set; }
        public Tag Tag { get; set; }
    }
    public class UserTagConfig : IEntityTypeConfiguration<UserTag>
    {
        public void Configure(EntityTypeBuilder<UserTag> builder)
        {
            builder.HasKey(i => new { i.UserId, i.TagId });
        }
    }
}
