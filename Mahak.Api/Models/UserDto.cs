using AutoMapper;
using Services.DynamicAuthorization.DTOs.Claims;
using Entities.User;
using WebFramework.Api;
using System.ComponentModel.DataAnnotations;
using Entities;

namespace Mahak.Api.Models;

public class UserDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }    
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IEnumerable<ClaimCollection>? Claims { get; set; }
    public IEnumerable<Guid>? Provinces { get; set; }
    public IEnumerable<Guid>? Tags { get; set; }
}
public class UserUpdateDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string UserName { get; set; }    
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public IEnumerable<ClaimCollection>? Claims { get; set; }
    public IEnumerable<Guid>? Provinces { get; set; }
    public IEnumerable<Guid>? Tags { get; set; }
}
public class UserChangePassword
{
    public string Password { get; set; }
}
public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}

public class UserSelectDto : BaseDto<UserSelectDto, User>
{
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public IReadOnlyList<ClaimCollectionDto>? ClaimCollections { get; set; }
    public IReadOnlyList<UserProvinceDto>? Provinces { get; set; }
    public IReadOnlyList<UserTagDto>? Tags { get; set; }

    public override void CustomMappings(IMappingExpression<User, UserSelectDto> mapping)
    {
        mapping.ForMember(dest => dest.FullName, src => src.MapFrom(i => i.FirstName + " " + i.LastName));
    }
}
public class UserPaginationDto : BaseDto<UserPaginationDto, User>
{
    public ICollection<UserSelectDto> Items { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}
