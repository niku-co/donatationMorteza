using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using Entities.User;
using Mahak.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services;
using WebFramework.Api;

namespace Mahak.Api.Controllers.v1;

[ApiVersion("1")]
public class UsersController : BaseController
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;
    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, IJwtService jwtService,
        UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtService = jwtService;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Obsolete]
    [HttpPut("[action]")]
    [Authorize(Roles = "Admin")]
    public virtual async Task<ActionResult<bool>> ChangePassword([FromBody] UserChangePassword changePassword, CancellationToken cancellationToken)
    {
        var userName = HttpContext.User.Identity.GetUserName();

        var user = await _userManager.FindByNameAsync(userName);
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, changePassword.Password);

        if (result.Succeeded)
            return Ok();
        return BadRequest("عملیات با موفقیت انجام نشد!");
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public virtual async Task<ActionResult<List<User>>> Get(CancellationToken cancellationToken)
    {
        //var userName = HttpContext.User.Identity.GetUserName();
        //userName = HttpContext.User.Identity.Name;
        //var userId = HttpContext.User.Identity.GetUserId();
        //var userIdInt = HttpContext.User.Identity.GetUserId<int>();
        //var phone = HttpContext.User.Identity.FindFirstValue(ClaimTypes.MobilePhone);
        //var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);

        var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [Obsolete]
    public virtual async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
    {
        var user2 = await _userManager.FindByIdAsync(id.ToString());
        var role = await _roleManager.FindByNameAsync("Admin");

        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        if (user == null)
            return NotFound();

        await _userManager.UpdateSecurityStampAsync(user);
        //await userRepository.UpdateSecurityStampAsync(user, cancellationToken);

        return user;
    }

    /// <summary>
    /// This method generate JWT Token
    /// </summary>
    /// <param name="tokenRequest">The information of token request</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("[action]")]
    public virtual async Task<ApiResult<AccessToken>> GetToken([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Current Native DateTime: {0}", NativeDateTime.Now());
        var jwt = await GetTokenAsync(tokenRequest);
        return jwt;
    }
    [AllowAnonymous]
    [HttpPost("[action]")]
    public virtual async Task<ActionResult> Token([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Current Native DateTime: {0}", NativeDateTime.Now());
        var jwt = await GetTokenAsync(tokenRequest);
        return new JsonResult(jwt);
    }
    async Task<AccessToken> GetTokenAsync(TokenRequest tokenRequest)
    {
        var user = await _userManager.FindByNameAsync(tokenRequest.username);
        if (user == null)
            throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.password);
        if (!isPasswordValid)
            throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");


        //if (user == null)
        //    throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است");

        var jwt = await _jwtService.GenerateAsync(user);
        return jwt;
    }
    [HttpPost]
    public virtual async Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
    {
        _logger.LogError("متد Create فراخوانی شد");
        //HttpContext.RiseError(new Exception("متد Create فراخوانی شد"));

        //var exists = await userRepository.TableNoTracking.AnyAsync(p => p.UserName == userDto.UserName);
        //if (exists)
        //    return BadRequest("نام کاربری تکراری است");

        var user = new User
        {
            UserName = userDto.UserName,
            //SecurityStamp = Guid.NewGuid().ToString("D")
        };
        var result = await _userManager.CreateAsync(user, userDto.Password);

        var result2 = await _roleManager.CreateAsync(new Role
        {
            Name = "Admin",
            Description = "admin role",
            //ConcurrencyStamp = Guid.NewGuid().ToString("D")
        });

        var result3 = await _userManager.AddToRoleAsync(user, "Admin");

        //await _userRepository.AddAsync(user, userDto.Password, cancellationToken);
        return user;
    }

    [HttpPut]
    public virtual async Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
    {
        var updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

        updateUser.UserName = user.UserName;
        updateUser.PasswordHash = user.PasswordHash;
        /*updateUser.FullName = user.FullName;
        updateUser.Age = user.Age;*/
        updateUser.Active = user.Active;
        updateUser.LastLoginDate = user.LastLoginDate;

        await _userRepository.UpdateAsync(updateUser, cancellationToken);

        return Ok();
    }

    [HttpDelete]
    [Obsolete]
    public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        await _userRepository.DeleteAsync(user, cancellationToken);

        return Ok();
    }

}
