using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Entities.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Services.DynamicAuthorization.DTOs.Claims;

namespace Mahak.Api.Models;

public class UserPaginationQuery : IRequest<UserPaginationDto>
{
    public string Filter { get; set; }
    public string FieldName { get; set; }
    public string Tags { get; set; }
    public int Limit { get; set; }
    public int Page { get; set; }
    public SortType SortType { get; set; }
}

public class UserPaginationHandler : IRequestHandler<UserPaginationQuery, UserPaginationDto>
{
    private readonly IRepository<User> _repository;
    private readonly IRepository<UserTag> _uTRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public UserPaginationHandler(IRepository<User> repository, UserManager<User> userManager, IMapper mapper, IRepository<UserTag> uTRepository)
    {
        _repository = repository;
        _userManager = userManager;
        _mapper = mapper;
        _uTRepository = uTRepository;
    }
    public async Task<UserPaginationDto> Handle(UserPaginationQuery request, CancellationToken cancellationToken)
    {
        var exp = _repository.TableNoTracking.Where(i => i.UserName != "admin").ProjectTo<UserSelectDto>(_mapper.ConfigurationProvider);

        if (!string.IsNullOrEmpty(request.Tags))
        {
            var tagIds = System.Text.Json.JsonSerializer.Deserialize<Guid[]>(request.Tags);
            if (tagIds.Length > 0)
            {
                var ut = _uTRepository.TableNoTracking.Where(c => tagIds.Contains(c.TagId)).Select(c => c.UserId);
                exp = exp.Where(c => ut.Contains(c.Id));
            }
        }
        if (!string.IsNullOrEmpty(request.Filter))
            exp = exp.Where(i => i.UserName.Contains(request.Filter));
        exp = request.SortType == SortType.Asc ? exp.OrderBy(i => request.FieldName) : exp.OrderByDescending(i => request.FieldName);

        var result = exp.Paginate(request.Page, request.Limit);
        var dto = result.Adapt<UserPaginationDto>();

        foreach (var userDto in dto.Items)
        {
            var user = userDto.Adapt<User>();
            var userClaims = await _userManager.GetClaimsAsync(user);
            userDto.ClaimCollections = ClaimStore.GetClaimCollections(userClaims).Adapt<List<ClaimCollectionDto>>();
        }
        return dto;
    }
}
