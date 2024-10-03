using System.Globalization;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mahak.Api.Models.Orders.Queries.Chart;

public class GetOrderChartQueryHandler : IRequestHandler<GetOrderChartQuery, CartChartResult>
{
    private readonly IRepository<Cart> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Cart> _logger;

    public GetOrderChartQueryHandler(IRepository<Cart> repository, IMapper mapper, ILogger<Cart> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public Task<CartChartResult> Handle(GetOrderChartQuery request, CancellationToken cancellationToken)
    {
        var catIds = JsonSerializer.Deserialize<int[]>(request.CategoryIds);
        //var catIds = new[] { 2, 15 };
        var startDate = NativeDateTime.Now().AddDays(-10).ToString(CultureInfo.CurrentUICulture).GetLongDate();
        var endDate = NativeDateTime.Now().ToString(CultureInfo.CurrentUICulture).GetLongDate();

        var query = _repository.TableNoTracking;

        if (catIds.Any())
        {
            query = query.Include(d => d.Category)//.Include(c => c.CartItems).ThenInclude(b => b.Item).ThenInclude(o => o.ItemTranslations)
                .Where(i => catIds.Contains(i.Category.Id));
        }

        Guid[] tagIds = new List<Guid>().ToArray();
        if (request.Tags != null)
        {
            tagIds = JsonSerializer.Deserialize<Guid[]>(request.Tags);
            if (tagIds.Length > 0)
            {
                query = query.Include(c => c.Category).ThenInclude(n => n.Tags).Where(c => c.Category.Tags.Any(t => tagIds.Contains(t.Id)));

            }
        }

        var prIds = JsonSerializer.Deserialize<Guid[]>(request.ProvinceIds).ToList();

        if (prIds.Any())
        {
            query = query.Where(i => prIds.Contains(i.Category.ProvinceId ?? Guid.Empty));
        }

        if (request.SearchType > 0)
        {
            if (!string.IsNullOrEmpty(request.Filter))
            {
                if (request.SearchType == 1)
                    query = query.Where(i => i.RefNum.Contains(request.Filter));
                else if (request.SearchType == 2)
                    query = query.Where(i => i.CardNumber.Contains(request.Filter));
                else
                    query = query.Where(i => i.PhoneNumber.Contains(request.Filter));

            }

        }
        var list = query.ProjectTo<CartChartDto>(_mapper.ConfigurationProvider);

        if (!string.IsNullOrEmpty(request.FromDate) && !string.IsNullOrEmpty(request.ToDate))
        {
            startDate = request.FromDate.GetLongDate();
            endDate = request.ToDate.GetLongDate();
            list = list.Where(i => i.Paid && i.DateTime / 1_000_000 >= startDate && i.DateTime / 1_000_000 <= endDate);
        }

       
        var dto = list.ToList();

        //var dto = mapper.ProjectTo<CartPerChartDto>(exp);

        var result = dto.Select(i => new
        {
            perDate = i.PerDateTime,
            price = i.TotalPrice
        }).GroupBy(i => i.perDate).Select(i => new
        {
            Label = i.Key,
            Data = i.Sum(j => j.price)
        });

        return Task.FromResult(new CartChartResult
        {
            Data = result.Select(i => i.Data),
            Label = result.Select(i => i.Label)
        });
    }

}