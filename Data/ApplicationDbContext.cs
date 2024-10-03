using System;
using Common.Utilities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Entities;
using Entities.Common;
using Entities.User;

namespace Data;

public class ApplicationDbContext : IdentityDbContext<User, Role, int>
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var entitiesAssembly = typeof(IEntity).Assembly;

        //modelBuilder.Entity<Item>()
        //.HasMany(i => i.Categories)
        //.WithMany(c => c.Items)
        //.UsingEntity(j => j.ToTable("CategoryItems"));

        //SeedData.Seed(modelBuilder);
        modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
        modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
        //modelBuilder.AddRestrictDeleteBehaviorConvention();
        modelBuilder.AddSequentialGuidForIdConvention();
        modelBuilder.AddPluralizingTableNameConvention();
        modelBuilder.Entity<Category>()
        .HasMany(p => p.Tags)
        .WithMany(p => p.Categories)
        .UsingEntity<CategoryTag>(
            j => j.HasOne(t => t.Tag).WithMany(p => p.CategoryTags),
            j => j.HasOne(t => t.Category).WithMany(p => p.CategoryTags));

        modelBuilder.AddAuditField();
        SetAuditValues(modelBuilder);
    }

    void SetAuditValues(ModelBuilder builder)
    {
        builder.Entity<Category>()
            .HasQueryFilter(m => EF.Property<bool>(m, "IsRemoved") == false);
        builder.Entity<Item>()
            .HasQueryFilter(m => EF.Property<bool>(m, "IsRemoved") == false);
        builder.Entity<Cart>()
            .HasQueryFilter(m => EF.Property<bool>(m, "IsRemoved") == false);
        builder.Entity<User>()
            .HasQueryFilter(m => EF.Property<bool>(m, "IsRemoved") == false);
    }
    public override int SaveChanges()
    {
        _cleanString();
        SaveAuditFields();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        _cleanString();
        SaveAuditFields();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        _cleanString();
        SaveAuditFields();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _cleanString();
        SaveAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void _cleanString()
    {
        var changedEntities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
        foreach (var item in changedEntities)
        {
            if (item.Entity == null)
                continue;

            var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                var propName = property.Name;
                var val = (string)property.GetValue(item.Entity, null);

                if (val.HasValue())
                {
                    var newVal = val.Fa2En().FixPersianChars();
                    if (newVal == val)
                        continue;
                    property.SetValue(item.Entity, newVal, null);
                }
            }
        }
    }
    void SaveAuditFields()
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(p => p.State == EntityState.Modified ||
                        p.State == EntityState.Added ||
                        p.State == EntityState.Deleted
            );
        foreach (var item in modifiedEntries)
        {
            var entityType = item.Context.Model.FindEntityType(item.Entity.GetType());
            if (entityType != null)
            {
                var inserted = entityType.FindProperty("InsertTime");
                var updated = entityType.FindProperty("UpdateTime");
                var RemoveTime = entityType.FindProperty("RemoveTime");
                var IsRemoved = entityType.FindProperty("IsRemoved");
                if (item.State == EntityState.Added && inserted != null)
                {
                    item.Property("InsertTime").CurrentValue = DateTime.Now;
                }
                if (item.State == EntityState.Modified && updated != null)
                {
                    item.Property("UpdateTime").CurrentValue = DateTime.Now;
                }

                if (item.State == EntityState.Deleted && RemoveTime != null && IsRemoved != null)
                {
                    item.Property("RemoveTime").CurrentValue = DateTime.Now;
                    item.Property("IsRemoved").CurrentValue = true;
                    item.State = EntityState.Modified;
                }
            }

        }
    }

}
