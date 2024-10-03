using System;
using Entities.Common;
using Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities
{
    public class UserProvince : BaseEntity<Guid>
    {
        public int UserId { get; set; }
        public Guid ProvinceId { get; set; }
        public Entities.User.User User { get; set; }
        public Province Province { get; set; }
    }
    public class UserProvinceConfig : IEntityTypeConfiguration<UserProvince>
    {
        public void Configure(EntityTypeBuilder<UserProvince> builder)
        {
            builder.HasKey(i => new { i.UserId, i.ProvinceId });
        }
    }
}
