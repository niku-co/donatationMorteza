using System.Data;
using System.Globalization;
using Data;
using Data.Contracts;
using Entities;
using Mahak.Api.Models.Items.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Category = Entities.Category;
using Item = Entities.Item;

namespace Mahak.Api.Models.DataMng.Queries;

public class GetDataItemQueryHandler : IRequestHandler<GetDataItemQuery, List<DataItemDto>>
{
    //private readonly IRepository<CategoryItem> _repository;
    private readonly IRepository<Category> _catRepository;
    private readonly ApplicationDbContext _dbContext;

    public GetDataItemQueryHandler(IRepository<Category> catRepository, ApplicationDbContext dbContext)
    {
        // _repository = repository;
        _catRepository = catRepository;
        _dbContext = dbContext;
    }
    private string add0(int digit)
    {
        return digit < 10 ? ("0" + digit) : (digit + "");
    }

    private long getLongDate(DateTime date)
    {
        return long.Parse((date.Year - 2000) + "" + add0(date.Month) + "" + add0(date.Day) + "" + add0(date.Hour) + "" + add0(date.Minute) + "" + add0(date.Second));
    }
    public Task<List<DataItemDto>> Handle(GetDataItemQuery request, CancellationToken cancellationToken)
    {
        DateTime date = DateTime.Now;
        long? startDate = getLongDate(date);
        long? endtDate = startDate;

        var p = new PersianCalendar();
        if (!string.IsNullOrEmpty(request.StartDate))
        {
            var strDate = request.StartDate.Split("-");
            date = new DateTime(int.Parse(strDate[0]), int.Parse(strDate[1]), int.Parse(strDate[2]), p);
            startDate = getLongDate(date);
        }
        if (!string.IsNullOrEmpty(request.EndDate))
        {
            var strDate = request.EndDate.Split("-");
            date = new DateTime(int.Parse(strDate[0]), int.Parse(strDate[1]), int.Parse(strDate[2]), 23, 59, 0, p);
            endtDate = getLongDate(date);
        }


        //var exp = _dbContext.Set<Item>().Include(i => i.ItemTranslations).IgnoreQueryFilters();

        //var list = exp
        //    .Join(_dbContext.Set<ItemTranslation>().Where(x => x.LanguageId == 1), k => k.Id, s => s.ItemId, (k, s) => new
        //    {
        //        s.Title,
        //        k.Price,
        //        ItemId = k.Id,
        //        k.Categories
        //    })
        //    .SelectMany(x => x.Categories, (k, s) => new
        //    {
        //        k.Title,
        //        k.ItemId,
        //        k.Price,
        //        CategoryId = s.Id
        //    })
        //    .Join(_dbContext.Set<CartItem>(), k => k.ItemId, s => s.ItemId, (k, s) => new
        //    {
        //        k.Title,
        //        k.Price,
        //        k.ItemId,
        //        k.CategoryId
        //    })
        //    .Join(_dbContext.Set<Cart>().Include(x => x.Category)
        //    .Where(j =>j.Paid && j.DateTime >= startDate && j.DateTime <= endtDate)
        //    .GroupBy(m => new { CategoryId = m.Category.Id })
        //    .Select(k => new { k.Key.CategoryId, TotalPrice = k.Sum(v => v.TotalPrice), Count = k.Count() }), k => k.CategoryId, s => s.CategoryId, (k, s) => new
        //    {
        //        k.Title,
        //        k.ItemId,
        //        k.Price,
        //        k.CategoryId,
        //        s.TotalPrice,
        //        s.Count,
        //    })
        //    .Where(d => request.CategoryIds.Contains(d.CategoryId))
        //    .GroupBy(i => new { i.Title, i.Price })
        //    .Select(group => new DataItemDto
        //    {
        //        Title = group.Key.Title,
        //        Price = group.Key.Price,
        //        TotalPrice = group.Sum(c => c.TotalPrice),
        //        Count = group.Sum(c => c.Count),

        //    });

        //var exp = _dbContext.Set<Cart>().Include(x => x.Category).Include(x=>x.CartItems)
        //    .Where(j => j.Paid && j.DateTime >= startDate && j.DateTime <= endtDate)
        //    .GroupBy(m => new { CategoryId = m.Category.Id })
        //    .Select(k => new { k.Key.CategoryId, TotalPrice = k.Sum(v => v.TotalPrice), Count = k.Count() });

        //var list = exp
        //    .Join(_dbContext.Set<Category>().Where(d => request.CategoryIds.Contains(d.Id)), k => k.CategoryId, s => s.Id, (k, s) => new
        //    {
        //        k.CategoryId
        //        ,
        //        s.Items,
        //        k.TotalPrice,
        //        k.Count,
        //    })
        //    .SelectMany(x => x.Items, (k, s) => new
        //    {
        //        k.CategoryId,
        //        ItemId = s.Id,
        //        k.TotalPrice,
        //        k.Count

        //    })
        //    .Join(_dbContext.Set<ItemTranslation>().Where(x => x.LanguageId == 1), k => k.ItemId, s => s.ItemId, (k, s) => new
        //    {
        //        s.Title,
        //        k.ItemId,
        //        k.TotalPrice,
        //        k.Count,
        //        k.CategoryId
        //    })
        //    .Join(_dbContext.Set<Item>(), k => k.ItemId, s => s.Id, (k, s) => new
        //    {
        //        k.Title,
        //        s.Price,
        //        k.ItemId,
        //        k.TotalPrice,
        //        k.Count,
        //        k.CategoryId
        //    })
        //    .GroupBy(i => new { i.Title, i.Price })
        //    .Select(group => new DataItemDto
        //    {
        //        Title = group.Key.Title,
        //        TotalPrice = group.Sum(c => c.TotalPrice),
        //        Count = group.Sum(c => c.Count),

        //    });


        var exp = _dbContext.Set<Cart>().Include(x => x.CartItems).Include(d=> d.Category)
            .Where(j => j.Paid && j.DateTime >= startDate && j.DateTime <= endtDate && request.CategoryIds.Contains(j.Category.Id))
            .SelectMany(x => x.CartItems, (k, s) => new
            {
                k.TotalPrice,
                s.ItemId
            });

        var list = exp
            .Join(_dbContext.Set<ItemTranslation>().Where(x => x.LanguageId == 1), k => k.ItemId, s => s.ItemId, (k, s) => new
            {
                s.Title,
                k.ItemId,
                k.TotalPrice,
            })
            .Join(_dbContext.Set<Item>(), k => k.ItemId, s => s.Id, (k, s) => new
            {
                k.Title,
                s.Price,
                k.ItemId,
                k.TotalPrice,
                Categories=s.Categories.Where(x => request.CategoryIds.Contains(x.Id))
            })
             //.SelectMany(x => x.Categories, (k, s) => new
             //{
             //    k.Title,
             //    k.Price,
             //    k.TotalPrice,
             //    CategoryId = s.Id
             //})
             //.Where(x => request.CategoryIds.Contains(x.CategoryId))
            .GroupBy(i => new { i.Title, i.Price })
            .Select(group => new DataItemDto
            {
                Title = group.Key.Title,
                Price = group.Key.Price,
                TotalPrice = group.Sum(c => c.TotalPrice),
                Count = group.Count(),

            });



        //if (!string.IsNullOrEmpty(request.FieldName))
        //    exp = request.SortType == SortType.Asc ? exp.OrderBy(request.FieldName) : exp.OrderByDescending(request.FieldName);

        //var result = exp.Paginate(request.Page, request.Limit);
        //var dto = result.Adapt<DataItemDto>();

        return Task.FromResult(list.ToList());
    }
}