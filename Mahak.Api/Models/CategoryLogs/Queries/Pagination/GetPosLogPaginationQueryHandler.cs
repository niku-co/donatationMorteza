using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mahak.Api.Models.Items.Queries.Pagination;

public class GetPosLogPaginationQueryHandler : IRequestHandler<GetPosLogPaginationQuery, CategoryLogPaginationDto>
{
    private readonly IRepository<CategoryLog> _repository;
    private readonly IMapper _mapper;
    public GetPosLogPaginationQueryHandler(IRepository<CategoryLog> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public Task<CategoryLogPaginationDto> Handle(GetPosLogPaginationQuery request, CancellationToken cancellationToken)
    {
        var catIds = JsonSerializer.Deserialize<int[]>(request.CategoryIds);

        var exp = _repository.TableNoTracking.Include(i => i.Category).Where(i => catIds.Contains(i.CategoryId)).AsSingleQuery();

        if (request.StartDate.HasValue)
            request.StartDate = request.StartDate.Value.Date;
        if (request.EndDate.HasValue)
            request.EndDate = request.EndDate.Value.Date.AddHours(23).AddMinutes(59);
        exp = exp.Where(c => c.InsertTime >= request.StartDate && c.InsertTime <= request.EndDate);

        //if (!string.IsNullOrEmpty(request.Filter))
        //    exp = exp.Where(i => i.ItemTranslations.Any(j => j.Title.Contains(request.Filter)));

        if (!string.IsNullOrEmpty(request.FieldName))
            exp = request.SortType == SortType.Asc ? exp.OrderBy(request.FieldName) : exp.OrderByDescending(request.FieldName);

        //var result = exp.Paginate(1, 10);
        var result = exp.ProjectTo<CategoryLogDto>(_mapper.ConfigurationProvider).Paginate(request.Page, request.Limit);
        var dto = result.Adapt<CategoryLogPaginationDto>();

        return Task.FromResult(dto);
    }
}