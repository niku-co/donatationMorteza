using System.Text.Json;
using Common.Utilities;
using Data.Contracts;
using Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mahak.Api.Models.Items.Queries.Pagination;

public class GetPaginationQueryHandler : IRequestHandler<GetItemPaginationQuery, ItemPaginationDto>
{
    private readonly IRepository<Item> _repository;

    public GetPaginationQueryHandler(IRepository<Item> repository)
    {
        _repository = repository;
    }

    public Task<ItemPaginationDto> Handle(GetItemPaginationQuery request, CancellationToken cancellationToken)
    {
        var catIds = JsonSerializer.Deserialize<int[]>(request.CategoryIds);

        var exp = _repository.TableNoTracking.Include(i => i.ItemTranslations).Include(i=>i.Categories).Where(i => i.Categories.Any(c => catIds.Contains(c.Id))).AsSingleQuery();
            
        if (!string.IsNullOrEmpty(request.Filter))
            exp = exp.Where(i => i.ItemTranslations.Any(j => j.Title.Contains(request.Filter)));

        if (!string.IsNullOrEmpty(request.FieldName))
            exp = request.SortType == SortType.Asc ? exp.OrderBy(request.FieldName) : exp.OrderByDescending(request.FieldName);

        //var result = exp.Paginate(1, 10);
        var result = exp.Paginate(request.Page, request.Limit);
        var dto = result.Adapt<ItemPaginationDto>();

        return Task.FromResult(dto);
    }   
}