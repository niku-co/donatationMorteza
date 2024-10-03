using Common;
using Entities.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities.User;

[Auditable]
public class User : IdentityUser<int>, IEntity<int>
{
    public override string UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{FirstName} {LastName}".Trim();
    //public virtual ICollection<UserRole>? Roles { get; set; }
    public bool? Active { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    //public ICollection<Claim>? Claims { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(p => p.UserName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Active).HasDefaultValue(true);
    }
}
