using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Common;
using Entities.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Entities;

[Auditable]
public class Category : BaseEntity
{
    public Category()
    {
        LastRequest = DateTime.Now;
    }
    public string DeviceId { get; set; }
    public string Title { get; set; }
    public string Ip { get; set; }
    public string TerminalNo { get; set; }
    public string? LocName { get; set; }
    public bool? State { get; set; }
    public string? ThumbnailPath { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime LastRequest { get; set; }
    public ICollection<Item> Items { get; set; }
    public ICollection<Cart> Carts { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public Setting Setting { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ConnectionId { get; set; }
    public string? ApkVersion { get; set; }
    public string? AnydeskCode { get; set; }
    public string? SimCardNumber { get; set; }
    public string? AgentCode { get; set; }
    public string? AgentFullname { get; set; }
    public string? Branch { get; set; }
    public string? AgentMobile { get; set; }
    public Guid? ProvinceId { get; set; }
    public Guid? CategoryAgentId { get; set; }
    public Province? Province { get; set; }
    public CategoryAgent? CategoryAgent { get; set; }
    
    public IList<CategoryTag> CategoryTags { get; } = new List<CategoryTag>();
}
public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasOne(i => i.Setting)
            .WithOne(i => i.Category)
            .HasForeignKey<Setting>(i => i.CategoryId);

        //builder.Property(i => i.Hour).IsRequired();
        builder.Property(i => i.Title).IsRequired();
        builder.Property(i => i.State).HasDefaultValue(true);
        builder.Property(i => i.ConnectionId).HasMaxLength(50);
        builder.Property(i => i.ApkVersion).HasMaxLength(15);
        builder.Property(i => i.AnydeskCode).HasMaxLength(50);
        builder.Property(i => i.SimCardNumber).HasMaxLength(11);
        builder.Property(i => i.AgentCode).HasMaxLength(50);
        builder.Property(i => i.AgentFullname).HasMaxLength(100);
        builder.Property(i => i.AgentMobile).HasMaxLength(11);
        //builder.Property(i => i.LastRequest).HasDefaultValue(DateTime.Now);

        //builder.Property(i => i.State).HasComputedColumn(Math.Round((DateTime.UtcNow - i).TotalHours, 2), true);
        //builder.HasIndex(i => i.DeviceId).IsUnique();

    }
}
