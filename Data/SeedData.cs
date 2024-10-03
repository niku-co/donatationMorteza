using System;
using Microsoft.EntityFrameworkCore;
using Entities;
using Entities.User;
using Microsoft.AspNetCore.Identity;

namespace Data;

public static class SeedData
{
    public const string Admin = "Admin";
    public static void Seed(ModelBuilder builder)
    {
        var adminRole = new Role
        {
            Id = 1,
            Name = Admin,
            NormalizedName = Admin.ToUpper(),
            Description = "Admin Role",
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
        var adminUser = new User
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "admin".ToUpper(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),            
            //Email = "admin@example.com",
            //NormalizedEmail = "ADMIN@EXAMPLE.COM",
            //EmailConfirmed = true,
            PasswordHash = new PasswordHasher<User>().HashPassword(null, "123456"),            
        };       

        Language faLang = new()
        {
            Id = 1,
            CodeName = "Fa",
            Name = "فارسی",
            Description = "لطفا زبان خود را انتخاب کنید",
            ErrorMessage = "زبان فارسی غیر فعال است",
        };
        Language enLang = new()
        {
            Id = 2,
            CodeName = "En",
            Name = "English",
            Description = "Please select your language",
            ErrorMessage = "English is disabled",
        };

        var firstCat = new Category()
        {
            Id = 1,
            DeviceId = "12345",
            Title = "دسته بندی اولیه 1",
        };
        Setting firstSetting = new()
        {
            Id = Guid.NewGuid(),
            CategoryId = 1
        };
        Item firstItem = new()
        {
            Id = 1,            
            Price = 1000,
        };
        ItemTranslation itemTranslation = new()
        {
            Id = 1,
            Title = "آیتم یک",
            LanguageId = 1,
            ItemId = 1
        };
        //firstItem.ItemTranslations = new List<ItemTranslation>() { itemTranslation };       
        //firstCat.Items = new List<Item>() { firstItem };

        builder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>
        {
            RoleId = 1,
            UserId = 1
        });
        builder.Entity<User>().HasData(adminUser);
        builder.Entity<Role>().HasData(adminRole);
        builder.Entity<Language>().HasData(faLang);
        builder.Entity<Language>().HasData(enLang);
        builder.Entity<Setting>().HasData(firstSetting);
        builder.Entity<Item>().HasData(firstItem);
        builder.Entity<ItemTranslation>().HasData(itemTranslation);
        builder.Entity<Category>().HasData(firstCat);
        //builder.Entity<Setting>().HasData(firstSetting);
    }
}