using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Data.Contracts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Mahak.Api.Models.Orders.Queries.Pagination;

public class GetOrderSummeryPaginationQueryHandler : IRequestHandler<GetOrderSummeryPaginationQuery, List<CartSummerySelectDto>>
{
    private readonly IRepository<Cart> _repository;
    private readonly IRepository<Entities.Category> _catRepository;
    private readonly IMapper _mapper;
    private readonly IRepository<Entities.CategoryPingLog> _cplRepository;
    public GetOrderSummeryPaginationQueryHandler(IRepository<Cart> repository, IMapper mapper, IRepository<Entities.Category> catRepository, IRepository<CategoryPingLog> cplRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _catRepository = catRepository;
        _cplRepository = cplRepository;
    }

    public Task<List<CartSummerySelectDto>> Handle(GetOrderSummeryPaginationQuery request, CancellationToken cancellationToken)
    {
        var catIds = JsonSerializer.Deserialize<int[]>(request.CategoryIds);

        IQueryable<CartSummerySelectDto> dto;
        var list = _repository.TableNoTracking;

        if (!string.IsNullOrEmpty(request.FromDate) && !string.IsNullOrEmpty(request.ToDate))
        {
            var startDate = request.FromDate.GetLongDate();
            var endDate = request.ToDate.GetLongDate();

            list = list.Where(i => i.Paid && i.DateTime / 1_000_000 >= startDate && i.DateTime / 1_000_000 <= endDate);
        }

        if (request.SearchType > 0)
        {
            if (!string.IsNullOrEmpty(request.Filter))
            {
                if (request.SearchType == 1)
                    list = list.Where(i => i.RefNum.Contains(request.Filter));
                else if (request.SearchType == 2)
                    list = list.Where(i => i.CardNumber.Contains(request.Filter));
                else
                    list = list.Where(i => i.PhoneNumber.Contains(request.Filter));

            }

        }

        var tagIds = JsonSerializer.Deserialize<Guid[]>(request.Tags).ToList();
        if (tagIds.Any())
        {
            if (catIds.Any())
            {
                var cats = _catRepository.TableNoTracking.Include(c => c.Tags)
                .Where(c => catIds.Contains(c.Id) && c.Tags.Any(t => tagIds.Contains(t.Id))).Select(c => c.Id).ToList<int>();

                list = list.Include(c => c.Category)
               .Where(i => cats.Contains(i.Category.Id)).AsSingleQuery();
            }
            else
            {
                var cats = _catRepository.TableNoTracking.Include(c => c.Tags)
                .Where(c => c.Tags.Any(t => tagIds.Contains(t.Id))).Select(c => c.Id).ToList<int>();

                list = list.Include(c => c.Category)
               .Where(i => cats.Contains(i.Category.Id)).AsSingleQuery();
            }

        }
        else
        {
            list = list.Include(c => c.Category)
           .Where(i => catIds.Length == 0 || catIds.Contains(i.Category.Id)).AsSingleQuery();
        }


        var prIds = JsonSerializer.Deserialize<Guid[]>(request.ProvinceIds).ToList();
        if (prIds.Any())
        {
            list = list.Where(i => prIds.Contains(i.Category.ProvinceId ?? Guid.Empty));
        }

       
        var groupData = list
            .GroupBy(c => new { c.Category.Id, c.Category.Title });

        var data = groupData.Select(c => new CartSummerySelectDto()
        {
            CategoryTitle = c.Key.Title,
            CategoryId = c.Key.Id,
            TotalPrice = c.Sum(c => c.TotalPrice),
            TotalCount = c.Count(),

        });

        var _startDate = request.FromDate.ToShortDateTime();
        var _endDate = request.ToDate.ToShortDateTime();
        var diffDate = _endDate - _startDate;
        var totalDays = diffDate.Days + 1;

        List<CartSummerySelectDto> finalData = new List<CartSummerySelectDto>();
        IQueryable<CategoryPingLog> qry;

        if (!string.IsNullOrEmpty(request.FromDate) && !string.IsNullOrEmpty(request.ToDate))
        {
            var startDate = new DateTime((int)request.FromDate.GetLongDate("yyyy"), (int)request.FromDate.GetLongDate("MM"), (int)request.FromDate.GetLongDate("dd"));
            var endDate = new DateTime((int)request.ToDate.GetLongDate("yyyy"), (int)request.ToDate.GetLongDate("MM"), (int)request.ToDate.GetLongDate("dd"));

            qry = _cplRepository.TableNoTracking
                .Where(c => c.Date >= startDate &&
                c.Date <= endDate.AddHours(23).AddMinutes(59) && (!catIds.Any() ||
                catIds.Contains(c.CategoryId)));
        }
        else
        {
            qry = _cplRepository.TableNoTracking.Where(c => !catIds.Any() ||
                catIds.Contains(c.CategoryId));
        }

        List<CategoryPingLog> pingList = qry.ToList();

        var finalList = _catRepository.TableNoTracking
            .Where(c => pingList.Select(x => x.CategoryId).Contains(c.Id) ||
                        data.Select(x => x.CategoryId).Contains(c.Id));

        foreach (var item in finalList.ToList())
        {
            var currentList = pingList.Where(c => c.CategoryId == item.Id);

            var grpList = currentList.GroupBy(c => c.Date.Date).Select(d => new
            {
                SuccessCounter = d.Sum(c => c.SuccessCounter),
                TotalRequestCounter = d.Sum(c => c.TotalRequestCounter),
                PosSuccessCounter = d.Sum(c => c.PosSuccessCounter),
                PosTotalRequestCounter = d.Sum(c => c.PosTotalRequestCounter),

            });
            var decimalSum = (decimal)grpList.Select(x => ((decimal)(x.SuccessCounter / ((decimal)x.TotalRequestCounter > 0 ? (decimal)x.TotalRequestCounter : 1)) * 100)).Sum(c => Math.Round(c, 2));
            var decimalPosSum = (decimal)grpList.Select(x => ((decimal)(x.PosSuccessCounter / ((decimal)x.PosTotalRequestCounter > 0 ? (decimal)x.PosTotalRequestCounter : 1)) * 100)).Sum(c => Math.Round(c, 2));

            var UpTime = totalDays > 0 ? (decimalSum / (decimal.Parse(totalDays.ToString()))) : 0;
            var UpTimePos = totalDays > 0 ? (decimalPosSum / (decimal.Parse(totalDays.ToString()))) : 0;
            var TurnOnHours = currentList.Sum(x => x.TurnOnHours);

            var currentItem = data.FirstOrDefault(x => x.CategoryId == item.Id);

            finalData.Add(new CartSummerySelectDto()
            {
                CategoryId = currentItem?.CategoryId ?? item.Id,
                CategoryTitle = currentItem?.CategoryTitle ?? item.Title,
                DailyTrabsactionCount = Math.Round((currentItem?.TotalCount ?? 0) / totalDays, 0),
                DailyTrabsactionPrice = Math.Round((currentItem?.TotalPrice ?? 0) / totalDays, 0),
                TotalCount = currentItem?.TotalCount ?? 0,
                TotalPrice = currentItem?.TotalPrice ?? 0,
                AveragePrice = Math.Round((currentItem?.TotalPrice ?? 0) / ((currentItem?.TotalCount ?? 1)), 2),
                UpTime = Math.Round(UpTime, 2),
                PosUpTime = Math.Round(UpTimePos, 2),
                TurnOnHours = TurnOnHours
            }); ;
        }
        // var result = data.Paginate(request.Page, request.Limit);

        return Task.FromResult(finalData.ToList());
    }

}