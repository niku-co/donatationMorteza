using System.Collections.Generic;
using Entities;
using System.Linq;
using Data.Contracts;
using Microsoft.AspNetCore.Hosting;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.DynamicAuthorization.DTOs.Claims;

namespace Services.DataInitializer;

public class DataInitializer : IDataInitializer
{
    private readonly IRepository<Category> _repository;
    private readonly IRepository<Language> _langRepository;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    public DataInitializer(IRepository<Category> repository, IRepository<Language> langRepository, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _repository = repository;
        _langRepository = langRepository;
        _userManager = userManager;
        _roleManager = roleManager;
    }


    public void InitializeData()
    {
        if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new Role { Name = "Admin", Description = "Admin role" }).GetAwaiter().GetResult();
        }

        if (!_roleManager.RoleExistsAsync("User").GetAwaiter().GetResult())
        {
            _roleManager.CreateAsync(new Role { Name = "User", Description = "User role" }).GetAwaiter().GetResult();
        }
        //var adminUser = _userManager.FindByEmailAsync("admin@site.com").GetAwaiter().GetResult();
        if (!_userManager.Users.AsNoTracking().Any(p => p.UserName == "admin"))
        {
            var user = new User
            {
                UserName = "admin",
                Email = "admin@site.com",
            };
            _userManager.CreateAsync(user, "123456").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
        }
        //if(!_userManager.GetClaimsAsync(adminUser).GetAwaiter().GetResult().Any())
        //{
        //    var claims = ClaimStore.AllClaims.SelectMany(i=>i.Claims.Select(j=>j.));
        //    _userManager.AddClaimsAsync(adminUser, claims).GetAwaiter().GetResult();
        //}


        //var val = _configuration["kiosk"];
        if (!_userManager.Users.AsNoTracking().Any(p => p.UserName == "nikukiosk"))
        {
            var user = new User
            {
                UserName = "nikukiosk",
                Email = "nikukiosk@site.com",
            };

            _userManager.CreateAsync(user, "4ee93af4-0c3d-4791-9b7a-4c512ac4c87c").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
        }

        var find = _langRepository.TableNoTracking.FirstOrDefault(p => p.CodeName == "Fa");
        if (find is null)
        {
            Language language = new()
            {
                CodeName = "Fa",
                Name = "فارسی",
                Description = "لطفا زبان خود را انتخاب کنید",
                ErrorMessage = "زبان فارسی غیر فعال است",
            };
            _langRepository.Add(language);
        }
        find = _langRepository.TableNoTracking.FirstOrDefault(p => p.CodeName == "En");
        if (find is null)
        {
            Language language = new()
            {
                CodeName = "En",
                Name = "English",
                Description = "Please select your language",
                ErrorMessage = "English is disabled",
            };
            _langRepository.Add(language);
        }
        if (!_repository.TableNoTracking.Any(p => p.Title == "دسته بندی اولیه 1"))
        {
            var cat = new Category()
            {
                DeviceId = "12345",
                Title = "دسته بندی اولیه 1",
                Items = new List<Item>()
                {
                    new()
                    {
                        ItemTranslations = new List<ItemTranslation>
                        {
                            new ItemTranslation
                            {
                                Title = "آیتم یک",
                                LanguageId = 1,
                                ItemId = 1
                            }
                        },
                        Price = 1000,
                    },
                },
                Setting = new Setting() { }

            };

            _repository.Add(cat);
        }
    }


}

