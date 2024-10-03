using System.Linq;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using DocumentFormat.OpenXml.Wordprocessing;
using Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Mahak.Api.Models.Orders.Queries.Pagination;

public class GetOrderPaginationQueryHandler : IRequestHandler<GetOrderPaginationQuery, PagedModel<CartSelectDto>>
{
    private readonly IRepository<Cart> _repository;
    private readonly IRepository<Entities.Category> _catRepository;
    private readonly IMapper _mapper;

    public GetOrderPaginationQueryHandler(IRepository<Cart> repository, IMapper mapper, IRepository<Entities.Category> catRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _catRepository = catRepository;
    }

    public Task<PagedModel<CartSelectDto>> Handle(GetOrderPaginationQuery request, CancellationToken cancellationToken)
    {
        var catIds = JsonSerializer.Deserialize<int[]>(request.CategoryIds);

        IQueryable<CartSelectDto> dto;

        var tagIds = JsonSerializer.Deserialize<Guid[]>(request.Tags).ToList();
        if (tagIds.Any())
        {
            if (catIds.Any())
            {
                var cats = _catRepository.TableNoTracking.Include(c => c.Tags)
                .Where(c => catIds.Contains(c.Id) && c.Tags.Any(t => tagIds.Contains(t.Id))).Select(c => c.Id).ToList<int>();

                dto = _repository.TableNoTracking.Include(c => c.Category).ProjectTo<CartSelectDto>(_mapper.ConfigurationProvider)
               .Where(i => cats.Contains(i.Category.Id)).AsSingleQuery();
            }
            else
            {
                var cats = _catRepository.TableNoTracking.Include(c => c.Tags)
                .Where(c => c.Tags.Any(t => tagIds.Contains(t.Id))).Select(c => c.Id).ToList<int>();

                dto = _repository.TableNoTracking.Include(c => c.Category).ProjectTo<CartSelectDto>(_mapper.ConfigurationProvider)
               .Where(i => cats.Contains(i.Category.Id)).AsSingleQuery();
            }

        }
        else
        {
            dto = _repository.TableNoTracking.Include(c => c.Category).ProjectTo<CartSelectDto>(_mapper.ConfigurationProvider)
           .Where(i => catIds.Length == 0 || catIds.Contains(i.Category.Id)).AsSingleQuery();
        }



        var prIds = JsonSerializer.Deserialize<Guid[]>(request.ProvinceIds).ToList();
        if (prIds.Any())
        {

            dto = dto.Where(i => prIds.Contains(i.Category.ProvinceId ?? Guid.Empty));
        }



        if (!string.IsNullOrEmpty(request.FromDate) && !string.IsNullOrEmpty(request.ToDate))
        {
            var startDate = request.FromDate.GetLongDate();
            var endDate = request.ToDate.GetLongDate();

            dto = dto.Where(i => i.Paid && i.DateTime / 1_000_000 >= startDate && i.DateTime / 1_000_000 <= endDate);
        }





        if (request.SearchType > 0)
        {
            if (!string.IsNullOrEmpty(request.Filter))
            {
                if (request.SearchType == 1)
                    dto = dto.Where(i => i.RefNum.Contains(request.Filter));
                else if (request.SearchType == 2)
                    dto = dto.Where(i => i.CardNumber.Contains(request.Filter));
                else
                    dto = dto.Where(i => i.PhoneNumber.Contains(request.Filter));

            }

        }


        if (!string.IsNullOrEmpty(request.FieldName))
            dto = request.SortType == SortType.Asc ? dto.OrderBy(request.FieldName) : dto.OrderByDescending(request.FieldName);

        if (request.ApplySum)
        {
            var grouping = dto.GroupBy(i => i.Paid).Select(c => new CartSelectDto()
            {
                TotalPrice = c.Sum(i => i.TotalPrice),
                TotalCount = c.Count(),

            });

            var result1 = grouping.Paginate(request.Page, request.Limit);
           
            return Task.FromResult(result1);
        }
        var result = dto.Paginate(request.Page, request.Limit);

        return Task.FromResult(result);
    }

}