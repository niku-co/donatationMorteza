using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using Common.Utilities;
using Data.Contracts;
using DocumentFormat.OpenXml.Math;
using Entities;
using Entities.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceReference1;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace Mahak.Api.Models.Categories.Queries.Pagination;

public class GetPaginationQueryHandler : IRequestHandler<GetCategoryPaginationQuery, CategoryPaginationDto>
{
    private readonly IRepository<Category> _repository;
    private readonly IRepository<ProvinceSettting> _psRepository;
    private readonly IRepository<UserProvince> _upRepository;
    private readonly IRepository<UserTag> _utRepository;
    private readonly IRepository<CategoryTag> _ctRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public GetPaginationQueryHandler(IRepository<Category> repository, IMapper mapper, IRepository<ProvinceSettting> psRepository
        , IRepository<UserProvince> upRepository, UserManager<User> userManager, IRepository<UserTag> utRepository, IRepository<CategoryTag> ctRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _psRepository = psRepository;
        _upRepository = upRepository;
        _userManager = userManager;
        _utRepository = utRepository;
        _ctRepository = ctRepository;
    }

    public async Task<CategoryPaginationDto> Handle(GetCategoryPaginationQuery request, CancellationToken cancellationToken)
    {
        var list = _repository.TableNoTracking;
        var user = await _userManager.FindByNameAsync(request.Username);
        var userId = 0;
        var provinseSetting = _psRepository.TableNoTracking.FirstOrDefault();
        if (provinseSetting != null && provinseSetting.ApplyProvince)
        {

            if (user != null)
            {
                if (user.UserName == "admin")//|| user.UserName == "nikukiosk"
                {
                }
                else
                {
                    userId = user.Id;

                    var provinceIds = _upRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.ProvinceId).ToList();
                    if (provinceIds.Any())
                    {
                        list = list.Where(i => i.ProvinceId != null && provinceIds.Contains(i.ProvinceId.Value));

                    }
                }
            }

        }

        if (!string.IsNullOrEmpty(request.Tags))
        {
            if (user != null)
            {
                if (user.UserName == "admin")//|| user.UserName == "nikukiosk"
                {
                }
                else
                {
                    userId = user.Id;

                    var tagsIds = _utRepository.TableNoTracking.Where(c => c.UserId == userId).Select(c => c.TagId).ToList();
                    if (tagsIds.Any())
                    {
                        var cats = _ctRepository.TableNoTracking.Where(c => tagsIds.Contains(c.TagsId)).Select(c => c.CategoriesId).ToList();
                        list = list.Where(i => cats.Contains(i.Id));

                    }
                }
            }
            var tagIds = System.Text.Json.JsonSerializer.Deserialize<Guid[]>(request.Tags);
            if (tagIds.Length > 0)
            {
                list = list.Include(c => c.Tags).Where(c => c.Tags.Any(t => tagIds.Contains(t.Id)));

            }

        }

        if (!string.IsNullOrEmpty(request.Provinces))
        {
            Guid?[] provinceIds = System.Text.Json.JsonSerializer.Deserialize<Guid?[]>(request.Provinces);
            if (provinceIds.Length > 0)
            {
                list = list.Include(c => c.Province).Where(c => c.ProvinceId != null && provinceIds.Contains(c.ProvinceId));

            }

        }


        var exp = list.Include(c => c.Setting).ProjectTo<CategorySearchDto>(_mapper.ConfigurationProvider);
        //var exp = _repository.TableNoTracking;

        if (!string.IsNullOrEmpty(request.Filter))
        {
            switch (request.SearchType)
            {
                case 1:
                    exp = exp.Where(i => i.Title.Contains(request.Filter));
                    break;
                case 2:
                    exp = exp.Where(i => i.DeviceId.Contains(request.Filter));
                    break;
                case 3:
                    exp = exp.Where(i => i.TerminalNo.Contains(request.Filter));
                    break;
                case 4:
                    exp = exp.Where(i => i.Ip.Contains(request.Filter));
                    break;
                case 5:
                    exp = exp.Where(i => i.AnydeskCode.Contains(request.Filter));
                    break;
            }
        }
        if (request.WithoutLatLong)
        {
            exp = exp.Where(i => i.Latitude == null || i.Longitude == null);
        }

        if (request.ActiveStatus >= 0 && request.ActiveStatus <= 1)
        {
            exp = exp.Where(i => request.ActiveStatus == 1 ? i.Active : !i.Active);

        }
        else if (request.ActiveStatus == 2)
        {
            exp = exp.Where(i => i.Active);
        }
        exp = request.SortType == SortType.Asc ? exp.OrderBy(i => request.FieldName) : exp.OrderByDescending(i => request.FieldName);
        try
        {
            if (request.ActiveStatus == 2)
            {
                var data = new CategoryPaginationDto();
                var paged = new PagedModel<CategorySearchDto>();

                var page = (request.Page < 0) ? 1 : request.Page;

                paged.CurrentPage = page;
                paged.PageSize = request.Limit;


                var startRow = (page - 1) * request.Limit;
                paged.Items = exp.ToList().Where(i => !(i.State ?? false)).ToList()
                    .Skip(startRow)
                    .Take(request.Limit)
                    .ToList();

                paged.TotalItems = exp.ToList().Where(i => !(i.State ?? false)).Count();
                var dto = paged.Adapt<CategoryPaginationDto>();

                return await Task.FromResult(dto);
            }
            else
            {
                var result = exp.Paginate(request.Page, request.Limit);
                var dto = result.Adapt<CategoryPaginationDto>();


                return await Task.FromResult(dto);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
}