using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data;
using Data.Contracts;
using Entities;
using Entities.User;
using Mahak.Api.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.DynamicAuthorization;
using Services.DynamicAuthorization.DTOs.Claims;
using Services.Services;
using WebFramework.Api;

namespace Mahak.Api.Controllers.v2;

[ApiVersion("2")]
public class UsersController : v1.UsersController
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<v1.UsersController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IMediator _mediator;
    private readonly IRepository<UserProvince> _repository;
    private readonly IRepository<UserTag> _uTRepository;
    private readonly IMapper _mapper;

    public UsersController(ApplicationDbContext dbContext, IUserRepository userRepository, ILogger<v1.UsersController> logger, IJwtService jwtService,
        UserManager<User> userManager, RoleManager<Role> roleManager, IMediator mediator, IRepository<UserProvince> repository, IMapper mapper, IRepository<UserTag> uTRepository)
        : base(userRepository, logger, jwtService, userManager, roleManager)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _logger = logger;
        _userManager = userManager;
        _mediator = mediator;
        _repository = repository;
        _mapper = mapper;
        _uTRepository = uTRepository;
    }
    [Obsolete]
    public override async Task<ActionResult<bool>> ChangePassword([FromBody] UserChangePassword changePassword, CancellationToken cancellationToken)
    {
        return await base.ChangePassword(changePassword, cancellationToken);
    }
    [HttpPut("[action]")]
    public virtual async Task<IActionResult> ChangePasswordWithConfirm([FromBody] ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.UserName);
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return NotFound();
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        return Ok();
    }
    public override async Task<ActionResult<List<User>>> Get(CancellationToken cancellationToken)
    {
        var users = await _userRepository.TableNoTracking.Where(i => i.UserName != "admin" && i.UserName != "nikukiosk").ToListAsync(cancellationToken);
        return Ok(users);
        //return await base.Get(cancellationToken);
    }

    [HttpGet("[action]")]
    //[Authorize(Roles = "Admin")]
    [DynamicAuthorization(UserClaims.UserList)]
    public async Task<ActionResult<UserPaginationDto>> GetBySearch([FromQuery] UserPaginationQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }
    [Obsolete]
    public override async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
    {
        return await base.Get(id, cancellationToken);
    }
    //public override async Task<ApiResult<AccessToken>> GetToken([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
    //{
    //    return await base.GetToken(tokenRequest, cancellationToken);
    //}
    public override async Task<ActionResult> Token([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Current Native DateTime: {0}", NativeDateTime.Now());
        return await base.Token(tokenRequest, cancellationToken);
    }

    [HttpGet("[action]/{email}")]
    [Authorize]
    public virtual async Task<ApiResult<UserSelectDto>> FindByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound();

        await _userManager.UpdateSecurityStampAsync(user);

        var result = user.Adapt<UserSelectDto>();
        var claims = await _userManager.GetClaimsAsync(user);
        var temp = ClaimStore.GetClaimCollections(claims).ToList();
        result.ClaimCollections = temp.Adapt<List<ClaimCollectionDto>>();
        try
        {
            var list = _repository.TableNoTracking.Where(c => c.UserId == user.Id);
            var dtoList = await list.ProjectTo<UserProvinceDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            result.Provinces = dtoList.Adapt<List<UserProvinceDto>>();
        }
        catch (Exception ex)
        {

        }
        return Ok(result);
    }
    [HttpGet("[action]")]
    [Authorize]
    public virtual async Task<ApiResult<UserSelectDto>> FindByEmailV2([FromQuery] string email, CancellationToken cancellationToken)
    {
        User user = null;
        if (email.ToLower() == "nikukiosk")
        {
            user = await _userManager.FindByNameAsync(email);
        }
        else
        {
            user = await _userManager.FindByEmailAsync(email);
        }


        if (user == null)
            return NotFound();

        await _userManager.UpdateSecurityStampAsync(user);

        var result = user.Adapt<UserSelectDto>();
        var claims = await _userManager.GetClaimsAsync(user);
        var temp = ClaimStore.GetClaimCollections(claims).ToList();
        result.ClaimCollections = temp.Adapt<List<ClaimCollectionDto>>();
        try
        {
            var list = _repository.TableNoTracking.Where(c => c.UserId == user.Id);
            var dtoList = await list.ProjectTo<UserProvinceDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            result.Provinces = dtoList.Adapt<List<UserProvinceDto>>();
        }
        catch (Exception ex)
        {

        }

        try
        {
            var list = _uTRepository.TableNoTracking.Where(c => c.UserId == user.Id);
            var dtoList = await list.ProjectTo<UserTagDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            result.Tags = dtoList.Adapt<List<UserTagDto>>();
        }
        catch (Exception ex)
        {

        }



        return Ok(result);
    }
    public override async Task<ApiResult<User>> Create(UserDto dto, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<User>().IgnoreQueryFilters().FirstOrDefaultAsync(i => i.UserName == dto.UserName, cancellationToken);
        //var user = await _userManager.FindByEmailAsync(dto.UserName);
        if (user is null)
        {
            user = new User
            {
                UserName = dto.UserName,
                NormalizedUserName = dto.UserName.ToUpperInvariant(),
                Email = dto.UserName,
                NormalizedEmail = dto.UserName.ToUpperInvariant(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
            };
            //user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, dto.Password);
            //await _context.AddAsync(user, cancellationToken);
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(result.Errors.FirstOrDefault().Description);
        }
        else
            return BadRequest("کاربر وجود دارد!");

        var claims = dto?.Claims?.SelectMany(x => x.Claims);
        await _userManager.AddClaimsAsync(user, claims);

        var provinces = dto?.Provinces;
        if (provinces != null && provinces.Count() > 0)
        {
            var list = await _dbContext.Set<UserProvince>().Where(c => c.UserId == user.Id).ToListAsync();
            if (list.Count > 0) _dbContext.RemoveRange(list);

            var newList = new List<UserProvince>();
            foreach (var province in provinces)
            {
                newList.Add(new UserProvince() { ProvinceId = province, UserId = user.Id });
            }
            if (newList.Count > 0)
            {
                await _dbContext.AddRangeAsync(newList);
                _dbContext.SaveChanges();
            }

        }

        var tags = dto?.Tags;
        if (tags != null && tags.Count() > 0)
        {
            var newList = new List<UserTag>();
            foreach (var tag in tags)
            {
                newList.Add(new UserTag() { TagId = tag, UserId = user.Id });
            }
            if (newList.Count > 0)
            {
                try
                {
                    await _dbContext.AddRangeAsync(newList);
                    _dbContext.SaveChanges();

                }
                catch (Exception ex)
                {

                }

            }

        }

        return user;
    }
    public override async Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
    {
        return await base.Update(id, user, cancellationToken);
    }

    [HttpPut("[action]")]
    [Authorize]
    public virtual async Task<ApiResult> Update(UserUpdateDto userDto, CancellationToken cancellationToken)
    {
        //var user = await _userManager.FindByEmailAsync(userDto.UserName);
        User user = null;
        if (userDto.UserName.ToLower() == "nikukiosk")
        {
            user = await _userManager.FindByNameAsync(userDto.UserName);
        }
        else
        {
            user = await _userManager.FindByEmailAsync(userDto.UserName);
        }

        if (user is null)
        {
            return NotFound();
        }
        user = userDto.Adapt(user);
        await _userManager.RemoveClaimsAsync(user, await _userManager.GetClaimsAsync(user));
        await _userManager.AddClaimsAsync(user, userDto?.Claims?.SelectMany(x => x.Claims));

        await _userManager.UpdateAsync(user);

        var list = await _dbContext.Set<UserProvince>().Where(c => c.UserId == user.Id).ToListAsync();
        if (list.Count > 0)
        {
            _dbContext.RemoveRange(list);
            _dbContext.SaveChanges();
        }

        var provinces = userDto?.Provinces;
        if (provinces != null && provinces.Count() > 0)
        {
            var newList = new List<UserProvince>();
            foreach (var province in provinces)
            {
                newList.Add(new UserProvince() { ProvinceId = province, UserId = user.Id });
            }
            if (newList.Count > 0)
            {
                try
                {
                    await _dbContext.AddRangeAsync(newList);
                    _dbContext.SaveChanges();

                }
                catch (Exception ex)
                {

                }

            }

        }

        var tagList = await _dbContext.Set<UserTag>().Where(c => c.UserId == user.Id).ToListAsync();
        if (tagList.Count > 0)
        {
            _dbContext.RemoveRange(tagList);
            _dbContext.SaveChanges();
        }

        var tags = userDto?.Tags;
        if (tags != null && tags.Count() > 0)
        {
            var newList = new List<UserTag>();
            foreach (var tag in tags)
            {
                newList.Add(new UserTag() { TagId = tag, UserId = user.Id });
            }
            if (newList.Count > 0)
            {
                try
                {
                    await _dbContext.AddRangeAsync(newList);
                    _dbContext.SaveChanges();

                }
                catch (Exception ex)
                {

                }

            }

        }

        return Ok();

    }
    [Obsolete]
    public override async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await base.Delete(id, cancellationToken);
    }
    [HttpDelete("{email}")]
    [Authorize]
    public virtual async Task<ApiResult> DeleteByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return NotFound();
        await _userManager.DeleteAsync(user);
        return Ok();
    }

    [HttpGet("[action]")]
    [Authorize]
    public IReadOnlyList<ClaimCollectionDto> GetClaims()
    {
        return ClaimStore.AllClaims.Adapt<List<ClaimCollectionDto>>();
    }

}